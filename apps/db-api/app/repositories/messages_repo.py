from sqlmodel import Session, SQLModel, select
from app.models import GroupChats, GroupChatMembers, Messages


def add_message_to_groupchat(
    groupchat_id: int, user_id: str, content: str, session: Session
) -> Messages:
    groupchat = session.get(GroupChats, groupchat_id)
    if not groupchat:
        raise ValueError(f"Groupchat with ID {groupchat_id} not found")

    user_is_member = session.exec(
        select(GroupChatMembers).where(
            GroupChatMembers.user_id == user_id,
            GroupChatMembers.group_chat_id == groupchat_id,
        )
    ).first()
    if not user_is_member:
        raise ValueError(
            f"User with ID {user_id} is not a member of groupchat with ID {groupchat_id}"
        )

    new_message = Messages(user_id=user_id, group_chat_id=groupchat_id, message=content)
    session.add(new_message)
    session.commit()
    session.refresh(new_message)

    return new_message


def delete_message_from_groupchat(
    groupchat_id: int, message_id: int, user_id: str, session: Session
) -> None:
    message = session.get(Messages, message_id)
    if not message:
        raise ValueError(f"Message with ID {message_id} not found")
    if message.group_chat_id != groupchat_id:
        raise ValueError(f"Message with ID {message_id} is not in this groupchat")
    if message.user_id != user_id:
        raise PermissionError("Only the message author can delete this message")

    session.delete(message)
    session.commit()


def update_message_in_groupchat(
    groupchat_id: int, message_id: int, new_content: str, user_id: str, session: Session
) -> Messages:
    message = session.exec(
        select(Messages).where(
            Messages.id == message_id,
            Messages.group_chat_id == groupchat_id,
        )
    ).first()
    if not message:
        raise ValueError(f"Message with ID {message_id} not found")

    if not new_content:
        raise ValueError("Message content cannot be empty")

    if message.user_id != user_id:
        raise PermissionError("Only the message author can update this message")

    message.message = new_content
    session.add(message)
    session.commit()
    session.refresh(message)

    return message
