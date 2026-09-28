from datetime import datetime

from pydantic import BaseModel


class GroupChatDto(BaseModel):
    groupchat_id: int
    name: str | None
    created_at: datetime | None
