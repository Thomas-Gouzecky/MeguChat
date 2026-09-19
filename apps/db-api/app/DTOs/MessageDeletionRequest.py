from pydantic import BaseModel


class MessageDeletionRequest(BaseModel):
    user_id: str