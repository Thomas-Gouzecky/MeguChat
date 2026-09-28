from sqlmodel import Session

from app.models import Messages
from app.DTOs import (
    MessageCreationRequest,
    MessageDto,
)


def get_messages_for_groupchat(
    groupchat_id: int,
    session: Session,
    current_user: str,
) -> list[MessageDto]:
    from app.repositories.messages_repo import (
        get_messages_for_groupchat as get_messages_for_groupchat_in_repository,
    )

    get_messages: list[Messages] = get_messages_for_groupchat_in_repository(
        groupchat_id, session, current_user
    )

    return [
        MessageDto(
            id=message.id,
            user_id=message.user_id,
            group_chat_id=message.group_chat_id,
            content=message.content,
            created_at=message.created_at,
            modified_at=message.modified_at,
        )
        for message in get_messages
    ]


def create_message_for_groupchat(
    groupchat_id: int,
    request_body: MessageCreationRequest,
    user_id: str,
    session: Session,
) -> MessageDto:
    from app.repositories.messages_repo import (
        add_message_to_groupchat as add_message_to_groupchat_in_repository,
    )

    if not request_body.content:
        raise ValueError("Message content cannot be empty")

    created_message: Messages = add_message_to_groupchat_in_repository(
        groupchat_id,
        user_id,
        request_body.content,
        session,
    )

    return MessageDto(
        id=created_message.id,
        user_id=created_message.user_id,
        group_chat_id=created_message.group_chat_id,
        content=created_message.content,
        created_at=created_message.created_at,
        modified_at=created_message.modified_at,
    )


def delete_message_from_groupchat(
    groupchat_id: int,
    message_id: int,
    user_id: str,
    session: Session,
) -> MessageDto:
    from app.repositories.messages_repo import (
        delete_message_from_groupchat as delete_message_from_groupchat_in_repository,
    )

    deleted_message: Messages = delete_message_from_groupchat_in_repository(
        groupchat_id, message_id, user_id, session
    )

    return MessageDto(
        id=deleted_message.id,
        user_id=deleted_message.user_id,
        group_chat_id=deleted_message.group_chat_id,
        content=deleted_message.content,
        created_at=deleted_message.created_at,
        modified_at=deleted_message.modified_at,
    )


def update_message_in_groupchat(
    groupchat_id: int,
    message_id: int,
    new_content: MessageCreationRequest,
    user_id: str,
    session: Session,
) -> MessageDto:
    from app.repositories.messages_repo import (
        update_message_in_groupchat as update_message_in_groupchat_in_repository,
    )

    if not new_content.content:
        raise ValueError("New content cannot be empty")
    
    updated_message: Messages = update_message_in_groupchat_in_repository(
        groupchat_id, message_id, new_content.content, user_id, session
    )

    return MessageDto(
        id=updated_message.id,
        user_id=updated_message.user_id,
        group_chat_id=updated_message.group_chat_id,
        content=updated_message.content,
        created_at=updated_message.created_at,
        modified_at=updated_message.modified_at,
    )
