#!/bin/bash

_now=$(date +"%m_%d_%Y")
_file="/home/rillopy/sp500_$_now.log"

files=(sp500*.log)

for ((i=${#files[@]}-1; i>= 0; i--));
do
    _output="`cat "${files[$i]}" | mono /home/rillopy/StandardCrawler/StandardParser/bin/Debug/StandardParser.exe`"
    echo "${files[$i]} $_output"
done
