import sys
import qadb

def main(container_name, sa_password):
    # create the container
    container = qadb.create_sql_container(container_name, sa_password)

    # create the tables in the database
    tables = qadb.create_sql_tables()

if __name__ == "__main__":
    if len(sys.argv) < 3:
        print("This script will create the containers needed to run this sample locally.") 
        print("Usage:   python3 setup.py <Container> <SA Password>")
    else:
        container_name = sys.argv[1]
        print("The first argument is ", container_name)

        sa_password = sys.argv[2]
        print("The second argument is ", sa_password)

        main(container_name, sa_password)
