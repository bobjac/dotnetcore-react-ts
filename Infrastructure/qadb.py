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

    command = "docker run -e \"ACCEPT_EULA=Y\" -e \"SA_PASSWORD="+sa_password+"\" -p 1444:1433 --name sql2 -d mcr.microsoft.com/mssql/server:2019-CU3-ubuntu-18.04"
    return_code = subprocess.call([command],shell=True,stderr=subprocess.STDOUT)

    #sleep to allow time for sql daemon to start    
    time.sleep(10)

    return return_code

def create_database(sa_password):
    cnxn = get_connection("master", sa_password)
    cursor = cnxn.cursor()
    result = cursor.execute("CREATE DATABASE QnA") 
    print(result)
    cursor.close()
    cnxn.close()

def create_sql_tables(sa_password):
    file_path = os.getcwd() + "/Sql/create_tables.sql"
    execute_sql_script(file_path, sa_password)

def execute_sql_script(script_path, sa_password):
    cnxn = get_connection("QnA", sa_password)
    c = cnxn.cursor()

    sql_query = ''
    with open(script_path, 'r') as inp:
        for line in inp:
            if line == 'GO\n':
                c.execute(sql_query)
                sql_query = ''
            elif 'PRINT' in line:
                disp = line.split("'")[1]
                print(disp, '\r')
            else:
                sql_query = sql_query + line
    
    c.close()
    cnxn.close()

def get_connection(database, sa_password):
    server = '127.0.0.1,1444'
    database = database
    username = 'SA'
    password = sa_password
    conn_str = 'DRIVER={ODBC Driver 17 for SQL Server};SERVER='+server+';DATABASE='+database+';UID='+username+';PWD='+ password

    cnxn = pyodbc.connect(conn_str, autocommit=True)
    return cnxn

def create_query_string(sql_full_path):
    with open(sql_full_path, 'r') as f_in:
        lines = f_in.read()
    
    query = textwrap.dedent("""{}""").format(lines)
    return query
