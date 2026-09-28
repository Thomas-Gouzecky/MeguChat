from datetime import datetime

from pydantic import BaseModel


class GroupChatMemberDto(BaseModel):
    id: int | None
    group_chat_id: int
    user_id: str
    joined_at: datetime | None
    last_active_at: datetime | None
    last_read_message_id: int | None
