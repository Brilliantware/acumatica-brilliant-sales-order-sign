using PX.Common;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.AR;
using PX.Objects.GL;
using PX.Objects.SO;
using PX.SM;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Brilliant.SalesOrderSign;

public class ProcessSignature : PXGraph<ProcessSignature>
{
  public class signatureMatch : BqlType<IBqlString, string>.Constant<signatureMatch>
  {
    public signatureMatch(): base("signature%")
    {
    }
  }
  

  public PXCancel<SOOrder> Cancel; 

  public SelectFrom<SOOrder>
    .InnerJoin<NoteDoc>.On<NoteDoc.noteID.IsEqual<SOOrder.noteID>>
    .InnerJoin<UploadFile>.On<UploadFile.fileID.IsEqual<NoteDoc.fileID>.And<UploadFile.name.IsLike<signatureMatch>>>
    .Where<SOOrder.noteID.IsNotNull>
    .AggregateTo<GroupBy<SOOrder.orderNbr>, GroupBy<SOOrder.orderType>>
    .ProcessingView SalesOrders;

  /*public PXProcessing<SOOrder, 
    
    Where<SOOrder.noteID.IsNotNull>> SalesOrders;*/

  public const string signatureAssigned = "The {0} signature has been successfully attached.";

  public ProcessSignature()
  {
    SalesOrders.SetProcessDelegate(
      delegate (List<SOOrder> list)
      {
        List<SOOrder> newlist = new List<SOOrder>(list.Count);
        foreach (SOOrder doc in list)
        {
          newlist.Add(doc);
        }
        Process(newlist, true);
      }
    );

  }

  public static void Process(SOOrder doc)
  {
    List<SOOrder> list = new List<SOOrder>();

    list.Add(doc);
    Process(list, false);
  }

  public static void Process(List<SOOrder> list, bool isMassProcess)
  {
    Stopwatch sw = new Stopwatch();
    sw.Start();
    var rg = CreateInstance<SOOrderEntry>();
    var inv = CreateInstance<SOInvoiceEntry>();
    var upload = PXGraph.CreateInstance<UploadFileMaintenance>();
    sw.Stop();
    Debug.Print("{0} PXGraph.CreateInstance<SOOrderEntry> in {1} millisec", DateTime.Now.TimeOfDay, sw.ElapsedMilliseconds);
    int i = 0;
    foreach (var order in list)
    {
      try
      {
        rg.Clear();
        sw.Reset();
        sw.Start();

        if (PXSelect<ARTran, Where<ARTran.sOOrderNbr, Equal<Required<SOOrder.orderNbr>>>>.Select(inv, order.OrderNbr).RowCast<ARTran>().FirstOrDefault() is ARTran tran)
        {
          if (PXSelect<ARInvoice, Where<ARInvoice.refNbr, Equal<Required<ARTran.refNbr>>>>.Select(inv, tran.RefNbr).RowCast<ARInvoice>().FirstOrDefault() is ARInvoice invoice)
          {
            rg.Document.Current = order;
            rg.Document.UpdateCurrent();

            inv.Document.Current = invoice;
            inv.Document.UpdateCurrent();

            Guid[] files = PXNoteAttribute.GetFileNotes(rg.Document.Cache, rg.Document.Current);
            foreach (Guid fileID in files)
            {
              FileInfo fileInfoRow = upload.GetFileWithNoData(fileID);
              if (fileInfoRow != null && fileInfoRow.FullName.Contains("signature") == true)
              {
                PXNoteAttribute.SetFileNotes(inv.Document.Cache, inv.Document.Current, fileInfoRow.UID.Value);
                inv.Save.Press();
              }
            }
          }
        }
        sw.Stop();
        Debug.Print("{0} Select SOOrder in {1} millisec", DateTime.Now.TimeOfDay, sw.ElapsedMilliseconds);
        PXProcessing<SOOrder>.SetInfo(i, ActionsMessages.RecordProcessed);
      }
      catch (Exception e)
      {
        if (isMassProcess)
        {
          PXProcessing<SOOrder>.SetError(i, e is PXOuterException ? e.Message + "\r\n" + String.Join("\r\n", ((PXOuterException)e).InnerMessages) : e.Message);
        }
        else
        {
          throw new PXMassProcessException(i, e);
        }
      }
      i++;
    }
  }

  public bool IsAnySignatureAttached(PXCache cache, SOOrder order)
  {
    Guid[] files = PXNoteAttribute.GetFileNotes(cache, order);
    var uploadFileMaintenance = CreateInstance<UploadFileMaintenance>();

    foreach (Guid fileID in files)
    {
      FileInfo fileInfoRow = uploadFileMaintenance.GetFileWithNoData(fileID);
      if (fileInfoRow != null && fileInfoRow.FullName.Contains("signature") == true)
      {
        return true;
      }
    }
    return false;
  }
}

