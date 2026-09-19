from pydantic import BaseModel


class MessageCreationRequest(BaseModel):
    content: str
    user_id: str = "anonymous"
