from typing import Annotated

from fastapi import Header
from app.configs.settings import ENVIRONMENT
from app.errors import UnauthenticateError


def get_current_user(
    current_user_id: Annotated[str | None, Header(alias="X-User-ID")] = None,
) -> str:
    if not current_user_id:
        if ENVIRONMENT != "development":
            raise UnauthenticateError(detail="Missing current user")

        current_user_id = "user1"  # Default user ID for testing purposes

    return current_user_id
