#!/bin/bash

# Bash script to execute the Azure CLI (az) to deploy an 
# Azure Resource Manager (ARM) template.
# Chris Joakim, Microsoft, 2020/04/01
#
# Azure CLI (Command Line Interface)
# https://docs.microsoft.com/en-us/cli/azure/?view=azure-cli-latest
# https://docs.microsoft.com/en-us/cli/azure/install-azure-cli?view=azure-cli-latest

# Azure Resource Manager (ARM)
# https://docs.microsoft.com/en-us/azure/azure-resource-manager/templates/overview

# Visual Studio Code (VSC)
# https://code.visualstudio.com/

# VSC Logic App Extension
# https://docs.microsoft.com/en-us/azure/logic-apps/quickstart-create-logic-apps-visual-studio-code
#
# Visual Studio Logic App Tools
# https://marketplace.visualstudio.com/items?itemName=VinaySinghMSFT.AzureLogicAppsToolsForVS2019

# azure-specific values
resource_group="cjoakim-logic5"
rg_region="eastus"

# construct unique time-based names for the deployment
name="logicapp"
epoch_time=`date +%s`
dep_name=""$epoch_time"-"$name"-deployment"

echo "resource_group: "$resource_group
echo "epoch_time:     "$epoch_time

#az login  (you'll have to run this command occasionally)

echo 'Create the Resource Group (RG) with command: az group create...'
az group create --name $resource_group --location $rg_region

echo 'Deploy to the Resource Group (RG) with command: az group deployment create...'
az group deployment create \
  --name $dep_name \
  --resource-group $resource_group \
  --template-file template.json \
  --parameters @parameters.json
