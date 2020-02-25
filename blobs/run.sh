#!/bin/bash

# Bash script to execute Program.cs locally, outside of Docker.
# Chris Joakim, Microsoft, 2020/02/25

# export RUNTYPE=env
# export RUNTYPE=populate_storage_blobs
export RUNTYPE=logic_app_process_blob

export POPULATE_STORAGE_RANDOMNESS=5
export POPULATE_STORAGE_DELAY=3
export SOURCE_BLOB_ID=JTJmc2ltdWxhdGlvbnMlMmZwb3N0YWxfY29kZXNfbmMuY3N2
export SOURCE_BLOB_PATH=/simulations/benjamin_hebert_1582653235.csv
export SOURCE_BLOB_NAME=benjamin_hebert_1582653235.csv
export TARGET_BLOB_CONTAINER=processed

dotnet build
dotnet run 
