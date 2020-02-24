
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System;
using System.Collections;
using System.IO;
using System.Threading.Tasks;


// https://docs.microsoft.com/en-us/azure/storage/blobs/storage-quickstart-blobs-dotnet-legacy

namespace blobs
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("starting");

            string containerName = blobContainer();
            Console.WriteLine("containerName: {0}", containerName);

            string connectionString = 
                Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING");
            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);

            //BlobContainerClient containerClient = await blobServiceClient.CreateBlobContainerAsync(containerName);
            foreach (DictionaryEntry de in Environment.GetEnvironmentVariables())
            {
                Console.WriteLine("{0} = {1}", de.Key, de.Value);
            }

            Console.WriteLine("sleeping");
            System.Threading.Thread.Sleep(10000);
            Console.WriteLine("exiting");
        }

        static string blobContainer()
        {
            char[] charSeparators = new char[] { '/' };
            string blobPath = Environment.GetEnvironmentVariable("BLOB_PATH");
            return blobPath.Split(charSeparators)[1];
        }

    }
}

//{
//  "Id": "JTJmc2ltdWxhdGlvbnMlMmZwb3N0YWxfY29kZXNfbmMuY3N2",
//  "Name": "postal_codes_nc.csv",
//  "DisplayName": "postal_codes_nc.csv",
//  "Path": "/simulations/postal_codes_nc.csv",
//  "LastModified": "2020-02-24T22:13:23Z",
//  "Size": 61273,
//  "MediaType": "text/csv",
//  "IsFolder": false,
//  "ETag": "\"0x8D7B976C3E20740\"",
//  "FileLocator": "JTJmc2ltdWxhdGlvbnMlMmZwb3N0YWxfY29kZXNfbmMuY3N2",
//  "LastModifiedBy": null
//}
