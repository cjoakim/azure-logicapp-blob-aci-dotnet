using System;
using System.Collections;
using System.Threading;

// https://docs.microsoft.com/en-us/azure/storage/blobs/storage-quickstart-blobs-dotnet-legacy

namespace blobs
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("starting");

            if (args.Length == 0)
            {
                Console.WriteLine("no command-line args");
            }
            else
            {
                foreach (string arg in args)
                {
                    Console.WriteLine("arg: " + arg);
                }
            }
                
            foreach (DictionaryEntry de in Environment.GetEnvironmentVariables())
            {
                Console.WriteLine("{0} = {1}", de.Key, de.Value);
            }

            Console.WriteLine("sleeping");
            System.Threading.Thread.Sleep(10000);
            Console.WriteLine("exiting");
        }
    }
}

//{
//  "Id": "JTJmc2ltdWxhdGlvbnMlMmZwb3N0YWxfY29kZXNfY3QuY3N2",
//  "Name": "postal_codes_ct.csv",
//  "DisplayName": "postal_codes_ct.csv",
//  "Path": "/simulations/postal_codes_ct.csv",
//  "LastModified": "2020-02-24T21:15:12Z",
//  "Size": 24394,
//  "MediaType": "text/csv",
//  "IsFolder": false,
//  "ETag": "\"0x8D7B96EA3067BB7\"",
//  "FileLocator": "JTJmc2ltdWxhdGlvbnMlMmZwb3N0YWxfY29kZXNfY3QuY3N2",
//  "LastModifiedBy": null
//}
