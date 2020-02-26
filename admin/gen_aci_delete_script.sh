#!/bin/bash

# Chris Joakim, Microsoft, 2020/02/26
# https://docs.microsoft.com/en-us/cli/azure/container?view=azure-cli-latest#az-container-delete

source ./config.sh

rm $container_list_file
rm aci_deletes.sh

echo 'resource_group: '$resource_group

#az login

az container list --subscription $AZURE_SUBSCRIPTION_ID --resource-group $resource_group > $container_list_file

python main.py gen_aci_delete_script $resource_group $container_list_file

chmod 744 aci_deletes.sh

echo 'displaying the generated aci_deletes.sh script below:'
echo ''
cat aci_deletes.sh
echo ''
