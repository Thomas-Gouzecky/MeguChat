from pydantic import BaseModel, Field


class GroupChatCreationRequest(BaseModel):
    name: str
    users: list[str] = Field(
        default_factory=list, description="List of user IDs to add to the group chat"
    )
