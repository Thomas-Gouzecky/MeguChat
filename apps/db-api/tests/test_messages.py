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
        response = client.post(
            "/api/groupchats", json={"name": name}, headers={"X-User-ID": "user1"}
        )
        assert response.status_code == 201
        groupchat_id = response.json()["groupchat_id"]

        member_response = client.post(
            f"/api/groupchats/{groupchat_id}/members",
            json={"users": ["user1", "user2"]},
            headers={"X-User-ID": "user1"},
        )
        assert member_response.status_code == 201
        assert (
            len(member_response.json()) == 1
        )  # user1 is already a member, so only user2 should be added

        return groupchat_id

    return _create_groupchat


def test_get_messages_for_groupchat(create_groupchat):
    groupchat_id = create_groupchat("Test Group for Messages")
    response = client.get(
        f"/api/groupchats/{groupchat_id}/messages", headers={"X-User-ID": "user1"}
    )

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
    assert response.status_code == 201
    assert response.json()["content"] == "Hello, this is a test message."

    # Retrieve messages for the groupchat
    get_response = client.get(
        f"/api/groupchats/{groupchat_id}/messages", headers={"X-User-ID": "user1"}
    )
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
    assert response.status_code == 201
    message_id = response.json()["id"]

    # Delete the message
    delete_response = client.delete(
        f"/api/groupchats/{groupchat_id}/messages/{message_id}",
        headers={"X-User-ID": "user1"},
    )
    assert delete_response.status_code == 200
    assert delete_response.json()["id"] == message_id

    # Verify that the message has been deleted
    get_response = client.get(
        f"/api/groupchats/{groupchat_id}/messages", headers={"X-User-ID": "user1"}
    )
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
    assert response.status_code == 201
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
    assert response.status_code == 201
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
    get_response = client.get(
        f"/api/groupchats/{groupchat_id}/messages", headers={"X-User-ID": "user1"}
    )
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
    assert response.status_code == 201
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
    assert response.status_code == 201

    # Attempt to retrieve messages for a user not in the groupchat
    get_response = client.get(
        f"/api/groupchats/{groupchat_id}/messages",
        headers={"X-User-ID": "unauthorized_user"},
    )
    assert get_response.status_code == 403


def test_users_not_in_groupchat_cannot_add_messages(create_groupchat):
    groupchat_id = create_groupchat("Test Group for Access Control on Adding Messages")
    # Attempt to add a message to the groupchat by a user not in the groupchat
    message_request_body = {"content": "Unauthorized message attempt."}
    response = client.post(
        f"/api/groupchats/{groupchat_id}/messages",
        json=message_request_body,
        headers={"X-User-ID": "unauthorized_user"},
    )

    assert response.status_code == 403


def test_messages_returned_in_chronological_order(create_groupchat):
    groupchat_id = create_groupchat("Test Group for Message Order")
    # Add multiple messages to the groupchat
    messages_to_add = [
        {"content": "First message."},
        {"content": "Second message."},
        {"content": "Third message."},
    ]

    for message in messages_to_add:
        response = client.post(
            f"/api/groupchats/{groupchat_id}/messages",
            json=message,
            headers={"X-User-ID": "user1"},
        )
        assert response.status_code == 201

    # Retrieve messages for the groupchat
    get_response = client.get(
        f"/api/groupchats/{groupchat_id}/messages", headers={"X-User-ID": "user1"}
    )
    assert get_response.status_code == 200
    messages = get_response.json()

    assert isinstance(messages, list)
    assert len(messages) == len(messages_to_add)

    assert [message["content"] for message in messages] == [
        "First message.",
        "Second message.",
        "Third message.",
    ]


