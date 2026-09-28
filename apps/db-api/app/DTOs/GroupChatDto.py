from datetime import datetime

from pydantic import BaseModel


class GroupChatDto(BaseModel):
    groupchat_id: int
    name: str | None = None
    created_at: datetime | None = None
