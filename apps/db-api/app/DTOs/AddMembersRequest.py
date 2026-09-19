from pydantic import BaseModel, Field


class AddMembersRequest(BaseModel):
    user_id: str = Field(..., description="The ID of the user to add to the group chat")
