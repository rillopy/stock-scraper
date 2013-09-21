#!/bin/bash

_now=$(date +"%m_%d_%Y")
_file="/home/rillopy/sp500_$_now.log"

for file in sp500*.log
do
    _output="`cat "$file" | mono /home/rillopy/StandardCrawler/StandardParser/bin/Debug/StandardParser.exe`"
    echo "$file $_output"
done
