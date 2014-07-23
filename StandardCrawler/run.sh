#!/bin/bash
 
_now=$(date +"%Y_%m_%d")
_file="/home/azureuser/sp500_$_now.log"

mono --runtime=v4.0.30319 /home/azureuser/crawler/StandardCrawler.exe > "$_file" 2>/dev/null
