from sqlmodel import Session

from app.models import GroupChats
from app.repositories.groupchat_repo import create_a_new_groupchat_entry
from app.DTOs import GroupChatCreationResponse


def create_groupchat(
    request_body: GroupChats,
    session: Session,
) -> GroupChatCreationResponse:
    groupchat = create_a_new_groupchat_entry(
        request_body,
        session,
    )

    if groupchat.id is None or not isinstance(groupchat.id, int):
        raise ValueError("Groupchat ID is not an integer")

    return GroupChatCreationResponse(groupchat_id=groupchat.id)
