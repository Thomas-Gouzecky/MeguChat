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
