CREATE ROLE replicator WITH REPLICATION LOGIN PASSWORD 'replication-password';

GRANT CONNECT ON DATABASE meguchat TO replicator;