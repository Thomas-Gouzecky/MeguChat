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
