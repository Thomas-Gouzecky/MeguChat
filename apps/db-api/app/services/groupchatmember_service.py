from sqlmodel import Session


def add_member_to_groupchat(
    groupchat_id: int,
    user_id: str,
    session: Session,
) -> None:
    from app.repositories.groupchatmembers_repo import (
        add_member_to_groupchat as add_member_to_groupchat_in_repository,
    )

    add_member_to_groupchat_in_repository(groupchat_id, user_id, session)


def remove_member_from_groupchat(
    groupchat_id: int,
    user_id: str,
    session: Session,
) -> None:
    from app.repositories.groupchatmembers_repo import (
        remove_member_from_groupchat as remove_member_from_groupchat_in_repository,
    )

    remove_member_from_groupchat_in_repository(groupchat_id, user_id, session)
