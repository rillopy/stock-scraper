#!/bin/bash

_now=$(date +"%Y_%m_%d")
_file="/home/rillopy/sp500_$_now.log"

mono /home/rillopy/StandardCrawler/StandardCrawler/bin/Debug/StandardCrawler.exe > "$_file" 2>/dev/null
