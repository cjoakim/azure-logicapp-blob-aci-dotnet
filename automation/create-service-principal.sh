#!/bin/bash

# Bash script to execute the Azure CLI (az) to create an Azure Service Principal.
# Chris Joakim, Microsoft, 2020/08/11
#
# Links:
# https://docs.microsoft.com/en-us/cli/azure/create-an-azure-service-principal-azure-cli?view=azure-cli-latest
# https://medium.com/@ArsenVlad/how-to-create-and-test-azure-service-principal-using-azure-cli-647787cdb526

# az ad sp create-for-rbac --help

az ad sp create-for-rbac --name chjoakim20200811

# az ad sp list


# Output:
# Changing "chjoakim20200811" to a valid URI of "http://chjoakim20200811", which is the required format used for service principal names
# Creating a role assignment under the scope of "/subscriptions/61761119-d249-4507-90c6-a16517e1874c"
#   Retrying role assignment creation: 1/36
# {
#   "appId": "fffcc82e-03a5-4aa2-...",
#   "displayName": "chjoakim20200418",
#   "name": "http://chjoakim20200418",
#   "password": "8a7aa712-ff89-4172-...",
#   "tenant": "72f988bf-86f1-41af-..."
# }
