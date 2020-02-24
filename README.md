# azure-logicapp-blob-aci-dotnet

Azure Logic App triggered by Azure Storage Blobs, executing an ACI container with DotNet Core code

## Project Creation - DotNet Core Containerized App

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

```
$ docker build -t cjoakim/azure-blobs-core .
```
