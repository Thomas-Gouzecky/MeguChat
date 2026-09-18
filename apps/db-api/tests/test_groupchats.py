from fastapi.testclient import TestClient

from app.main import app

client = TestClient(app)


def test_create_new_groupchat_successful():
    body = {"users": ["user1", "user2", "user3"], "name": "Testing Group Chat"}
    response = client.post("/api/groupchats", json=body)

    assert response.status_code == 200
