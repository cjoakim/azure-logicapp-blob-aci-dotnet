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
            Console.WriteLine("Hello World!");

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
                
            System.Threading.Thread.Sleep(20000);
            Console.WriteLine("exiting");
        }
    }
}
