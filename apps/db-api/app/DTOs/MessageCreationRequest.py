from pydantic import BaseModel


class MessageCreationRequest(BaseModel):
    content: str
