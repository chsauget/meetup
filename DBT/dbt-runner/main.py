from typing import Union

from fastapi import FastAPI
import os
app = FastAPI()


@app.get("/health")
def read_root():
    return {"status": "ok"}

@app.get("/")
def read_root():
    stream = os.popen('dbt run --profiles-dir profiles')
    output = stream.read()
    return output
    

