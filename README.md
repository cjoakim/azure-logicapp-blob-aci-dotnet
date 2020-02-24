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

## Azure Logic App Creation
