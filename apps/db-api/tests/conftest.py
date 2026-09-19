import pytest

from app.sql.session import SQLModel, create_db_and_tables, engine


@pytest.fixture(autouse=True)
def reset_database():
    create_db_and_tables()
    yield
    SQLModel.metadata.drop_all(engine)