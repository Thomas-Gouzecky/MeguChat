from datetime import datetime, timezone

from sqlmodel import Field, SQLModel, Column
from sqlalchemy import DateTime, ForeignKey, Integer, Text, text


class Messages(SQLModel, table=True):
    __tablename__: str = "Messages"

    id: int | None = Field(sa_column=Column(Integer, default=None, primary_key=True))
    user_id: str = Field(sa_column=Column(Text, nullable=False))
    group_chat_id: int = Field(
        sa_column=Column(
            Integer,
            ForeignKey("GroupChats.id"),
            nullable=False,
        )
    )
    message: str = Field(sa_column=Column(Text, default=""))
    created_at: datetime = Field(
        sa_column=Column(
            DateTime(timezone=True),
            server_default=text("CURRENT_TIMESTAMP"),
            nullable=False,
        ),
    )
    modified_at: datetime = Field(
        sa_column=Column(
            DateTime(timezone=True),
            server_default=text("CURRENT_TIMESTAMP"),
            onupdate=lambda: datetime.now(timezone.utc),
            nullable=False,
        )
    )
