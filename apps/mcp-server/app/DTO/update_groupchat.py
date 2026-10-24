from dataclasses import dataclass


@dataclass
class UpdateGroupchatDTO:
    name: str | None = None
