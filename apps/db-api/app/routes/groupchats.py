from fastapi import APIRouter, Depends, HTTPException
from app.services import groupchat_service, groupchatmember_service
from app.sql import SessionDep
from app.DTOs import (
    GroupChatCreationRequest,
    GroupChatCreationResponse,
    GroupChatUpdateResponse,
    AddMembersRequest,
    GroupChatMembersResponse,
)
from app.auth import get_current_user

router = APIRouter(prefix="/api/groupchats", tags=["groupchats"])


@router.post("", response_model=GroupChatCreationResponse)
def create_new_groupchat(
    request_body: GroupChatCreationRequest, session: SessionDep
) -> GroupChatCreationResponse:
    return groupchat_service.create_groupchat(request_body, session)


@router.put("/{groupchat_id}", response_model=GroupChatUpdateResponse)
def update_groupchat(
    groupchat_id: int, request_body: GroupChatCreationRequest, session: SessionDep
) -> GroupChatUpdateResponse:
    return groupchat_service.update_groupchat(groupchat_id, request_body, session)


@router.delete("/{groupchat_id}", response_model=dict)
def delete_groupchat(
    groupchat_id: int,
    session: SessionDep,
    current_user: str = Depends(get_current_user),
) -> dict:
    try:
        groupchat_service.delete_groupchat(groupchat_id, session, current_user)
    except PermissionError as error:
        raise HTTPException(status_code=403, detail=str(error)) from error
    except ValueError as error:
        raise HTTPException(status_code=422, detail=str(error)) from error

    return {"message": f"Groupchat with ID {groupchat_id} has been deleted."}


# Get groupchats for a specific user


@router.get("/user/{user_id}", response_model=list[GroupChatCreationResponse])
def get_groupchats_for_user(
    user_id: str, session: SessionDep
) -> list[GroupChatCreationResponse]:
    return groupchat_service.find_groupchats_for_user(user_id, session)


# Members management endpoints


@router.get("/{groupchat_id}/members", response_model=list[GroupChatMembersResponse])
def get_members_user_id_of_groupchat(
    groupchat_id: int, session: SessionDep
) -> list[GroupChatMembersResponse]:
    try:
        members = groupchatmember_service.get_members_of_groupchat(
            groupchat_id, session
        )
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


@router.post("/{groupchat_id}/members", response_model=dict)
def add_members_to_groupchat(
    groupchat_id: int,
    request_body: AddMembersRequest,
    session: SessionDep,
    current_user: str = Depends(get_current_user),
) -> dict:
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

    return {
        "message": f"Users with IDs {', '.join(added_users)} have been added to groupchat {groupchat_id}."
    }


@router.delete("/{groupchat_id}/members/{user_id}", response_model=dict)
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
