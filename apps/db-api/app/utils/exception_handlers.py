from fastapi import FastAPI, Request
from fastapi.responses import JSONResponse
from app.errors import UnauthenticateError
import logging

logging.basicConfig(level=logging.INFO)
logger = logging.getLogger(__name__)


def create_exception_handlers(app: FastAPI):
    @app.exception_handler(Exception)
    async def generic_exception_handler(request: Request, exc: Exception):
        logger.error(f"Unhandled exception: {exc}", exc_info=True)
        return JSONResponse(
            status_code=500,
            content={"detail": "Internal Server Error"},
        )

    @app.exception_handler(UnauthenticateError)
    async def unauthenticated_exception_handler(
        request: Request, exc: UnauthenticateError
    ):
        logger.warning(f"Unauthenticated access attempt: {exc.detail}")
        return JSONResponse(
            status_code=exc.status_code,
            content={"detail": exc.detail},
        )

    @app.exception_handler(PermissionError)
    async def permission_exception_handler(request: Request, exc: PermissionError):
        logger.warning(f"Permission denied: {exc}")
        return JSONResponse(
            status_code=403,
            content={"detail": "Permission Denied"},
        )

    @app.exception_handler(ValueError)
    async def value_error_handler(request: Request, exc: ValueError):
        return JSONResponse(
            status_code=422,
            content={"detail": str(exc)},
        )

    @app.exception_handler(LookupError)
    async def lookup_error_handler(request: Request, exc: LookupError):
        return JSONResponse(
            status_code=404,
            content={"detail": str(exc)},
        )
