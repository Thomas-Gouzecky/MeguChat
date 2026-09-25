from httpx import request
from sqlmodel import Session, SQLModel, select
from app.models import GroupChats, GroupChatMembers, Messages
from app.utils.user_permissions import validate_user_is_member_of_groupchat
from app.utils.helper import add_users_to_groupchat


def create_a_new_groupchat_entry(
    request_body: GroupChats,
    session: Session,
    current_user: str,
) -> GroupChats:

    groupchat = GroupChats(name=request.name)
    session.add(groupchat)
    session.flush()

    if groupchat.id is None or not isinstance(groupchat.id, int):
        raise ValueError("Groupchat ID is not an integer")

    added_users = add_users_to_groupchat(
        groupchat_id=groupchat.id,
        users=request.users,
        session=session,
        current_user=current_user,
    )

    session.commit()
    session.refresh(groupchat)

    return groupchat


def find_groupchats_for_user(user_id: str, session: Session) -> list[GroupChats]:
    group_chats_table = SQLModel.metadata.tables[GroupChats.__tablename__]
    group_chat_members_table = SQLModel.metadata.tables[GroupChatMembers.__tablename__]
    statement = (
        select(GroupChats)
        .join(
            GroupChatMembers,
            group_chats_table.c.id == group_chat_members_table.c.group_chat_id,
        )
        .where(group_chat_members_table.c.user_id == user_id)
    )
    groupchats = list(session.exec(statement).all())

    return groupchats


def update_groupchat(
    groupchat_id: int, request_body: GroupChats, session: Session, current_user: str
) -> GroupChats:
    groupchat = session.get(GroupChats, groupchat_id)
    if not groupchat:
        raise ValueError(f"Groupchat with ID {groupchat_id} not found")

    validate_user_is_member_of_groupchat(current_user, groupchat_id, session)

    groupchat.name = request_body.name
    session.add(groupchat)
    session.commit()
    session.refresh(groupchat)

    return groupchat


def delete_groupchat(groupchat_id: int, session: Session, current_user: str) -> None:
    groupchat = session.get(GroupChats, groupchat_id)
    if not groupchat:
        raise ValueError(f"Groupchat with ID {groupchat_id} not found")

    validate_user_is_member_of_groupchat(current_user, groupchat_id, session)

    session.delete(groupchat)
    session.commit()
