from sqlmodel import Session, select
from app.models import GroupChatMembers


def is_user_member_of_groupchat(
    user_id: str, groupchat_id: int, session: Session
) -> bool:
    """
    Checks if the user is a member of the specified group chat.

    Args:
        user_id (str): The ID of the user.
        groupchat_id (int): The ID of the group chat.
        session (Session): The database session.

    Returns:
        bool: True if the user is a member of the group chat, False otherwise.
    """
    user_is_member = session.exec(
        select(GroupChatMembers).where(
            GroupChatMembers.user_id == user_id,
            GroupChatMembers.group_chat_id == groupchat_id,
        )
    ).first()
    return user_is_member is not None


def validate_user_is_member_of_groupchat(
    user_id: str, groupchat_id: int, session: Session
) -> None:
    """
    Validates if the user is a member of the specified group chat.

    Args:
        user_id (str): The ID of the user.
        groupchat_id (int): The ID of the group chat.
        session (Session): The database session.

    Raises:
        PermissionError: If the user is not a member of the group chat.
    """
    if not is_user_member_of_groupchat(user_id, groupchat_id, session):
        raise PermissionError(
            f"User {user_id} is not a member of group chat {groupchat_id}."
        )
