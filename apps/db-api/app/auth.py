from typing import Annotated

from fastapi import Header, HTTPException


def get_current_user(
    user_id: Annotated[str | None, Header(alias="X-User-ID")] = None,
) -> str:
    if not user_id:
        raise HTTPException(status_code=401, detail="Missing current user")

    return user_id
