from datetime import datetime

from pydantic import BaseModel


class GroupChatCreationResponse(BaseModel):
    groupchat_id: int
    name: str | None = None
    created_at: datetime | None = None
