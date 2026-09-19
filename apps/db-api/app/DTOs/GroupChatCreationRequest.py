from pydantic import BaseModel, Field


class GroupChatCreationRequest(BaseModel):
    users: list[str] = Field(min_length=1)
    name: str
