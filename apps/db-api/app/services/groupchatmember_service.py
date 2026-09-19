from sqlmodel import Session

from app.models import GroupChats
from app.repositories.groupchat_repo import (
    create_a_new_groupchat_entry,
    find_groupchats_for_user as find_groupchats_for_user_in_repository,
    update_groupchat as update_groupchat_in_repository,
    delete_groupchat as delete_groupchat_in_repository,
    get_messages_for_groupchat as get_messages_for_groupchat_in_repository,
)
from app.DTOs import (
    GroupChatCreationRequest,
    GroupChatCreationResponse,
    GroupChatUpdateResponse,
)


def add_member_to_groupchat(
    groupchat_id: int,
    user_id: str,
    session: Session,
) -> None:
    from app.repositories.groupchatmembers_repo import (
        add_member_to_groupchat as add_member_to_groupchat_in_repository,
    )

    add_member_to_groupchat_in_repository(groupchat_id, user_id, session)
