<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListView.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="BLSO30IN.aspx.cs" Inherits="Page_BLSO30IN" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/ListView.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="Brilliant.SalesOrderSign.ProcessSignature"
        PrimaryView="SalesOrders"
        >
		<CallbackCommands>

		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phL" runat="Server">
	<px:PXGrid SyncPosition="True" ID="grid" runat="server" DataSourceID="ds" Width="100%" Height="150px" SkinID="Primary" AllowAutoHide="false">
		<Levels>
			<px:PXGridLevel DataMember="SalesOrders">
			    <Columns>
				<px:PXGridColumn AllowCheckAll="True" Type="CheckBox" DataField="Selected" Width="60" ></px:PXGridColumn>
				<px:PXGridColumn DataField="OrderType" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="OrderNbr" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn DataField="Status" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="OrderDate" Width="90" ></px:PXGridColumn>
				<px:PXGridColumn DataField="BranchID" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn DataField="CustomerID" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn DataField="OrderQty" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="CreatedByID" Width="280" ></px:PXGridColumn>
				<px:PXGridColumn DataField="CreatedDateTime" Width="90" ></px:PXGridColumn></Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" ></AutoSize>
		<ActionBar >
		</ActionBar>
	</px:PXGrid>
</asp:Content>