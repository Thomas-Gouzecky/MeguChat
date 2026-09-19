from pydantic import BaseModel, Field


class AddMembersRequest(BaseModel):
    users: str | list[str] = Field(
        ..., description="The ID of the user to add to the group chat"
    )
