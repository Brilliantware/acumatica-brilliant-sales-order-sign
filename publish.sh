#!/usr/bin/env sh
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" &> /dev/null && pwd )"
FILE=$(basename $0)
$DIR/Brilliant.Postbuild.exe copy publish --config "$DIR/Brilliant.Postbuild.config"