from fastapi import APIRouter, Depends, HTTPException
from app.services import groupchat_service
from app.sql import SessionDep
from app.DTOs import (
    GroupChatCreationRequest,
    GroupChatCreationResponse,
    GroupChatUpdateResponse,
)
from app.auth import get_current_user

router = APIRouter(prefix="/api/groupchats", tags=["groupchats"])


@router.post("", response_model=GroupChatCreationResponse)
def create_new_groupchat(
    request_body: GroupChatCreationRequest,
    session: SessionDep,
    current_user: str = Depends(get_current_user),
) -> GroupChatCreationResponse:
    return groupchat_service.create_groupchat(request_body, session, current_user)


@router.put("/{groupchat_id}", response_model=GroupChatUpdateResponse)
def update_groupchat(
    groupchat_id: int,
    request_body: GroupChatCreationRequest,
    session: SessionDep,
    current_user: str = Depends(get_current_user),
) -> GroupChatUpdateResponse:
    return groupchat_service.update_groupchat(
        groupchat_id, request_body, session, current_user
    )


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
