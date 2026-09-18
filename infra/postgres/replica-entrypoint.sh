#!/bin/bash
set -e

: "${PGDATA:=/var/lib/postgresql/18/docker}"
: "${POSTGRES_PRIMARY_HOST:=postgres-primary}"
: "${POSTGRES_PRIMARY_PORT:=5432}"
: "${REPLICATION_USER:=replicator}"
: "${REPLICATION_PASSWORD:=replication-password}"

if [ ! -f "$PGDATA/standby.signal" ]; then
  echo "Initializing PostgreSQL standby from ${POSTGRES_PRIMARY_HOST}:${POSTGRES_PRIMARY_PORT}"
  rm -rf "$PGDATA"/*

  until pg_isready -h "$POSTGRES_PRIMARY_HOST" -p "$POSTGRES_PRIMARY_PORT" -U "$POSTGRES_USER" -d postgres; do
    sleep 1
  done

  export PGPASSWORD="$REPLICATION_PASSWORD"
  pg_basebackup \
    -h "$POSTGRES_PRIMARY_HOST" \
    -p "$POSTGRES_PRIMARY_PORT" \
    -D "$PGDATA" \
    -U "$REPLICATION_USER" \
    -Fp \
    -Xs \
    -P \
    -R
fi

exec /usr/local/bin/docker-entrypoint.sh postgres "$@"
