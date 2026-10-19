import tomllib


def get_required_setting(setting_name: str) -> str:
    with open("pyproject.toml", "rb") as f:
        pyproject_data = tomllib.load(f)
    value = pyproject_data.get("project", {}).get(setting_name)
    if not value:
        raise RuntimeError(
            f"Missing required mcp setting: {setting_name}. "
            "Set it in pyproject.toml."
        )
    return value


SERVER_NAME = get_required_setting("name")
SERVER_VERSION = get_required_setting("version")
SERVER_DESCRIPTION = get_required_setting("description")
