from dataclasses import dataclass


@dataclass
class CreateGroupchatDTO:
    name: str
    users: list[str]
