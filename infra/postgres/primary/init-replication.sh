#!/bin/bash
set -e

echo "host replication replicator 172.24.0.0/16 scram-sha-256" >> "$PGDATA/pg_hba.conf"
