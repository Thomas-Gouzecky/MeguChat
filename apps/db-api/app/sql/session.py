from typing import Annotated
from fastapi import Depends
from sqlmodel import Session, create_engine, SQLModel

from app.configs import PRIMARY_DB_STRING
from app.models import GroupChats, GroupChatMembers, Messages

engine = create_engine(PRIMARY_DB_STRING)


def migrate_database():
    SQLModel.metadata.create_all(engine)


def create_db_and_tables():
    migrate_database()


def get_session():
    with Session(engine) as session:
        yield session


SessionDep = Annotated[Session, Depends(get_session)]
