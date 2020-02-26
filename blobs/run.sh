#!/bin/bash

# Bash script to execute Program.cs locally, outside of Docker.
# Chris Joakim, Microsoft, 2020/02/26

# export RUNTYPE=env
# export RUNTYPE=logic_app_process_blob
# export RUNTYPE=populate_storage_blobs
export RUNTYPE=populate_storage_blobs

export POPULATE_STORAGE_RANDOM_PCT=101
export POPULATE_STORAGE_DELAY=3
export POPULATE_STORAGE_MAX_COUNT=0
export TARGET_BLOB_CONTAINER=processed

# these are used to simulate the processing of a blob in the Logic App
export SOURCE_BLOB_NAME=cjoakim.csv
export SOURCE_BLOB_PATH=/simulations/cjoakim.csv

# AZURE_STORAGE_CONNECTION_STRING is already an env var on my workstatiob.

dotnet build
dotnet run 

# "@{concat('c-', workflow().run.name)}"
# "@{toLower(concat('c-', workflow().run.name))}"
# c-08586188714282859200091546359cu06
# az container delete --name c-08586188715343982168990286893cu39 --resource-group cjoakim-logic