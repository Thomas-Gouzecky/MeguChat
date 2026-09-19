from sqlmodel import Session

from app.models import Messages
from app.repositories.groupchat_repo import (
    get_messages_for_groupchat as get_messages_for_groupchat_in_repository,
)
from app.DTOs import (
    MessageCreationRequest,
)


def get_messages_for_groupchat(
    groupchat_id: int,
    session: Session,
) -> list:
    get_messages = get_messages_for_groupchat_in_repository(groupchat_id, session)
    return get_messages


def create_message_for_groupchat(
    groupchat_id: int,
    request_body: MessageCreationRequest,
    session: Session,
) -> Messages:
    from app.repositories.messages_repo import (
        add_message_to_groupchat as add_message_to_groupchat_in_repository,
    )

    if not request_body.content:
        raise ValueError("Message content cannot be empty")

    return add_message_to_groupchat_in_repository(
        groupchat_id,
        request_body.user_id,
        request_body.content,
        session,
    )


def delete_message_from_groupchat(
    groupchat_id: int,
    message_id: int,
    user_id: str,
    session: Session,
) -> None:
    from app.repositories.messages_repo import (
        delete_message_from_groupchat as delete_message_from_groupchat_in_repository,
    )

    delete_message_from_groupchat_in_repository(
        groupchat_id, message_id, user_id, session
    )


def update_message_in_groupchat(
    message_id: int,
    new_content: MessageCreationRequest,
    session: Session,
) -> Messages:
    from app.repositories.messages_repo import (
        update_message_in_groupchat as update_message_in_groupchat_in_repository,
    )

    if not new_content.content:
        raise ValueError("New content cannot be empty")
    if not new_content.user_id:
        raise ValueError("User ID cannot be empty")

    return update_message_in_groupchat_in_repository(
        message_id, new_content.content, new_content.user_id, session
    )
