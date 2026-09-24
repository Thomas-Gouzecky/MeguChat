from datetime import datetime

from pydantic import BaseModel


class GroupChatMembersResponse(BaseModel):
    id: int | None = None
    group_chat_id: int
    user_id: str
    joined_at: datetime | None = None
    last_active_at: datetime | None = None
    last_read_message_id: int | None = None
