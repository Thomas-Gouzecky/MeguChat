from sqlmodel import Session, SQLModel, select
from sqlalchemy.exc import IntegrityError
from app.models import GroupChats, GroupChatMembers, Messages


def add_member_to_groupchat(groupchat_id: int, user_id: str, session: Session) -> None:
    groupchat = session.get(GroupChats, groupchat_id)
    if not groupchat:
        raise ValueError(f"Groupchat with ID {groupchat_id} not found")

    existing_member = session.exec(
        select(GroupChatMembers).where(
            GroupChatMembers.group_chat_id == groupchat_id,
            GroupChatMembers.user_id == user_id,
        )
    ).first()
    if existing_member:
        raise ValueError(
            f"User with ID {user_id} is already a member of groupchat {groupchat_id}"
        )

    new_member = GroupChatMembers(group_chat_id=groupchat_id, user_id=user_id)
    session.add(new_member)
    try:
        session.commit()
    except IntegrityError as error:
        session.rollback()
        raise ValueError(
            f"User with ID {user_id} is already a member of groupchat {groupchat_id}"
        ) from error


def remove_member_from_groupchat(
    groupchat_id: int, user_id: str, session: Session
) -> None:
    groupchat = session.get(GroupChats, groupchat_id)
    if not groupchat:
        raise ValueError(f"Groupchat with ID {groupchat_id} not found")

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



def get_members_of_groupchat(groupchat_id: int, session: Session) -> list[GroupChatMembers]:
    groupchat = session.get(GroupChats, groupchat_id)
    if not groupchat:
        raise ValueError(f"Groupchat with ID {groupchat_id} not found")

    members = session.exec(
        select(GroupChatMembers).where(GroupChatMembers.group_chat_id == groupchat_id)
    ).all()

    return list(members)
