from datetime import datetime

from pydantic import BaseModel


class MessageDto(BaseModel):
    id: int | None
    user_id: str
    group_chat_id: int
    content: str
    created_at: datetime | None
    modified_at: datetime | None
