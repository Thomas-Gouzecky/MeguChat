from fastapi import APIRouter, HTTPException
from app.services import groupchat_service, groupchatmember_service
from app.sql import SessionDep
from app.DTOs import (
    GroupChatCreationRequest,
    GroupChatCreationResponse,
    GroupChatUpdateResponse,
    MessageCreationRequest,
)

router = APIRouter(prefix="/api/groupchats", tags=["groupchats"])


@router.post("", response_model=GroupChatCreationResponse)
def create_new_groupchat(
    request_body: GroupChatCreationRequest, session: SessionDep
) -> GroupChatCreationResponse:
    return groupchat_service.create_groupchat(request_body, session)


@router.get("/user/{user_id}", response_model=list[GroupChatCreationResponse])
def get_groupchats_for_user(
    user_id: str, session: SessionDep
) -> list[GroupChatCreationResponse]:
    return groupchat_service.find_groupchats_for_user(user_id, session)


@router.put("/{groupchat_id}", response_model=GroupChatUpdateResponse)
def update_groupchat(
    groupchat_id: int, request_body: GroupChatCreationRequest, session: SessionDep
) -> GroupChatUpdateResponse:
    return groupchat_service.update_groupchat(groupchat_id, request_body, session)


@router.delete("/{groupchat_id}", response_model=dict)
def delete_groupchat(groupchat_id: int, session: SessionDep) -> dict:
    try:
        groupchat_service.delete_groupchat(groupchat_id, session)
    except ValueError as error:
        raise HTTPException(status_code=422, detail=str(error)) from error

    return {"message": f"Groupchat with ID {groupchat_id} has been deleted."}


@router.get("/{groupchat_id}/messages", response_model=list)
def get_messages_for_groupchat(groupchat_id: int, session: SessionDep) -> list:
    messages = groupchat_service.get_messages_for_groupchat(groupchat_id, session)
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


@router.post("/{groupchat_id}/messages", response_model=dict)
def create_message_for_groupchat(
    groupchat_id: int,
    request_body: MessageCreationRequest,
    session: SessionDep,
) -> dict:
    try:
        message = groupchat_service.create_message_for_groupchat(
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


@router.post("/{groupchat_id}/messages", response_model=dict)
def add_message_to_groupchat(
    groupchat_id: int, request_body: dict, session: SessionDep
) -> dict:
    content = request_body.get("content")
    if not content:
        raise HTTPException(status_code=422, detail="Missing 'content' in request body")
    user_id = request_body.get("user_id")
    if not user_id:
        raise HTTPException(status_code=422, detail="Missing 'user_id' in request body")

    try:
        groupchat_service.add_message_to_groupchat(
            user_id, groupchat_id, content, session
        )
    except ValueError as error:
        raise HTTPException(status_code=422, detail=str(error)) from error

    return {"message": f"Message has been added to groupchat {groupchat_id}."}


@router.post("/{groupchat_id}/members", response_model=dict)
def add_member_to_groupchat(
    groupchat_id: int, request_body: dict, session: SessionDep
) -> dict:
    user_id = request_body.get("user_id")
    if not user_id:
        raise HTTPException(status_code=422, detail="Missing 'user_id' in request body")

    try:
        groupchatmember_service.add_member_to_groupchat(groupchat_id, user_id, session)
    except ValueError as error:
        raise HTTPException(status_code=422, detail=str(error)) from error

    return {
        "message": f"User with ID {user_id} has been added to groupchat {groupchat_id}."
    }


@router.delete("/{groupchat_id}/members/{user_id}", response_model=dict)
def remove_member_from_groupchat(
    groupchat_id: int, user_id: str, session: SessionDep
) -> dict:
    try:
        groupchatmember_service.remove_member_from_groupchat(
            groupchat_id, user_id, session
        )
    except ValueError as error:
        raise HTTPException(status_code=422, detail=str(error)) from error

    return {
        "message": f"User with ID {user_id} has been removed from groupchat {groupchat_id}."
    }
