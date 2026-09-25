from fastapi.testclient import TestClient

from app.main import app

client = TestClient(app)


def test_create_new_groupchat_successful():
    request_body = {"name": "Testing Group Chat"}
    response = client.post("/api/groupchats", json=request_body)

    assert response.status_code == 200

    response_body = response.json()

    assert "groupchat_id" in response_body
    assert isinstance(response_body["groupchat_id"], int)
    assert response_body["groupchat_id"] > 0


def test_create_new_groupchat_missing_fields():
    request_body = {}
    response = client.post("/api/groupchats", json=request_body)

    assert response.status_code == 422


def test_update_groupchat_name():
    # Create a new groupchat first
    request_body = {"name": "Initial Group Chat Name"}
    create_response = client.post("/api/groupchats", json=request_body)
    assert create_response.status_code == 200
    groupchat_id = create_response.json()["groupchat_id"]

    # Update the groupchat name
    update_request_body = {"name": "Updated Group Chat Name"}
    update_response = client.put(
        f"/api/groupchats/{groupchat_id}", json=update_request_body
    )

    assert update_response.status_code == 200
    updated_groupchat = update_response.json()

    assert updated_groupchat["groupchat_id"] == groupchat_id
    assert updated_groupchat["name"] == "Updated Group Chat Name"


def test_update_groupchat_missing_fields():
    # Create a new groupchat first
    request_body = {"name": "Initial Group Chat Name"}
    create_response = client.post("/api/groupchats", json=request_body)
    assert create_response.status_code == 200
    groupchat_id = create_response.json()["groupchat_id"]

    # Update the groupchat with missing fields
    update_request_body = {}
    update_response = client.put(
        f"/api/groupchats/{groupchat_id}", json=update_request_body
    )

    assert update_response.status_code == 422


def test_delete_groupchat():
    # Create a new groupchat first
    request_body = {"name": "Group Chat to Delete"}
    create_response = client.post(
        "/api/groupchats", json=request_body, headers={"X-User-Id": "some_user"}
    )
    assert create_response.status_code == 200
    groupchat_id = create_response.json()["groupchat_id"]

    # Delete the groupchat
    delete_response = client.delete(
        f"/api/groupchats/{groupchat_id}", headers={"X-User-Id": "some_user"}
    )
    assert delete_response.status_code == 200
    delete_message = delete_response.json()
    assert (
        delete_message["message"]
        == f"Groupchat with ID {groupchat_id} has been deleted."
    )

    # Verify that the groupchat no longer exists
    get_response = client.get(f"/api/groupchats/user/some_user")
    assert get_response.status_code == 200
    groupchats = get_response.json()
    assert all(gc["groupchat_id"] != groupchat_id for gc in groupchats)


def test_delete_nonexistent_groupchat():
    # Attempt to delete a groupchat that doesn't exist
    nonexistent_groupchat_id = 9999
    delete_response = client.delete(f"/api/groupchats/{nonexistent_groupchat_id}")
    assert delete_response.status_code == 422


def test_get_messages_for_groupchat():
    # Create a new groupchat first
    request_body = {"name": "Group Chat for Messages"}
    create_response = client.post("/api/groupchats", json=request_body)
    assert create_response.status_code == 200
    groupchat_id = create_response.json()["groupchat_id"]

    # Get messages for the newly created groupchat
    get_messages_response = client.get(f"/api/groupchats/{groupchat_id}/messages")
    assert get_messages_response.status_code == 200
    messages = get_messages_response.json()
    assert isinstance(messages, list)


def test_get_messages_for_nonexistent_groupchat():
    # Attempt to get messages for a groupchat that doesn't exist
    nonexistent_groupchat_id = 9999
    get_messages_response = client.get(
        f"/api/groupchats/{nonexistent_groupchat_id}/messages",
        headers={"X-User-Id": "user1"},
    )
    assert get_messages_response.status_code == 422


def test_ensure_messages_are_groupchat_specific():
    # Create two groupchats
    request_body1 = {"name": "Group Chat 1"}
    create_response1 = client.post("/api/groupchats", json=request_body1)
    assert create_response1.status_code == 200
    groupchat_id1 = create_response1.json()["groupchat_id"]

    request_body2 = {"name": "Group Chat 2"}
    create_response2 = client.post("/api/groupchats", json=request_body2)
    assert create_response2.status_code == 200
    groupchat_id2 = create_response2.json()["groupchat_id"]

    # Get messages for both groupchats
    get_messages_response1 = client.get(f"/api/groupchats/{groupchat_id1}/messages")
    assert get_messages_response1.status_code == 200
    messages1 = get_messages_response1.json()

    get_messages_response2 = client.get(f"/api/groupchats/{groupchat_id2}/messages")
    assert get_messages_response2.status_code == 200
    messages2 = get_messages_response2.json()

    # Ensure that messages for one groupchat do not appear in the other
    assert all(msg not in messages2 for msg in messages1)
