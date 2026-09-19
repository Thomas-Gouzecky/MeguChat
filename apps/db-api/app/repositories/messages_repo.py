from sqlmodel import Session, SQLModel, select
from app.models import GroupChats, GroupChatMembers, Messages


def add_message_to_groupchat(
    user_id: str, groupchat_id: int, content: str, session: Session
) -> Messages:
    groupchat = session.get(GroupChats, groupchat_id)
    if not groupchat:
        raise ValueError(f"Groupchat with ID {groupchat_id} not found")

    new_message = Messages(user_id=user_id, group_chat_id=groupchat_id, message=content)
    session.add(new_message)
    session.commit()
    session.refresh(new_message)

    return new_message
