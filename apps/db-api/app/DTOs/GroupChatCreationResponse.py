from pydantic import BaseModel


class GroupChatCreationResponse(BaseModel):
    groupchat_id: int
