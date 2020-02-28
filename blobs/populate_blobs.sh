#!/bin/bash

# Bash script to execute Program.cs locally, outside of Docker,
# to populate the source/simulations container with randomized blobs.
# Chris Joakim, Microsoft, 2020/02/28

export RUNTYPE=populate_storage_blobs
export POPULATE_STORAGE_RANDOM_PCT=15
export POPULATE_STORAGE_MAX_COUNT=50
export POPULATE_STORAGE_DELAY=250

# AZURE_STORAGE_CONNECTION_STRING is already an env var on my workstatiob.

dotnet build
dotnet run 
