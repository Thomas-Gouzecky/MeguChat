from sqlmodel import Session, select
from sqlalchemy.exc import IntegrityError
from app.models import GroupChats, GroupChatMembers


def add_users_to_groupchat(
    groupchat_id: int,
    users: str | list[str],
    session: Session,
    current_user: str,
) -> list[str]:
    """
    Add users to a group chat.

    Args:
        groupchat_id (int): The ID of the group chat.
        users (str | list[str]): A single user ID or a list of user IDs to add.
        session (Session): The database session.
        current_user (str): The ID of the current user performing the action.

    Returns:
        list[str]: A list of user IDs that were successfully added to the group chat.
    """
    if isinstance(users, str):
        users = [users]

    # Include the creator and remove duplicates.
    user_ids = list(dict.fromkeys([current_user, *users]))

    for user_id in user_ids:
        session.add(
            GroupChatMembers(
                group_chat_id=groupchat_id,
                user_id=user_id,
            )
        )

    session.flush()

    return user_ids
