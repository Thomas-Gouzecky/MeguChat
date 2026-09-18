from sqlmodel import Session
from app.models import GroupChats


def create_a_new_groupchat_entry(
    request_body: GroupChats, session: Session
) -> GroupChats:

    session.add(request_body)
    session.commit()
    session.refresh(request_body)

    return request_body
