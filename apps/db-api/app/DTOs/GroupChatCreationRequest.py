from pydantic import BaseModel


class GroupChatCreationRequest(BaseModel):
    users: list[str]
    name: str
