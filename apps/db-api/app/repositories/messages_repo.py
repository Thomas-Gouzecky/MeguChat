from sqlmodel import Session, SQLModel, select
from app.models import GroupChats, GroupChatMembers, Messages
from app.utils.user_permissions import validate_user_is_member_of_groupchat


def get_messages_for_groupchat(
    groupchat_id: int, session: Session, current_user: str
) -> list[Messages]:
    group_chats_table = SQLModel.metadata.tables[GroupChats.__tablename__]
    messages_table = SQLModel.metadata.tables[Messages.__tablename__]

    if current_user is None:
        raise ValueError("Current user is not authenticated")

    if not session.get(GroupChats, groupchat_id):
        raise ValueError(f"Groupchat with ID {groupchat_id} not found")
    validate_user_is_member_of_groupchat(current_user, groupchat_id, session)
    statement = (
        select(Messages)
        .join(
            GroupChats,
            messages_table.c.group_chat_id == group_chats_table.c.id,
        )
        .where(group_chats_table.c.id == groupchat_id)
    )
    messages = list(session.exec(statement).all())

    return messages


def add_message_to_groupchat(
    groupchat_id: int, user_id: str, content: str, session: Session
) -> Messages:
    groupchat = session.get(GroupChats, groupchat_id)
    if not groupchat:
        raise ValueError(f"Groupchat with ID {groupchat_id} not found")

    validate_user_is_member_of_groupchat(user_id, groupchat_id, session)

    new_message = Messages(user_id=user_id, group_chat_id=groupchat_id, content=content)
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

    message.content = new_content
    session.add(message)
    session.commit()
    session.refresh(message)

    return message
