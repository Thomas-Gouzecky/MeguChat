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
        request_body = {"name": name}
        response = client.post("/api/groupchats", json=request_body)
        assert response.status_code == 200
        return response.json()["groupchat_id"]

    def _add_members_to_groupchat(groupchat_id: int, user_ids: list[str]):
        for user_id in user_ids:
            response = client.post(
                f"/api/groupchats/{groupchat_id}/members", json={"user_id": user_id}
            )
            assert response.status_code == 200

    chat_id = _create_groupchat("Test Group")
    _add_members_to_groupchat(chat_id, ["user1", "user2"])
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
        f"/api/groupchats/{groupchat_id}/messages", json=message_request_body
    )
    assert response.status_code == 200

    # Retrieve messages for the groupchat
    get_response = client.get(f"/api/groupchats/{groupchat_id}/messages")
    assert get_response.status_code == 200
    messages = get_response.json()

    assert isinstance(messages, list)
    assert len(messages) == 1
    assert messages[0]["content"] == "Hello, this is a test message."


def test_delete_message_from_groupchat(create_groupchat):
    groupchat_id = create_groupchat("Test Group for Deleting Messages")
    # Add a message to the groupchat
    message_request_body = {"content": "Message to be deleted."}
    response = client.post(
        f"/api/groupchats/{groupchat_id}/messages", json=message_request_body
    )
    assert response.status_code == 200
    message_id = response.json()["id"]

    # Delete the message
    delete_response = client.delete(
        f"/api/groupchats/{groupchat_id}/messages/{message_id}"
    )
    assert delete_response.status_code == 200

    # Verify that the message has been deleted
    get_response = client.get(f"/api/groupchats/{groupchat_id}/messages")
    assert get_response.status_code == 200
    messages = get_response.json()

    assert isinstance(messages, list)
    assert all(message["id"] != message_id for message in messages)
