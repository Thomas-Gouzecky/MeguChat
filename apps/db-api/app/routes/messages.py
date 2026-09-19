from fastapi import APIRouter, HTTPException
from app.services import groupchat_service, groupchatmember_service, messages_service
from app.sql import SessionDep
from app.DTOs import (
    GroupChatCreationRequest,
    GroupChatCreationResponse,
    GroupChatUpdateResponse,
    MessageCreationRequest,
)


from fastapi import APIRouter

router = APIRouter(prefix="/api/groupchats/{groupchat_id}/messages", tags=["messages"])


@router.get("", response_model=list)
def get_messages_for_groupchat(groupchat_id: int, session: SessionDep) -> list:
    try:
        messages = messages_service.get_messages_for_groupchat(groupchat_id, session)
    except ValueError as error:
        raise HTTPException(status_code=422, detail=str(error)) from error
    except LookupError as error:
        raise HTTPException(status_code=404, detail=str(error)) from error
    except Exception as error:
        raise HTTPException(status_code=500, detail="Internal Server Error") from error
    return [
        {
            "id": message.id,
            "user_id": message.user_id,
            "group_chat_id": message.group_chat_id,
            "content": message.message,
            "created_at": message.created_at,
            "modified_at": message.modified_at,
        }
        for message in messages
    ]


@router.post("", response_model=dict)
def create_message_for_groupchat(
    groupchat_id: int,
    request_body: MessageCreationRequest,
    session: SessionDep,
) -> dict:
    try:
        message = messages_service.create_message_for_groupchat(
            groupchat_id, request_body, session
        )
    except ValueError as error:
        raise HTTPException(status_code=422, detail=str(error)) from error

    return {
        "id": message.id,
        "user_id": message.user_id,
        "group_chat_id": message.group_chat_id,
        "content": message.message,
        "created_at": message.created_at,
        "modified_at": message.modified_at,
    }


@router.delete("/{message_id}", response_model=dict)
def delete_message_from_groupchat(
    groupchat_id: int, message_id: int, session: SessionDep
) -> dict:
    try:
        messages_service.delete_message_from_groupchat(message_id, session)
    except ValueError as error:
        raise HTTPException(status_code=422, detail=str(error)) from error

    return {
        "message": f"Message with ID {message_id} has been deleted from groupchat {groupchat_id}."
    }


@router.put("/{message_id}", response_model=dict)
def update_message_in_groupchat(
    groupchat_id: int,
    message_id: int,
    request_body: MessageCreationRequest,
    session: SessionDep,
) -> dict:
    try:
        message = messages_service.update_message_in_groupchat(
            message_id, request_body, session
        )
    except ValueError as error:
        raise HTTPException(status_code=422, detail=str(error)) from error

    return {
        "id": message.id,
        "user_id": message.user_id,
        "group_chat_id": message.group_chat_id,
        "content": message.message,
        "created_at": message.created_at,
        "modified_at": message.modified_at,
    }
