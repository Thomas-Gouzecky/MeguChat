from fastapi import APIRouter, HTTPException
from app.services import groupchat_service, groupchatmember_service
from app.sql import SessionDep
from app.DTOs import (
    GroupChatCreationRequest,
    GroupChatCreationResponse,
    GroupChatUpdateResponse,
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
    return groupchat_service.get_messages_for_groupchat(groupchat_id, session)


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
