from fastapi import FastAPI

from app.sql import SessionDep, create_db_and_tables

from app.routes import messages, groupchats

app = FastAPI()

app.include_router(messages.router)
app.include_router(groupchats.router)


@app.on_event("startup")
def on_startup():
    create_db_and_tables()


@app.get("/health")
def health():
    return {"status": "ok"}
