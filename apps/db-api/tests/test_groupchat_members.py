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


def test_add_multiple_members_to_groupchat():
    # Create a groupchat for user1
    user1_request_body = {"name": "User 1 Group Chat"}
    create_response = client.post("/api/groupchats", json=user1_request_body)
    assert create_response.status_code == 200
    groupchat_id = create_response.json()["groupchat_id"]

    # Add multiple members to the groupchat
    add_members_request_body = {"users": ["user2", "user3"]}
    add_members_response = client.post(
        f"/api/groupchats/{groupchat_id}/members", json=add_members_request_body
    )
    assert add_members_response.status_code == 200

    # Verify that user2 and user3 can now see the groupchat
    for user_id in ["user2", "user3"]:
        user_response = client.get(f"/api/groupchats/user/{user_id}")
        assert user_response.status_code == 200
        user_groupchats = user_response.json()
        assert any(gc["groupchat_id"] == groupchat_id for gc in user_groupchats)


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
    add_member_request_body = {"users": "user2"}
    add_member_response = client.post(
        f"/api/groupchats/{groupchat_id}/members", json=add_member_request_body
    )
    assert add_member_response.status_code == 200

    # Verify that user2 can now see the groupchat
    user2_response = client.get("/api/groupchats/user/user2")
    assert user2_response.status_code == 200
    user2_groupchats = user2_response.json()
    assert any(gc["groupchat_id"] == groupchat_id for gc in user2_groupchats)


def test_cannot_add_same_member_to_groupchat_twice():
    create_response = client.post(
        "/api/groupchats", json={"name": "Duplicate Member Test"}
    )
    groupchat_id = create_response.json()["groupchat_id"]
    member_request = {"users": "user2"}

    first_response = client.post(
        f"/api/groupchats/{groupchat_id}/members", json=member_request
    )
    duplicate_response = client.post(
        f"/api/groupchats/{groupchat_id}/members", json=member_request
    )

    assert first_response.status_code == 200
    assert duplicate_response.status_code == 422
    assert "already a member" in duplicate_response.json()["detail"]


def test_remove_member_from_groupchat():
    # Create a groupchat for user1
    user1_request_body = {"name": "User 1 Group Chat"}
    create_response = client.post("/api/groupchats", json=user1_request_body)
    assert create_response.status_code == 200
    groupchat_id = create_response.json()["groupchat_id"]

    # Add user2 to the groupchat
    add_member_request_body = {"users": "user2"}
    add_member_response = client.post(
        f"/api/groupchats/{groupchat_id}/members", json=add_member_request_body
    )
    assert add_member_response.status_code == 200

    # Remove user2 from the groupchat
    remove_member_response = client.delete(
        f"/api/groupchats/{groupchat_id}/members/{add_member_request_body['users']}"
    )
    assert remove_member_response.status_code == 200

    # Verify that user2 can no longer see the groupchat
    user2_response = client.get("/api/groupchats/user/user2")
    assert user2_response.status_code == 200
    user2_groupchats = user2_response.json()
    assert all(gc["groupchat_id"] != groupchat_id for gc in user2_groupchats)


def test_get_members_of_groupchat():
    # Create a groupchat for user1
    user1_request_body = {"name": "User 1 Group Chat"}
    create_response = client.post("/api/groupchats", json=user1_request_body)
    assert create_response.status_code == 200
    groupchat_id = create_response.json()["groupchat_id"]

    # Add user2 and user3 to the groupchat
    add_members_request_body = {"users": ["user2", "user3"]}
    add_members_response = client.post(
        f"/api/groupchats/{groupchat_id}/members", json=add_members_request_body
    )
    assert add_members_response.status_code == 200

    # Get members of the groupchat
    get_members_response = client.get(f"/api/groupchats/{groupchat_id}/members")
    assert get_members_response.status_code == 200
    members = get_members_response.json()

    # Verify that user2, and user3 are in the members list
    member_ids = [member["user_id"] for member in members]
    assert "user2" in member_ids
    assert "user3" in member_ids


def test_members_from_different_groupchats_are_separated():
    # Create two groupchats
    groupchat1_response = client.post("/api/groupchats", json={"name": "Group Chat 1"})
    groupchat2_response = client.post("/api/groupchats", json={"name": "Group Chat 2"})
    groupchat1_id = groupchat1_response.json()["groupchat_id"]
    groupchat2_id = groupchat2_response.json()["groupchat_id"]

    # Add user2 to the first groupchat and user3 to the second
    client.post(f"/api/groupchats/{groupchat1_id}/members", json={"users": "user2"})
    client.post(f"/api/groupchats/{groupchat2_id}/members", json={"users": "user3"})

    # Get members of the first groupchat
    members_groupchat1_response = client.get(f"/api/groupchats/{groupchat1_id}/members")
    members_groupchat1 = members_groupchat1_response.json()
    member_ids_groupchat1 = [member["user_id"] for member in members_groupchat1]

    # Get members of the second groupchat
    members_groupchat2_response = client.get(f"/api/groupchats/{groupchat2_id}/members")
    members_groupchat2 = members_groupchat2_response.json()
    member_ids_groupchat2 = [member["user_id"] for member in members_groupchat2]

    # Verify that user2 is only in the first groupchat and user3 is only in the second
    assert "user2" in member_ids_groupchat1
    assert "user3" not in member_ids_groupchat1
    assert "user3" in member_ids_groupchat2
    assert "user2" not in member_ids_groupchat2