def test_editing_message_still_returns_in_chronological_order(create_groupchat):
    groupchat_id = create_groupchat("Test Group for Editing Message Order")
    # Add multiple messages to the groupchat
    messages_to_add = [
        {"content": "First message."},
        {"content": "Second message."},
        {"content": "Third message."},
    ]

    message_ids = []
    for message in messages_to_add:
        response = client.post(
            f"/api/groupchats/{groupchat_id}/messages",
            json=message,
            headers={"X-User-ID": "user1"},
        )
        assert response.status_code == 201
        message_ids.append(response.json()["id"])

    # Edit the second message
    update_request_body = {"content": "Updated second message."}
    update_response = client.put(
        f"/api/groupchats/{groupchat_id}/messages/{message_ids[1]}",
        json=update_request_body,
        headers={"X-User-ID": "user1"},
    )
    assert update_response.status_code == 200

    # Retrieve messages for the groupchat
    get_response = client.get(
        f"/api/groupchats/{groupchat_id}/messages", headers={"X-User-ID": "user1"}
    )
    assert get_response.status_code == 200
    messages = get_response.json()

    assert isinstance(messages, list)
    assert len(messages) == len(messages_to_add)

    # Editing a message must not change its creation-time position.
    expected_contents = [
        "First message.",
        "Updated second message.",
        "Third message.",
    ]
    for i, message in enumerate(messages):
        assert message["content"] == expected_contents[i]


def test_messages_are_limited_by_query_parameter(create_groupchat):
    groupchat_id = create_groupchat("Test Group for Message Limit")
    # Add multiple messages to the groupchat
    messages_to_add = [
        {"content": "Message 1."},
        {"content": "Message 2."},
        {"content": "Message 3."},
        {"content": "Message 4."},
        {"content": "Message 5."},
    ]

    for message in messages_to_add:
        response = client.post(
            f"/api/groupchats/{groupchat_id}/messages",
            json=message,
            headers={"X-User-ID": "user1"},
        )
        assert response.status_code == 201

    # Retrieve messages for the groupchat with a limit of 3
    get_response = client.get(
        f"/api/groupchats/{groupchat_id}/messages?limit=3",
        headers={"X-User-ID": "user1"},
    )
    assert get_response.status_code == 200
    messages = get_response.json()

    assert isinstance(messages, list)
    assert len(messages) == 3


def test_messages_fetched_before_date_in_chronological_order(create_groupchat):
    groupchat_id = create_groupchat("Test Group for Fetching Messages Before Date")
    # Add multiple messages to the groupchat
    messages_to_add = [
        {"content": "Message 1."},
        {"content": "Message 2."},
        {"content": "Message 3."},
        {"content": "Message 4."},
        {"content": "Message 5."},
    ]

    added_messages = []
    for message in messages_to_add:
        response = client.post(
            f"/api/groupchats/{groupchat_id}/messages",
            json=message,
            headers={"X-User-ID": "user1"},
        )
        assert response.status_code == 201
        added_messages.append(response.json())

    # Retrieve messages for the groupchat with a limit of 2
    before_created_at = added_messages[4]["created_at"]
    get_response = client.get(
        f"/api/groupchats/{groupchat_id}/messages?limit=2&before_created_at={before_created_at}",
        headers={"X-User-ID": "user1"},
    )
    assert get_response.status_code == 200
    messages = get_response.json()

    assert isinstance(messages, list)
    assert len(messages) == 2
    assert [message["content"] for message in messages] == [
        "Message 3.",
        "Message 4.",
    ]
    assert all(message["created_at"] < before_created_at for message in messages)
    assert messages[0]["created_at"] < messages[1]["created_at"]

    # get the new set of messages with a limit of 2 and before_created_at set to the created_at of the last message retrieved
    before_created_at = messages[0]["created_at"]
    get_response = client.get(
        f"/api/groupchats/{groupchat_id}/messages?limit=2&before_created_at={before_created_at}",
        headers={"X-User-ID": "user1"},
    )
    assert get_response.status_code == 200
    messages = get_response.json()
    assert isinstance(messages, list)
    assert len(messages) == 2
    assert [message["content"] for message in messages] == [
        "Message 1.",
        "Message 2.",
    ]
    assert all(message["created_at"] < before_created_at for message in messages)
    assert messages[0]["created_at"] < messages[1]["created_at"]
