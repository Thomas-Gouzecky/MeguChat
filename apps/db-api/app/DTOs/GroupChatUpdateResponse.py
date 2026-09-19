from datetime import datetime

from pydantic import BaseModel


class GroupChatUpdateResponse(BaseModel):
    groupchat_id: int
    name: str | None = None
    created_at: datetime | None = None
