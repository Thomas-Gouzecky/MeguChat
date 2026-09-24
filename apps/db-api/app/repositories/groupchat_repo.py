from sqlmodel import Session, SQLModel, select
from app.models import GroupChats, GroupChatMembers, Messages


def create_a_new_groupchat_entry(
    request_body: GroupChats, session: Session
) -> GroupChats:

    session.add(request_body)
    session.commit()
    session.refresh(request_body)

    return request_body


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
    groupchat_id: int, request_body: GroupChats, session: Session
) -> GroupChats:
    groupchat = session.get(GroupChats, groupchat_id)
    if not groupchat:
        raise ValueError(f"Groupchat with ID {groupchat_id} not found")

    groupchat.name = request_body.name
    session.add(groupchat)
    session.commit()
    session.refresh(groupchat)

    return groupchat


def delete_groupchat(groupchat_id: int, session: Session) -> None:
    groupchat = session.get(GroupChats, groupchat_id)
    if not groupchat:
        raise ValueError(f"Groupchat with ID {groupchat_id} not found")

    session.delete(groupchat)
    session.commit()

