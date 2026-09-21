from dotenv import load_dotenv
import os

load_dotenv(".env.dev")


def get_required_setting(name: str) -> str:
    value = os.getenv(name)
    if not value:
        raise RuntimeError(
            f"Missing required database setting: {name}. "
            "Set it in the environment or apps/db-api/.env.dev."
        )
    return value


PRIMARY_DB_USER = get_required_setting("PRIMARY_DATABASE_USER")
PRIMARY_DB_PASSWORD = get_required_setting("PRIMARY_DATABASE_PASSWORD")
PRIMARY_DB_HOST = get_required_setting("PRIMARY_DATABASE_HOST")
PRIMARY_DB_PORT = get_required_setting("PRIMARY_DATABASE_PORT")
PRIMARY_DB_NAME = get_required_setting("PRIMARY_DATABASE_NAME")

PRIMARY_DB_STRING = (
    f"postgresql+psycopg://{PRIMARY_DB_USER}:{PRIMARY_DB_PASSWORD}"
    f"@{PRIMARY_DB_HOST}:{PRIMARY_DB_PORT}/{PRIMARY_DB_NAME}"
)

REPLICA_DB_USER = get_required_setting("REPLICA_DATABASE_USER")
REPLICA_DB_PASSWORD = get_required_setting("REPLICA_DATABASE_PASSWORD")
REPLICA_DB_HOST = get_required_setting("REPLICA_DATABASE_HOST")
REPLICA_DB_PORT = get_required_setting("REPLICA_DATABASE_PORT")
REPLICA_DB_NAME = get_required_setting("REPLICA_DATABASE_NAME")

REPLICA_DB_STRING = (
    f"postgresql+psycopg://{REPLICA_DB_USER}:{REPLICA_DB_PASSWORD}"
    f"@{REPLICA_DB_HOST}:{REPLICA_DB_PORT}/{REPLICA_DB_NAME}"
)
