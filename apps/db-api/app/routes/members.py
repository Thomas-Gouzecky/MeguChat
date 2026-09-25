from fastapi import APIRouter, Depends, HTTPException
from app.services import groupchatmember_service
from app.sql import SessionDep
from app.DTOs import (
    AddMembersRequest,
    GroupChatMembersResponse,
)
from app.auth import get_current_user

router = APIRouter(prefix="/api/groupchats/{groupchat_id}/members", tags=["members"])


@router.get("", response_model=list[GroupChatMembersResponse])
def get_members_user_id_of_groupchat(
    groupchat_id: int,
    session: SessionDep,
    current_user: str = Depends(get_current_user),
) -> list[GroupChatMembersResponse]:
    try:
        members = groupchatmember_service.get_members_of_groupchat(
            groupchat_id, session, current_user
        )
    except PermissionError as error:
        raise HTTPException(status_code=403, detail=str(error)) from error
    except ValueError as error:
        raise HTTPException(status_code=422, detail=str(error)) from error

    return [
        GroupChatMembersResponse(
            id=member.id,
            user_id=member.user_id,
            group_chat_id=member.group_chat_id,
            joined_at=member.joined_at,
            last_active_at=member.last_active_at,
            last_read_message_id=member.last_read_message_id,
        )
        for member in members
    ]


@router.post("", response_model=dict)
def add_members_to_groupchat(
    groupchat_id: int,
    request_body: AddMembersRequest,
    session: SessionDep,
    current_user: str = Depends(get_current_user),
) -> list[str]:
    users = request_body.users
    if not users:
        raise HTTPException(status_code=422, detail="Missing 'users' in request body")
    if isinstance(users, str):
        users = [users]

    try:
        added_users = groupchatmember_service.add_members_to_groupchat(
            groupchat_id, users, session, current_user
        )
    except PermissionError as error:
        raise HTTPException(status_code=403, detail=str(error)) from error
    except ValueError as error:
        raise HTTPException(status_code=422, detail=str(error)) from error

    return added_users


@router.delete("/{user_id}", response_model=dict)
def remove_member_from_groupchat(
    groupchat_id: int,
    user_id: str,
    session: SessionDep,
    current_user: str = Depends(get_current_user),
) -> dict:
    try:
        groupchatmember_service.remove_member_from_groupchat(
            groupchat_id, user_id, session, current_user
        )
    except PermissionError as error:
        raise HTTPException(status_code=403, detail=str(error)) from error
    except ValueError as error:
        raise HTTPException(status_code=422, detail=str(error)) from error

    return {
        "message": f"User with ID {user_id} has been removed from groupchat {groupchat_id}."
    }
