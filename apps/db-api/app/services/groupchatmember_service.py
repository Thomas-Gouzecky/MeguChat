from sqlmodel import Session
from app.models import GroupChatMembers


def add_members_to_groupchat(
    groupchat_id: int,
    users: str | list[str],
    session: Session,
    curent_user: str,
) -> list[str]:
    from app.repositories.groupchatmembers_repo import (
        add_member_to_groupchat as add_member_to_groupchat_in_repository,
    )

    if isinstance(users, str):
        users = [users]

    added_users: list[str] = []
    for user_id in users:
        was_added = add_member_to_groupchat_in_repository(
            groupchat_id, user_id, session, curent_user
        )
        if was_added:
            added_users.append(user_id)

    return added_users


def remove_member_from_groupchat(
    groupchat_id: int,
    user_id: str,
    session: Session,
    current_user: str,
) -> None:
    from app.repositories.groupchatmembers_repo import (
        remove_member_from_groupchat as remove_member_from_groupchat_in_repository,
    )

    remove_member_from_groupchat_in_repository(
        groupchat_id, user_id, session, current_user
    )


def get_members_of_groupchat(
    groupchat_id: int,
    session: Session,
    current_user: str,
) -> list[GroupChatMembers]:
    from app.repositories.groupchatmembers_repo import (
        get_members_of_groupchat as get_members_of_groupchat_in_repository,
    )

    return get_members_of_groupchat_in_repository(groupchat_id, session, current_user)
