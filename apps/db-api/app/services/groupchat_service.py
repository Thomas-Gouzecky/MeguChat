from sqlmodel import Session

from app.models import GroupChats
from app.repositories.groupchat_repo import (
    create_a_new_groupchat_entry,
    find_groupchats_for_user as find_groupchats_for_user_in_repository,
    update_groupchat as update_groupchat_in_repository,
    delete_groupchat as delete_groupchat_in_repository,
    get_messages_for_groupchat as get_messages_for_groupchat_in_repository,
)
from app.DTOs import (
    GroupChatCreationRequest,
    GroupChatCreationResponse,
    GroupChatUpdateResponse,
)


def create_groupchat(
    request_body: GroupChatCreationRequest,
    session: Session,
) -> GroupChatCreationResponse:

    groupchat_request = GroupChats(
        name=request_body.name,
    )

    groupchat = create_a_new_groupchat_entry(
        groupchat_request,
        session,
    )

    if groupchat.id is None or not isinstance(groupchat.id, int):
        raise ValueError("Groupchat ID is not an integer")

    return GroupChatCreationResponse(groupchat_id=groupchat.id)


def find_groupchats_for_user(
    user_id: str,
    session: Session,
) -> list[GroupChatCreationResponse]:

    groupchats = find_groupchats_for_user_in_repository(user_id, session)
    return [
        GroupChatCreationResponse(
            groupchat_id=groupchat.id,
            name=groupchat.name,
            created_at=groupchat.created_at,
        )
        for groupchat in groupchats
        if groupchat.id is not None
    ]


def update_groupchat(
    groupchat_id: int,
    request_body: GroupChatCreationRequest,
    session: Session,
) -> GroupChatUpdateResponse:
    groupchat_request = GroupChats(name=request_body.name)
    groupchat = update_groupchat_in_repository(
        groupchat_id,
        groupchat_request,
        session,
    )

    if groupchat.id is None:
        raise ValueError("Groupchat ID is missing")

    return GroupChatUpdateResponse(
        groupchat_id=groupchat.id,
        name=groupchat.name,
        created_at=groupchat.created_at,
    )


def delete_groupchat(
    groupchat_id: int,
    session: Session,
) -> None:
    delete_groupchat_in_repository(groupchat_id, session)


def get_messages_for_groupchat(
    groupchat_id: int,
    session: Session,
) -> list:
    get_messages = get_messages_for_groupchat_in_repository(groupchat_id, session)
    return get_messages
