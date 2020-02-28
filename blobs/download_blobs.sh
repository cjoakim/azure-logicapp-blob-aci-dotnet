#!/bin/bash

# Bash script to execute Program.cs locally, outside of Docker,
# to download, and optionally delete, the target/calculated blobs.
# Chris Joakim, Microsoft, 2020/02/28

export RUNTYPE=download_target_blobs
export TARGET_BLOB_CONTAINER=processed
export DELETE_AFTER_DOWNLOAD=1

# AZURE_STORAGE_CONNECTION_STRING is already an env var on my workstatiob.

dotnet build
dotnet run 
