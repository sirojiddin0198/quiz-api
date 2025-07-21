#!/bin/bash

set -e
set -u

function create_user_and_database() {
    local database=$1
    local user=$2
    local password=$3
    echo "Creating user '$user' and database '$database'"
    psql -v ON_ERROR_STOP=0 --username "$POSTGRES_USER" --dbname "postgres" <<-EOSQL
        DO \$\$
        BEGIN
            IF NOT EXISTS (SELECT FROM pg_user WHERE usename = '$user') THEN
                CREATE USER $user WITH PASSWORD '$password';
            ELSE
                ALTER USER $user WITH PASSWORD '$password';
            END IF;
        END
        \$\$;
        CREATE DATABASE $database;
        GRANT ALL PRIVILEGES ON DATABASE $database TO $user;
EOSQL
    
    # Connect to the new database to grant schema privileges
    psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$database" <<-EOSQL
        GRANT USAGE, CREATE ON SCHEMA public TO $user;
EOSQL
}

if [ -n "${DBCONFIGS:-}" ]; then
    echo "Multiple database creation requested: $DBCONFIGS"
    for dbconfig in $(echo $DBCONFIGS | tr ',' ' '); do
        echo "Processing dbconfig: $dbconfig"
        IFS=':' read -r db user password <<< "$dbconfig"
        echo "Creating database: $db, user: $user"
        create_user_and_database $db $user $password
    done
    echo "Multiple databases created"
fi