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
    create_response = client.post("/api/groupchats", json=request_body)
    assert create_response.status_code == 200
    groupchat_id = create_response.json()["groupchat_id"]

    # Delete the groupchat
    delete_response = client.delete(f"/api/groupchats/{groupchat_id}")
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
    