from typing import Union
from azure.storage.blob import ContainerClient
from pydantic import BaseModel
from fastapi import FastAPI
import os
from subprocess import Popen, PIPE
import shutil
app = FastAPI()

class DbtConf(BaseModel):
    account_url: str
    project_name: str


@app.get("/health")
def read_root():
    return {"status": "ok"}

@app.post("/dbt")
async def dbt_run(dbt_conf : DbtConf):
        container = ContainerClient.from_container_url(container_url = dbt_conf.account_url)
        base_path = "dbt-code/"+ dbt_conf.project_name
        try:
            shutil.rmtree(base_path)
        except OSError as e:
            print("Error: %s - %s." % (e.filename, e.strerror))
        try:
            for blob in container.list_blobs():
                if(blob.content_settings.content_type != None) :
                    file_content = ContainerClient.download_blob(container,blob=blob)
                    download_file_path = os.path.join(base_path, blob.name)
                    os.makedirs(os.path.dirname(download_file_path), exist_ok=True)
                    with open(download_file_path, "wb") as file:
                        file.write(file_content.readall())
        except Exception as e:
            print(e)

        stream = os.popen('cd '+base_path+';dbt run --profiles-dir profiles')
        output = stream.read()
        return output
    

