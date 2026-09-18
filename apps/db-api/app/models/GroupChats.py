from datetime import datetime
from sqlmodel import Field, SQLModel, Column
from sqlalchemy import DateTime, Text, Integer, text
from sqlalchemy.dialects.postgresql import ARRAY


class GroupChats(SQLModel, table=True):
    __tablename__: str = "GroupChats"

    id: int | None = Field(sa_column=Column(Integer, default=None, primary_key=True))
    name: str = Field(sa_column=Column(Text, nullable=False))
    users: list[str] = Field(
        default_factory=list,
        sa_column=Column(ARRAY(Text), nullable=False),
    )
    created_at: datetime = Field(
        sa_column=Column(
            DateTime(timezone=True),
            server_default=text("CURRENT_TIMESTAMP"),
            nullable=False,
        ),
    )
