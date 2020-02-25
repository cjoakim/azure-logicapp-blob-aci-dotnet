# azure-logicapp-blob-aci-dotnet

Azure Logic App triggered by Azure Storage Blobs, executing an ACI container with DotNet Core code

---

## DotNet Core Project Creation

### Bootstrap the DotNet Core Project

```
$ dotnet --version
3.1.101
$ dotnet new console -o blobs
$ cd blobs
$ dotnet add package Microsoft.Azure.Storage.Blob
$ dotnet build
$ dotnet run
Hello World!
```

### Create and Execute the Docker Container

```
$ ./containerize.sh 
$ docker run -d -e xxx=yyy cjoakim/azure-blobs-core:latest
```

---

## Azure Logic

### Azure Logic App Designer View

![logic-app-designer](img/logic-app-designer.png)

### Sample Blob Trigger JSON

The JSON triggerBody() looks like this:

```
{
  "Id": "JTJmc2ltdWxhdGlvbnMlMmZwb3N0YWxfY29kZXNfbmMuY3N2",
  "Name": "postal_codes_nc.csv",
  "DisplayName": "postal_codes_nc.csv",
  "Path": "/simulations/postal_codes_nc.csv",
  "LastModified": "2020-02-24T22:13:23Z",
  "Size": 61273,
  "MediaType": "text/csv",
  "IsFolder": false,
  "ETag": "\"0x8D7B976C3E20740\"",
  "FileLocator": "JTJmc2ltdWxhdGlvbnMlMmZwb3N0YWxfY29kZXNfbmMuY3N2",
  "LastModifiedBy": null
}
```

### Azure Logic App Code View 

```
{
    "definition": {
        "$schema": "https://schema.management.azure.com/providers/Microsoft.Logic/schemas/2016-06-01/workflowdefinition.json#",
        "actions": {
            "Create_container_group": {
                "inputs": {
                    "body": {
                        "location": "@{string('eastus')}",
                        "properties": {
                            "containers": [
                                {
                                    "name": "c1",
                                    "properties": {
                                        "environmentVariables": [
                                            {
                                                "name": "AZURE_STORAGE_ACCOUNT",
                                                "value": "cjoakimstorage"
                                            },
                                            {
                                                "name": "BLOB_ID",
                                                "value": "@{triggerBody()['Id']}"
                                            },
                                            {
                                                "name": "BLOB_NAME",
                                                "value": "@{triggerBody()['Name']}"
                                            },
                                            {
                                                "name": "BLOB_PATH",
                                                "value": "@{triggerBody()['Path']}"
                                            }
                                        ],
                                        "image": "cjoakim/azure-blobs-core:latest",
                                        "resources": {
                                            "limits": {
                                                "cpu": 1,
                                                "memoryInGB": 2
                                            },
                                            "requests": {
                                                "cpu": 1,
                                                "memoryInGB": 2
                                            }
                                        }
                                    }
                                }
                            ],
                            "osType": "Linux",
                            "restartPolicy": "Never"
                        }
                    },
                    "host": {
                        "connection": {
                            "name": "@parameters('$connections')['aci']['connectionId']"
                        }
                    },
                    "method": "put",
                    "path": "/subscriptions/@{encodeURIComponent('your-subscription-id')}/resourceGroups/@{encodeURIComponent('cjoakim-logic')}/providers/Microsoft.ContainerInstance/containerGroups/@{encodeURIComponent(string('aci-group'))}",
                    "queries": {
                        "x-ms-api-version": "2017-10-01-preview"
                    }
                },
                "runAfter": {},
                "type": "ApiConnection"
            },
            "Delay": {
                "inputs": {
                    "interval": {
                        "count": 40,
                        "unit": "Second"
                    }
                },
                "runAfter": {
                    "Create_container_group": [
                        "Succeeded"
                    ]
                },
                "type": "Wait"
            },
            "Get_logs_of_a_container": {
                "inputs": {
                    "host": {
                        "connection": {
                            "name": "@parameters('$connections')['aci']['connectionId']"
                        }
                    },
                    "method": "get",
                    "path": "/subscriptions/@{encodeURIComponent('your-subscription-id')}/resourceGroups/@{encodeURIComponent('cjoakim-logic')}/providers/Microsoft.ContainerInstance/containerGroups/@{encodeURIComponent('aci-group')}/containers/@{encodeURIComponent('c1')}/logs",
                    "queries": {
                        "x-ms-api-version": "2017-10-01-preview"
                    }
                },
                "runAfter": {
                    "Delay": [
                        "Succeeded"
                    ]
                },
                "type": "ApiConnection"
            }
        },
        "contentVersion": "1.0.0.0",
        "outputs": {},
        "parameters": {
            "$connections": {
                "defaultValue": {},
                "type": "Object"
            }
        },
        "triggers": {
            "When_a_blob_is_added_or_modified_(properties_only)": {
                "inputs": {
                    "host": {
                        "connection": {
                            "name": "@parameters('$connections')['azureblob']['connectionId']"
                        }
                    },
                    "method": "get",
                    "path": "/datasets/default/triggers/batch/onupdatedfile",
                    "queries": {
                        "folderId": "JTJmc2ltdWxhdGlvbnM=",
                        "maxFileCount": 1
                    }
                },
                "metadata": {
                    "JTJmc2ltdWxhdGlvbnM=": "/simulations"
                },
                "recurrence": {
                    "frequency": "Minute",
                    "interval": 1
                },
                "splitOn": "@triggerBody()",
                "type": "ApiConnection"
            }
        }
    },
    "parameters": {
        "$connections": {
            "value": {
                "aci": {
                    "connectionId": "/subscriptions/your-subscription-id/resourceGroups/cjoakim-logic/providers/Microsoft.Web/connections/aci",
                    "connectionName": "aci",
                    "id": "/subscriptions/your-subscription-id/providers/Microsoft.Web/locations/eastus/managedApis/aci"
                },
                "azureblob": {
                    "connectionId": "/subscriptions/your-subscription-id/resourceGroups/cjoakim-logic/providers/Microsoft.Web/connections/azureblob",
                    "connectionName": "azureblob",
                    "id": "/subscriptions/your-subscription-id/providers/Microsoft.Web/locations/eastus/managedApis/azureblob"
                }
            }
        }
    }
}
```



---

## Links

- https://docs.microsoft.com/en-us/azure/storage/blobs/storage-quickstart-blobs-dotnet
- https://www.klaine.net/example-azure-container-instances-with-logic-apps/
- https://www.nuget.org/packages/Joakimsoftware.M26/
- https://github.com/cjoakim/m26-cs
