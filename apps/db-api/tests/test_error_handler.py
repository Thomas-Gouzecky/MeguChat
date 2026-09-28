from fastapi.testclient import TestClient

from app.main import app

client = TestClient(app)


# test error handling for missing user ID in request
def test_no_user_id_in_request_returns_401():
    response = client.post(
        "/api/groupchats",
        json={"name": "Test Groupchat", "description": "A test groupchat"},
        headers={"Content-Type": "application/json"},
    )
    assert response.status_code == 401


# test permission error when user is not a member of the groupchat
def test_user_not_member_of_groupchat_returns_403():
    # First, create a groupchat with user1 as the creator
    response = client.post(
        "/api/groupchats",
        json={"name": "Test Groupchat", "description": "A test groupchat"},
        headers={"Content-Type": "application/json", "X-User-ID": "user1"},
    )

    # Now, try to add a message to the groupchat with a user who is not a member
    response = client.post(
        "/api/groupchats/1/messages",
        json={"user_id": "non_member_user", "content": "Hello"},
        headers={"Content-Type": "application/json", "X-User-ID": "non_member_user"},
    )
    assert response.status_code == 403
