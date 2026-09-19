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
