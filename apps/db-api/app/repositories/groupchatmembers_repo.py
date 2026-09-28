from sqlmodel import Session, select
from sqlalchemy.exc import IntegrityError
from app.models import GroupChats, GroupChatMembers
from app.utils.user_permissions import validate_user_is_member_of_groupchat
from app.errors import ConflictError


def add_member_to_groupchat(
    groupchat_id: int, user_id: str, session: Session, current_user: str
) -> GroupChatMembers | None:
    groupchat = session.get(GroupChats, groupchat_id)
    if not groupchat:
        raise LookupError(f"Groupchat with ID {groupchat_id} not found")

    validate_user_is_member_of_groupchat(current_user, groupchat_id, session)

    existing_member = session.exec(
        select(GroupChatMembers).where(
            GroupChatMembers.group_chat_id == groupchat_id,
            GroupChatMembers.user_id == user_id,
        )
    ).first()
    if existing_member:
        return None  # User is already a member of the groupchat

    new_member = GroupChatMembers(group_chat_id=groupchat_id, user_id=user_id)
    session.add(new_member)
    try:
        session.commit()
    except IntegrityError as error:
        session.rollback()
        raise ConflictError(
            f"User with ID {user_id} is already a member of groupchat {groupchat_id}"
        ) from error

    return new_member


def remove_member_from_groupchat(
    groupchat_id: int, user_id: str, session: Session, current_user: str
) -> GroupChatMembers:
    groupchat = session.get(GroupChats, groupchat_id)
    if not groupchat:
        raise LookupError(f"Groupchat with ID {groupchat_id} not found")

    validate_user_is_member_of_groupchat(current_user, groupchat_id, session)

    member_to_remove = session.exec(
        select(GroupChatMembers).where(
            GroupChatMembers.group_chat_id == groupchat_id,
            GroupChatMembers.user_id == user_id,
        )
    ).first()

    if not member_to_remove:
        raise LookupError(
            f"User with ID {user_id} is not a member of groupchat {groupchat_id}"
        )

    session.delete(member_to_remove)
    session.commit()

    return member_to_remove


def get_members_of_groupchat(
    groupchat_id: int, session: Session, current_user: str
) -> list[GroupChatMembers]:
    groupchat = session.get(GroupChats, groupchat_id)
    if not groupchat:
        raise LookupError(f"Groupchat with ID {groupchat_id} not found")

    validate_user_is_member_of_groupchat(current_user, groupchat_id, session)

    members = session.exec(
        select(GroupChatMembers).where(GroupChatMembers.group_chat_id == groupchat_id)
    ).all()

    return list(members)
