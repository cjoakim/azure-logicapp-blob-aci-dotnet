#!/bin/bash

# Bash script to create a Docker container for this dotnet core project.
# See https://docs.microsoft.com/en-us/dotnet/core/docker/build-container
# Chris Joakim, Microsoft, 2020/02/24

container_name="cjoakim/azure-blobs-core"

dotnet restore
dotnet build
dotnet publish -c Release

echo 'building container: '$container_name
docker build -t $container_name .

docker images | grep $container_name

echo 'done'

# ./containerize.sh 
#
# docker build -t cjoakim/azure-blobs-core . 
#
# docker run -d -e xxx=yyy cjoakim/azure-blobs-core:latest
# 
# docker push cjoakim/azure-blobs-core:latest
