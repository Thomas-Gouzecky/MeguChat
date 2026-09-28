from fastapi import APIRouter, Depends
from app.services import groupchat_service
from app.sql import SessionDep
from app.DTOs import (
    GroupChatCreationRequest,
    GroupChatDto,
)
from app.auth import get_current_user

router = APIRouter(prefix="/api/groupchats", tags=["groupchats"])


@router.post("", response_model=GroupChatDto)
def create_new_groupchat(
    request_body: GroupChatCreationRequest,
    session: SessionDep,
    current_user: str = Depends(get_current_user),
) -> GroupChatDto:

    created_groupchat: GroupChatDto = groupchat_service.create_groupchat(
        request_body, session, current_user
    )

    return created_groupchat


@router.put("/{groupchat_id}", response_model=GroupChatDto)
def update_groupchat(
    groupchat_id: int,
    request_body: GroupChatCreationRequest,
    session: SessionDep,
    current_user: str = Depends(get_current_user),
) -> GroupChatDto:

    updated_groupchat: GroupChatDto = groupchat_service.update_groupchat(
        groupchat_id, request_body, session, current_user
    )
    return updated_groupchat


@router.delete("/{groupchat_id}", response_model=GroupChatDto)
def delete_groupchat(
    groupchat_id: int,
    session: SessionDep,
    current_user: str = Depends(get_current_user),
) -> GroupChatDto:
    deleted_groupchat: GroupChatDto = groupchat_service.delete_groupchat(
        groupchat_id, session, current_user
    )

    return deleted_groupchat


# Get groupchats for a specific user


@router.get("/user/{user_id}", response_model=list[GroupChatDto])
def get_groupchats_for_user(user_id: str, session: SessionDep) -> list[GroupChatDto]:

    list_of_groupchats: list[GroupChatDto] = groupchat_service.find_groupchats_for_user(
        user_id, session
    )
    return list_of_groupchats
