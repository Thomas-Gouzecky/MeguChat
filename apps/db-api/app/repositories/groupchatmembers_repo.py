from sqlmodel import Session, SQLModel, select
from sqlalchemy.exc import IntegrityError
from app.models import GroupChats, GroupChatMembers, Messages
from app.utils.user_permissions import validate_user_is_member_of_groupchat
from app.DTOs import GroupChatMembersResponse


def add_member_to_groupchat(
    groupchat_id: int, user_id: str, session: Session, current_user: str
) -> GroupChatMembersResponse | None:
    groupchat = session.get(GroupChats, groupchat_id)
    if not groupchat:
        raise ValueError(f"Groupchat with ID {groupchat_id} not found")

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
        raise ValueError(
            f"User with ID {user_id} is already a member of groupchat {groupchat_id}"
        ) from error

    return GroupChatMembersResponse(
        id=new_member.id,
        user_id=new_member.user_id,
        group_chat_id=new_member.group_chat_id,
        joined_at=new_member.joined_at,
        last_active_at=new_member.last_active_at,
        last_read_message_id=new_member.last_read_message_id,
    )  # User successfully added to the groupchat


def remove_member_from_groupchat(
    groupchat_id: int, user_id: str, session: Session, current_user: str
) -> GroupChatMembersResponse:
    groupchat = session.get(GroupChats, groupchat_id)
    if not groupchat:
        raise ValueError(f"Groupchat with ID {groupchat_id} not found")

    validate_user_is_member_of_groupchat(current_user, groupchat_id, session)

    member_to_remove = session.exec(
        select(GroupChatMembers).where(
            GroupChatMembers.group_chat_id == groupchat_id,
            GroupChatMembers.user_id == user_id,
        )
    ).first()

    if not member_to_remove:
        raise ValueError(
            f"User with ID {user_id} is not a member of groupchat {groupchat_id}"
        )

    session.delete(member_to_remove)
    session.commit()

    return GroupChatMembersResponse(
        id=member_to_remove.id,
        user_id=member_to_remove.user_id,
        group_chat_id=member_to_remove.group_chat_id,
        joined_at=member_to_remove.joined_at,
        last_active_at=member_to_remove.last_active_at,
        last_read_message_id=member_to_remove.last_read_message_id,
    )


def get_members_of_groupchat(
    groupchat_id: int, session: Session, current_user: str
) -> list[GroupChatMembers]:
    groupchat = session.get(GroupChats, groupchat_id)
    if not groupchat:
        raise ValueError(f"Groupchat with ID {groupchat_id} not found")

    validate_user_is_member_of_groupchat(current_user, groupchat_id, session)

    members = session.exec(
        select(GroupChatMembers).where(GroupChatMembers.group_chat_id == groupchat_id)
    ).all()

    return list(members)
