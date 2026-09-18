from fastapi import APIRouter

router = APIRouter(prefix="/api/groupchats", tags=["groupchats"])

@router.post("")
def create_new_groupchat() -> :