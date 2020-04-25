import os
import subprocess
import pyodbc

def create_sql_container(sql_container, sa_password):
    f = os.popen('date')
    now = f.read()
    print("Today is ", now)

    sa_password_param = "\"SA_Password=" + sa_password + "\""
    print("sa_password: " + sa_password_param)

    print("sql_container: " + sql_container)

    subprocess.call(["docker", "run", "-e", "\"ACCEPT_EULA\"", "-e", sa_password_param, "-p", "1444:1433", "--name", "sql2", "-d", sql_container])

def create_sql_tables():
    print("pyodbc successfully imported")
