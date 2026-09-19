from datetime import datetime
from sqlmodel import Field, ForeignKey, SQLModel, Column
from sqlalchemy import DateTime, Text, Integer, UniqueConstraint, text


class GroupChatMembers(SQLModel, table=True):
    __tablename__: str = "GroupChatMembers"
    __table_args__ = (
        UniqueConstraint(
            "group_chat_id",
            "user_id",
            name="uq_group_chat_members_group_chat_user",
        ),
    )

    id: int | None = Field(
        default=None,
        sa_column=Column(Integer, primary_key=True),
    )
    user_id: str = Field(sa_column=Column(Text, nullable=False))
    group_chat_id: int = Field(
        sa_column=Column(
            Integer,
            ForeignKey("GroupChats.id"),
            nullable=False,
        )
    )
    joined_at: datetime | None = Field(
        default=None,
        sa_column=Column(
            DateTime(timezone=True),
            server_default=text("CURRENT_TIMESTAMP"),
            nullable=False,
        ),
    )
    last_active_at: datetime | None = Field(
        default=None,
        sa_column=Column(
            DateTime(timezone=True),
            server_default=text("CURRENT_TIMESTAMP"),
            onupdate=lambda: datetime.now(),
            nullable=False,
        ),
    )
    last_read_message_id: int | None = Field(
        default=None,
        sa_column=Column(
            Integer,
            ForeignKey("Messages.id"),
            nullable=True,
        ),
    )
