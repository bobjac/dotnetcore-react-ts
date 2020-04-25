#!/bin/sh

echo 'Installing pyodbc Python package'
pip3 install pyodbc

CONTAINER="mcr.microsoft.com/mssql/server:2019-CU3-ubuntu-18.04"
SA_PASSWORD="<INSERT PASSWORD HERE>"

python3 ./setup.py $CONTAINER $SA_PASSWORD