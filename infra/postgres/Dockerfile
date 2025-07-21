FROM postgres:16

COPY ./docker-entrypoint-initdb.d/init-databases.sh /docker-entrypoint-initdb.d/init-databases.sh
RUN chmod +x /docker-entrypoint-initdb.d/init-databases.sh

EXPOSE 5432