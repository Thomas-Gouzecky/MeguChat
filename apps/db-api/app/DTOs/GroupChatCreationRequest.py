from pydantic import BaseModel, Field


class GroupChatCreationRequest(BaseModel):
    name: str
