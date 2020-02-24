using System;
using System.Threading;

// https://docs.microsoft.com/en-us/azure/storage/blobs/storage-quickstart-blobs-dotnet-legacy

namespace blobs
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            System.Threading.Thread.Sleep(60000);
            Console.WriteLine("exiting");
        }
    }
}
