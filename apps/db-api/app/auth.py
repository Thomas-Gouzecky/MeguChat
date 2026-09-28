from typing import Annotated

from fastapi import Header
from app.errors import UnauthenticateError


def get_current_user(
    current_user_id: Annotated[str | None, Header(alias="X-User-ID")] = None,
) -> str:
    if not current_user_id:
        raise UnauthenticateError(detail="Missing current user")

    return current_user_id
