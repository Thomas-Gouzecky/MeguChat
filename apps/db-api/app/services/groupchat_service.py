from sqlmodel import Session

from app.models import GroupChats
from app.repositories.groupchat_repo import create_a_new_groupchat_entry
from app.DTOs import GroupChatCreationRequest, GroupChatCreationResponse


def create_groupchat(
    request_body: GroupChatCreationRequest,
    session: Session,
) -> GroupChatCreationResponse:

    groupchat_request = GroupChats(
        name=request_body.name,
        users=request_body.users,
    )

    groupchat = create_a_new_groupchat_entry(
        groupchat_request,
        session,
    )

    if groupchat.id is None or not isinstance(groupchat.id, int):
        raise ValueError("Groupchat ID is not an integer")

    return GroupChatCreationResponse(groupchat_id=groupchat.id)
