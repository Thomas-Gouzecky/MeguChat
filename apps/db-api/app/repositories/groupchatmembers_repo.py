from sqlmodel import Session, SQLModel, select
from app.models import GroupChats, GroupChatMembers, Messages


def add_member_to_groupchat(groupchat_id: int, user_id: str, session: Session) -> None:
    groupchat = session.get(GroupChats, groupchat_id)
    if not groupchat:
        raise ValueError(f"Groupchat with ID {groupchat_id} not found")

    new_member = GroupChatMembers(group_chat_id=groupchat_id, user_id=user_id)
    session.add(new_member)
    session.commit()
