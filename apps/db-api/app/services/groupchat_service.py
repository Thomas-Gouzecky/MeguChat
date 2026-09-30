from sqlmodel import Session

from app.models import GroupChats
from app.repositories.groupchat_repo import (
    create_a_new_groupchat_entry,
    get_groupchat_by_id as get_groupchat_by_id_from_repository,
    find_groupchats_for_user as find_groupchats_for_user_in_repository,
    update_groupchat as update_groupchat_in_repository,
    delete_groupchat as delete_groupchat_in_repository,
)
from app.DTOs import (
    GroupChatCreationRequest,
    GroupChatDto,
)


def get_groupchat_by_id(
    groupchat_id: int,
    session: Session,
    current_user: str,
) -> GroupChatDto:
    groupchat: GroupChats = get_groupchat_by_id_from_repository(
        groupchat_id, session, current_user
    )

    if groupchat.id is None:
        raise ValueError("Groupchat ID is missing")

    return GroupChatDto(
        groupchat_id=groupchat.id,
        name=groupchat.name,
        created_at=groupchat.created_at,
    )


def create_groupchat(
    request_body: GroupChatCreationRequest,
    session: Session,
    current_user: str,
) -> GroupChatDto:

    groupchat_request = GroupChats(
        name=request_body.name,
    )

    created_groupchat = create_a_new_groupchat_entry(
        groupchat_request,
        request_body.users,
        session,
        current_user,
    )

    if created_groupchat.id is None or not isinstance(created_groupchat.id, int):
        raise ValueError("Groupchat ID is not an integer")

    return GroupChatDto(
        groupchat_id=created_groupchat.id,
        name=created_groupchat.name,
        created_at=created_groupchat.created_at,
    )


def find_groupchats_for_user(
    user_id: str,
    session: Session,
) -> list[GroupChatDto]:

    users_groupchats = find_groupchats_for_user_in_repository(user_id, session)
    return [
        GroupChatDto(
            groupchat_id=groupchat.id,
            name=groupchat.name,
            created_at=groupchat.created_at,
        )
        for groupchat in users_groupchats
        if groupchat.id is not None
    ]


def update_groupchat(
    groupchat_id: int,
    request_body: GroupChatCreationRequest,
    session: Session,
    current_user: str,
) -> GroupChatDto:
    groupchat_request = GroupChats(name=request_body.name)
    updated_groupchat = update_groupchat_in_repository(
        groupchat_id,
        groupchat_request,
        session,
        current_user,
    )

    if updated_groupchat.id is None:
        raise ValueError("Groupchat ID is missing")

    return GroupChatDto(
        groupchat_id=updated_groupchat.id,
        name=updated_groupchat.name,
        created_at=updated_groupchat.created_at,
    )


def delete_groupchat(
    groupchat_id: int,
    session: Session,
    current_user: str,
) -> GroupChatDto:

    deleted_groupchat: GroupChats = delete_groupchat_in_repository(
        groupchat_id, session, current_user
    )

    if deleted_groupchat.id is None:
        raise ValueError("Groupchat ID is missing")

    return GroupChatDto(
        groupchat_id=deleted_groupchat.id,
        name=deleted_groupchat.name,
        created_at=deleted_groupchat.created_at,
    )
