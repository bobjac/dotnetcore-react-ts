#!/bin/sh

echo 'Installing pyodbc Python package'
pip3 install pyodbc

CONTAINER="mcr.microsoft.com/mssql/server:2019-CU3-ubuntu-18.04"
SA_PASSWORD="<PASSWORD>"

#docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=<password>" \
#   -p 1444:1433 --name sql2 \
#   -d mcr.microsoft.com/mssql/server:2019-CU3-ubuntu-18.04

python3 ./setup.py $CONTAINER $SA_PASSWORD
