from typing import Annotated

from fastapi import Header, HTTPException
from app.configs.settings import ENVIRONMENT


def get_current_user(
    current_user_id: Annotated[str | None, Header(alias="X-User-ID")] = None,
) -> str:
    if not current_user_id:
        if ENVIRONMENT != "development":
            raise HTTPException(status_code=401, detail="Missing current user")

        current_user_id = "user1"  # Default user ID for testing purposes

    return current_user_id
