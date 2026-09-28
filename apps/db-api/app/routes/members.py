from fastapi import APIRouter, Depends
from app.services import groupchatmember_service
from app.sql import SessionDep
from app.DTOs import (
    AddMembersRequest,
    GroupChatMemberDto,
)
from app.auth import get_current_user

router = APIRouter(prefix="/api/groupchats/{groupchat_id}/members", tags=["members"])


@router.get("", response_model=list[GroupChatMemberDto])
def get_members_user_id_of_groupchat(
    groupchat_id: int,
    session: SessionDep,
    current_user: str = Depends(get_current_user),
) -> list[GroupChatMemberDto]:
    members: list[GroupChatMemberDto] = (
        groupchatmember_service.get_members_of_groupchat(
            groupchat_id, session, current_user
        )
    )

    return members


@router.post("", response_model=list[GroupChatMemberDto])
def add_members_to_groupchat(
    groupchat_id: int,
    request_body: AddMembersRequest,
    session: SessionDep,
    current_user: str = Depends(get_current_user),
) -> list[GroupChatMemberDto]:
    users = request_body.users

    if not users:
        raise ValueError("Missing 'users' in request body")

    if isinstance(users, str):
        users = [users]

    added_users: list[GroupChatMemberDto] = (
        groupchatmember_service.add_members_to_groupchat(
            groupchat_id, users, session, current_user
        )
    )

    return added_users


@router.delete("/{user_id}", response_model=GroupChatMemberDto)
def remove_member_from_groupchat(
    groupchat_id: int,
    user_id: str,
    session: SessionDep,
    current_user: str = Depends(get_current_user),
) -> GroupChatMemberDto:
    deleted_member: GroupChatMemberDto = (
        groupchatmember_service.remove_member_from_groupchat(
            groupchat_id, user_id, session, current_user
        )
    )

    return deleted_member
