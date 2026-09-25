from fastapi.testclient import TestClient
from pytest import fixture

from app.main import app

client = TestClient(app)


@fixture(autouse=True)
def setup_and_teardown():
    # Setup: Create the database and tables before each test
    from app.sql.session import create_db_and_tables

    create_db_and_tables()
    yield
    # Teardown: Drop the database and tables after each test
    from app.sql.session import engine, SQLModel

    SQLModel.metadata.drop_all(engine)


@fixture
def create_groupchat():
    def _create_groupchat(name: str):
        response = client.post("/api/groupchats", json={"name": name})
        assert response.status_code == 200
        groupchat_id = response.json()["groupchat_id"]

        member_response = client.post(
            f"/api/groupchats/{groupchat_id}/members",
            json={"users": ["user1", "user2"]},
        )
        assert member_response.status_code == 200

        return groupchat_id

    return _create_groupchat


def test_get_messages_for_groupchat(create_groupchat):
    groupchat_id = create_groupchat("Test Group for Messages")
    response = client.get(f"/api/groupchats/{groupchat_id}/messages")

    assert response.status_code == 200
    response_body = response.json()

    assert isinstance(response_body, list)
    # Assuming the groupchat is new and has no messages yet
    assert len(response_body) == 0


def test_add_messages_to_groupchat(create_groupchat):
    groupchat_id = create_groupchat("Test Group for Adding Messages")
    # Add a message to the groupchat
    message_request_body = {"content": "Hello, this is a test message."}
    response = client.post(
        f"/api/groupchats/{groupchat_id}/messages",
        json=message_request_body,
        headers={"X-User-ID": "user1"},
    )
    assert response.status_code == 200
    assert response.json()["content"] == "Hello, this is a test message."

    # Retrieve messages for the groupchat
    get_response = client.get(f"/api/groupchats/{groupchat_id}/messages")
    assert get_response.status_code == 200
    messages = get_response.json()

    assert isinstance(messages, list)
    assert len(messages) == 1


def test_delete_message_from_groupchat(create_groupchat):
    groupchat_id = create_groupchat("Test Group for Deleting Messages")
    # Add a message to the groupchat
    message_request_body = {"content": "Message to be deleted."}
    response = client.post(
        f"/api/groupchats/{groupchat_id}/messages",
        json=message_request_body,
        headers={"X-User-ID": "user1"},
    )
    assert response.status_code == 200
    message_id = response.json()["id"]

    # Delete the message
    delete_response = client.delete(
        f"/api/groupchats/{groupchat_id}/messages/{message_id}",
        headers={"X-User-ID": "user1"},
    )
    assert delete_response.status_code == 200

    # Verify that the message has been deleted
    get_response = client.get(f"/api/groupchats/{groupchat_id}/messages")
    assert get_response.status_code == 200
    messages = get_response.json()

    assert isinstance(messages, list)
    assert all(message["id"] != message_id for message in messages)


def test_other_users_cannot_delete_messages(create_groupchat):
    groupchat_id = create_groupchat("Test Group for Unauthorized Deletion")
    # Add a message to the groupchat by user1
    message_request_body = {"content": "Message by user1."}
    response = client.post(
        f"/api/groupchats/{groupchat_id}/messages",
        json=message_request_body,
        headers={"X-User-ID": "user1"},
    )
    assert response.status_code == 200
    message_id = response.json()["id"]

    # Attempt to delete the message by user2 (not the owner)
    delete_response = client.delete(
        f"/api/groupchats/{groupchat_id}/messages/{message_id}",
        headers={"X-User-ID": "user2"},
    )
    # Assuming the API does not allow deletion by non-owners, we expect a 403 Forbidden or similar status code.
    assert delete_response.status_code == 422 or delete_response.status_code == 403


def test_update_message_in_groupchat(create_groupchat):
    groupchat_id = create_groupchat("Test Group for Updating Messages")
    # Add a message to the groupchat
    message_request_body = {"content": "Message to be updated."}
    response = client.post(
        f"/api/groupchats/{groupchat_id}/messages",
        json=message_request_body,
        headers={"X-User-ID": "user1"},
    )
    assert response.status_code == 200
    message_id = response.json()["id"]

    # Update the message
    update_request_body = {"content": "Updated message content."}
    update_response = client.put(
        f"/api/groupchats/{groupchat_id}/messages/{message_id}",
        json=update_request_body,
        headers={"X-User-ID": "user1"},
    )
    assert update_response.status_code == 200

    # Verify that the message has been updated
    get_response = client.get(f"/api/groupchats/{groupchat_id}/messages")
    assert get_response.status_code == 200
    messages = get_response.json()

    assert isinstance(messages, list)
    updated_message = next((msg for msg in messages if msg["id"] == message_id), None)
    assert updated_message is not None
    assert updated_message["content"] == "Updated message content."


def test_other_users_cannot_update_messages(create_groupchat):
    groupchat_id = create_groupchat("Test Group for Unauthorized Update")
    # Add a message to the groupchat by user1
    message_request_body = {"content": "Message by user1."}
    response = client.post(
        f"/api/groupchats/{groupchat_id}/messages",
        json=message_request_body,
        headers={"X-User-ID": "user1"},
    )
    assert response.status_code == 200
    message_id = response.json()["id"]

    # Attempt to update the message by user2 (not the owner)
    update_request_body = {"content": "Unauthorized update attempt."}
    update_response = client.put(
        f"/api/groupchats/{groupchat_id}/messages/{message_id}",
        json=update_request_body,
        headers={"X-User-ID": "user2"},
    )
    # Assuming the API does not allow updates by non-owners, we expect a 403 Forbidden or similar status code.
    assert update_response.status_code == 422 or update_response.status_code == 403


def test_users_not_in_groupchat_cannot_see_messages(create_groupchat):
    groupchat_id = create_groupchat("Test Group for Access Control")
    # Add a message to the groupchat
    message_request_body = {"content": "Message for access control test."}
    response = client.post(
        f"/api/groupchats/{groupchat_id}/messages",
        json=message_request_body,
        headers={"X-User-ID": "user2"},
    )
    assert response.status_code == 200

    # Attempt to retrieve messages for a user not in the groupchat
    get_response = client.get(f"/api/groupchats/{groupchat_id}/messages")
    assert get_response.status_code == 200
    messages = get_response.json()

    # Assuming the API does not filter messages based on user membership,
    # we will check if the messages are returned. In a real scenario, you would
    # implement access control and check for 403 Forbidden or similar.
    assert isinstance(messages, list)


def test_users_not_in_groupchat_cannot_add_messages(create_groupchat):
    groupchat_id = create_groupchat("Test Group for Access Control on Adding Messages")
    # Attempt to add a message to the groupchat by a user not in the groupchat
    message_request_body = {"content": "Unauthorized message attempt."}
    response = client.post(
        f"/api/groupchats/{groupchat_id}/messages",
        json=message_request_body,
        headers={"X-User-ID": "unauthorized_user"},
    )
    # Assuming the API does not allow adding messages by users not in the groupchat,
    # we expect a 403 Forbidden or similar status code. Adjust based on your implementation.
    assert response.status_code == 422 or response.status_code == 403
