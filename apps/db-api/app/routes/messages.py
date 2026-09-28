from fastapi import APIRouter, Depends, status
from app.auth import get_current_user
from app.services import messages_service
from app.sql import SessionDep
from app.DTOs import MessageCreationRequest, MessageDto


from fastapi import APIRouter

router = APIRouter(prefix="/api/groupchats/{groupchat_id}/messages", tags=["messages"])


@router.get("", response_model=list[MessageDto])
def get_messages_for_groupchat(
    groupchat_id: int,
    session: SessionDep,
    current_user: str = Depends(get_current_user),
) -> list[MessageDto]:

    messages: list[MessageDto] = messages_service.get_messages_for_groupchat(
        groupchat_id, session, current_user
    )

    return messages


@router.post("", response_model=MessageDto, status_code=status.HTTP_201_CREATED)
def create_message_for_groupchat(
    groupchat_id: int,
    request_body: MessageCreationRequest,
    session: SessionDep,
    current_user: str = Depends(get_current_user),
) -> MessageDto:

    created_message: MessageDto = messages_service.create_message_for_groupchat(
        groupchat_id, request_body, current_user, session
    )

    return created_message


@router.delete("/{message_id}", response_model=MessageDto)
def delete_message_from_groupchat(
    groupchat_id: int,
    message_id: int,
    session: SessionDep,
    current_user: str = Depends(get_current_user),
) -> MessageDto:
    deleted_message: MessageDto = messages_service.delete_message_from_groupchat(
        groupchat_id, message_id, current_user, session
    )

    return deleted_message


@router.put("/{message_id}", response_model=MessageDto)
def update_message_in_groupchat(
    groupchat_id: int,
    message_id: int,
    request_body: MessageCreationRequest,
    session: SessionDep,
    current_user: str = Depends(get_current_user),
) -> MessageDto:
    updated_message: MessageDto = messages_service.update_message_in_groupchat(
        groupchat_id, message_id, request_body, current_user, session
    )

    return updated_message
