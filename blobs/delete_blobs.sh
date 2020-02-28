#!/bin/bash

# Bash script to execute Program.cs locally, outside of Docker,
# to delete any blobs in source/simulations container.
# Chris Joakim, Microsoft, 2020/02/28

export RUNTYPE=delete_source_blobs

# AZURE_STORAGE_CONNECTION_STRING is already an env var on my workstatiob.

dotnet build
dotnet run 
