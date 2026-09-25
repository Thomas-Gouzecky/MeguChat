from fastapi import FastAPI

from app.sql import create_db_and_tables

from app.routes import messages, groupchats, members

app = FastAPI()

app.include_router(messages.router)
app.include_router(groupchats.router)
app.include_router(members.router)


@app.on_event("startup")
def on_startup():
    create_db_and_tables()


@app.get("/health")
def health():
    return {"status": "ok"}
