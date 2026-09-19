from fastapi.testclient import TestClient

from app.main import app

client = TestClient(app)


def test_get_groupchat_for_user():
    user_id = "user1"
    response = client.get(f"/api/groupchats/user/{user_id}")

    assert response.status_code == 200
    response_body = response.json()

    assert isinstance(response_body, list)
    for groupchat in response_body:
        assert "groupchat_id" in groupchat
        assert "name" in groupchat
        assert "created_at" in groupchat


def test_get_groupchat_for_user_no_groupchats():
    user_id = "nonexistent_user"
    response = client.get(f"/api/groupchats/user/{user_id}")

    assert response.status_code == 200
    response_body = response.json()

    assert isinstance(response_body, list)
    assert len(response_body) == 0


def test_groupchats_dont_show_to_users_not_in_them():
    # Create a groupchat for user1
    user1_request_body = {"name": "User 1 Group Chat"}
    client.post("/api/groupchats", json=user1_request_body)

    # Get groupchats for user2 (who is not in the groupchat)
    user2_response = client.get("/api/groupchats/user/user2")
    assert user2_response.status_code == 200
    user2_groupchats = user2_response.json()

    # Ensure that user2 does not see the groupchat created for user1
    assert all(gc["name"] != "User 1 Group Chat" for gc in user2_groupchats)


def test_add_member_to_groupchat():
    # Create a groupchat for user1
    user1_request_body = {"name": "User 1 Group Chat"}
    create_response = client.post("/api/groupchats", json=user1_request_body)
    assert create_response.status_code == 200
    groupchat_id = create_response.json()["groupchat_id"]

    # Add user2 to the groupchat
    add_member_request_body = {"user_id": "user2"}
    add_member_response = client.post(
        f"/api/groupchats/{groupchat_id}/members", json=add_member_request_body
    )
    assert add_member_response.status_code == 200

    # Verify that user2 can now see the groupchat
    user2_response = client.get("/api/groupchats/user/user2")
    assert user2_response.status_code == 200
    user2_groupchats = user2_response.json()
    assert any(gc["groupchat_id"] == groupchat_id for gc in user2_groupchats)


def test_remove_member_from_groupchat():
    # Create a groupchat for user1
    user1_request_body = {"name": "User 1 Group Chat"}
    create_response = client.post("/api/groupchats", json=user1_request_body)
    assert create_response.status_code == 200
    groupchat_id = create_response.json()["groupchat_id"]

    # Add user2 to the groupchat
    add_member_request_body = {"user_id": "user2"}
    add_member_response = client.post(
        f"/api/groupchats/{groupchat_id}/members", json=add_member_request_body
    )
    assert add_member_response.status_code == 200

    # Remove user2 from the groupchat
    remove_member_response = client.delete(
        f"/api/groupchats/{groupchat_id}/members/{add_member_request_body['user_id']}"
    )
    assert remove_member_response.status_code == 200

    # Verify that user2 can no longer see the groupchat
    user2_response = client.get("/api/groupchats/user/user2")
    assert user2_response.status_code == 200
    user2_groupchats = user2_response.json()
    assert all(gc["groupchat_id"] != groupchat_id for gc in user2_groupchats)
