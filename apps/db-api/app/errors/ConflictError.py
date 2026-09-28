class ConflictError(Exception):
    """Exception raised for conflicts in the database, such as unique constraint violations."""

    def __init__(self, detail: str, status_code: int = 409):
        self.status_code = status_code
        self.detail = detail
        super().__init__(self.detail)
