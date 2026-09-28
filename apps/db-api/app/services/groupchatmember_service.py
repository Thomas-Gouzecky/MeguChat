from sqlmodel import Session
from app.models import GroupChatMembers
from app.DTOs import GroupChatMemberDto


def add_members_to_groupchat(
    groupchat_id: int,
    users: str | list[str],
    session: Session,
    current_user: str,
) -> list[GroupChatMemberDto]:
    from app.repositories.groupchatmembers_repo import (
        add_member_to_groupchat as add_member_to_groupchat_in_repository,
    )

    if isinstance(users, str):
        users = [users]

    added_users: list[GroupChatMemberDto] = []
    for user_id in users:
        new_user = add_member_to_groupchat_in_repository(
            groupchat_id, user_id, session, current_user
        )
        if new_user:
            added_users.append(
                GroupChatMemberDto(
                    id=new_user.id,
                    user_id=new_user.user_id,
                    group_chat_id=new_user.group_chat_id,
                    joined_at=new_user.joined_at,
                    last_active_at=new_user.last_active_at,
                    last_read_message_id=new_user.last_read_message_id,
                )
            )

    return added_users


def remove_member_from_groupchat(
    groupchat_id: int,
    user_id: str,
    session: Session,
    current_user: str,
) -> GroupChatMemberDto:
    from app.repositories.groupchatmembers_repo import (
        remove_member_from_groupchat as remove_member_from_groupchat_in_repository,
    )

    deleted_member: GroupChatMembers = remove_member_from_groupchat_in_repository(
        groupchat_id, user_id, session, current_user
    )
    return GroupChatMemberDto(
        id=deleted_member.id,
        user_id=deleted_member.user_id,
        group_chat_id=deleted_member.group_chat_id,
        joined_at=deleted_member.joined_at,
        last_active_at=deleted_member.last_active_at,
        last_read_message_id=deleted_member.last_read_message_id,
    )


def get_members_of_groupchat(
    groupchat_id: int,
    session: Session,
    current_user: str,
) -> list[GroupChatMemberDto]:
    from app.repositories.groupchatmembers_repo import (
        get_members_of_groupchat as get_members_of_groupchat_in_repository,
    )

    members: list[GroupChatMembers] = get_members_of_groupchat_in_repository(
        groupchat_id, session, current_user
    )

    return [
        GroupChatMemberDto(
            id=member.id,
            user_id=member.user_id,
            group_chat_id=member.group_chat_id,
            joined_at=member.joined_at,
            last_active_at=member.last_active_at,
            last_read_message_id=member.last_read_message_id,
        )
        for member in members
    ]
