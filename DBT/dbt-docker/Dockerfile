##
#  Generic dockerfile for dbt image building.
#  See README for operational details
##

# Top level build args
ARG build_for=linux/amd64
##
# base image (abstract)
##
FROM --platform=$build_for python:3.10.7-slim-bullseye as base

# N.B. The refs updated automagically every release via bumpversion
# N.B. dbt-postgres is currently found in the core codebase so a value of dbt-core@<some_version> is correct

ARG dbt_core_ref=dbt-core@v1.3.0




# System setup
RUN apt-get update \
  && apt-get dist-upgrade -y \
  && apt-get install -y --no-install-recommends \
    git \
    ssh-client \
    software-properties-common \
    make \
    build-essential \
    ca-certificates \
    libpq-dev \
	wget \
  && apt-get clean \
  && rm -rf \
    /var/lib/apt/lists/* \
    /tmp/* \
    /var/tmp/*

# Env vars
ENV PYTHONIOENCODING=utf-8
ENV LANG=C.UTF-8
# Update python
RUN python -m pip install --upgrade pip setuptools wheel --no-cache-dir

# Set docker basics
WORKDIR /usr/app/dbt-runner
VOLUME /usr/app


##
# dbt-core
##
FROM base as dbt-core
RUN python -m pip install --no-cache-dir "git+https://github.com/dbt-labs/${dbt_core_ref}#egg=dbt-core&subdirectory=core"

##
# dbt-databricks
##
FROM dbt-core as dbt-databricks
RUN python -m pip install dbt-databricks

##
# azure-cli
##
FROM dbt-databricks as azcopy
RUN wget https://aka.ms/downloadazcopy-v10-linux \
      && tar -xvf downloadazcopy-v10-linux \
	  && cp ./azcopy_linux_amd64_*/azcopy /usr/bin/ \
	  && rm -r ./azcopy_linux_amd64_* \
	  && rm downloadazcopy-v10-linux

##
# api
##
RUN python -m pip install fastapi pydantic uvicorn azure.storage.blob --no-cache-dir

	  
CMD azcopy copy "${DBT_SAS}" "/usr/app/" --recursive=true ; ls ; uvicorn main:app --host 0.0.0.0 --port 80
#CMD azcopy copy "${DBT_SAS}" "/usr/app/" --recursive=true ; dbt run --profiles-dir profiles
#CMD ["az"]