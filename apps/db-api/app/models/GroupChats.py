from datetime import datetime
from sqlmodel import Field, SQLModel, Column
from sqlalchemy import DateTime, Text, Integer, text


class GroupChats(SQLModel, table=True):
    __tablename__: str = "GroupChats"

    id: int | None = Field(
        default=None,
        sa_column=Column(Integer, primary_key=True),
    )
    name: str = Field(sa_column=Column(Text, nullable=False))
    created_at: datetime | None = Field(
        default=None,
        sa_column=Column(
            DateTime(timezone=True),
            server_default=text("CURRENT_TIMESTAMP"),
            nullable=False,
        ),
    )
