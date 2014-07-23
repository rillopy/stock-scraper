#!/bin/bash
tar -pcjf rawlogs.tar.bz2 /home/azureuser/*.log

_now=$(date +"%m_%d_%Y")
_file="/home/azureuser/sp500_$_now.log"

files=(sp500*.log)

for ((i=${#files[@]}-1; i>= 0; i--));
do
    _output="`cat "${files[$i]}" | mono --runtime=v4.0.30319 /home/azureuser/parser/StandardParser.exe`"
    echo "${files[$i]} $_output"
done

