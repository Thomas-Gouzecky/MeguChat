class UnauthenticateError(Exception):
    """Exception raised for unauthenticated access attempts."""

    def __init__(self, status_code: int = 401, detail: str = "Unauthenticated"):
        self.status_code = status_code
        self.detail = detail
        super().__init__(self.detail)
