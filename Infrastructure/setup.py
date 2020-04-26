import sys
import qadb

def main(container_name, sa_password):
    # create the container
    return_code = qadb.create_sql_container(container_name, sa_password)
    print("The return code from creating the container is " + str(return_code))

    if return_code == 0:
        # create the tables in the database
        qadb.create_database(sa_password)
        qadb.create_sql_tables(sa_password)
        qadb.create_sql_procs(sa_password)
    else:
        print("There was an error creating the container")
        

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
