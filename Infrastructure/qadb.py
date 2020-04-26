import os
import subprocess
import textwrap
import pyodbc
import time

def create_sql_container(sql_container: str, sa_password: str) -> int:
    f = os.popen('date')
    now = f.read()
    print("Today is ", now)

    sa_password_param = "\"SA_PASSWORD=" + sa_password + "\""
    print("sa_password: " + sa_password_param)
    print("sql_container: " + sql_container)

    command = "docker run -e \"ACCEPT_EULA=Y\" -e \"SA_PASSWORD=AFw2hawabf23\" -p 1444:1433 --name sql2 -d mcr.microsoft.com/mssql/server:2019-CU3-ubuntu-18.04"
    return_code = subprocess.call([command],shell=True,stderr=subprocess.STDOUT)
    #return_code =  subprocess.call(["docker", "run", "-e", "\"ACCEPT_EULA=Y\"", "-e", sa_password_param, "-p", "1444:1433", "--name", "sql2", "-d", sql_container], shell=True, stderr=subprocess.STDOUT)
    
    time.sleep(10)
    return return_code

def create_sql_tables():
    print("pyodbc successfully imported")

   # cnx = pyodbc.connect(
  #      server="127.0.0.1",
  #      database="master",
  #      user='SA',
  #      tds_version='7.3',
 #       password="AFw2hawabf23",
 #       port=1444,
  #      driver='/usr/local/lib/libtdsodbc.so'
  #  )

    server = '127.0.0.1,1444'
    database = 'master'
    username = 'SA'
    password = 'AFw2hawabf23'
    cnxn = pyodbc.connect('DRIVER={ODBC Driver 17 for SQL Server};SERVER='+server+';DATABASE='+database+';UID='+username+';PWD='+ password)
    cursor = cnxn.cursor()
    cursor.execute("SELECT @@version;") 
    row = cursor.fetchone() 

    while row: 
        print(row[0])
        row = cursor.fetchone()

    cursor.close()
    cnxn.close()

def create_query_string(sql_full_path):
    with open(sql_full_path, 'r') as f_in:
        lines = f_in.read()
    
    query = textwrap.dedent("""{}""").format(lines)
    return query
