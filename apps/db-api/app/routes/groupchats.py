from fastapi import APIRouter
from app.services import groupchat_service
from app.sql import SessionDep
from app.DTOs import GroupChatCreationRequest, GroupChatCreationResponse

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
