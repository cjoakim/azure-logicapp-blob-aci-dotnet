
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
        static int      TEN_SECONDS = 10000;
        static string   DEFAULT_SOURCE_CONTAINER = "simulations";
        static string   DEFAULT_TARGET_CONTAINER = "processed";
        static int      DEFAULT_RANDOMNESS = 5;  // a percentage, 5% of csv rows read
        static Random   random = new Random();
        static DateTime epochDateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        static string runtype = null;
        static string sourceBlobContainerName = null;
        static string targetBlobContainerName = null;
        static string processedBlobContainerName = null;
        static string blobConnectionString = null;

        static BlobServiceClient   blobServiceClient = null;  // in Azure.Storage.Blobs
        static BlobContainerClient sourceBlobContainerClient = null;
        static BlobContainerClient targetBlobContainerClient = null;

        static void Main(string[] args) {
            start();
            switch (runtype) {
                case "populate_storage_blobs":
                    populateStorageBlobs();
                    break;
                default:
                    logicAppProcessBlob();
                    break;
            }
        }

        static void start() {
            try {
                displayEnvVars();
                runtype = EnvVar("RUNTYPE", "undefined");
                Console.WriteLine("start() runtype is: {0}", runtype);
                sourceBlobContainerName = getSourceBlobContainerName();
                targetBlobContainerName = EnvVar("TARGET_BLOB_CONTAINER", DEFAULT_TARGET_CONTAINER);
                blobConnectionString = EnvVar("AZURE_STORAGE_CONNECTION_STRING");
                Console.WriteLine("sourceBlobContainerName: {0}", sourceBlobContainerName);
                Console.WriteLine("targetBlobContainerName: {0}", targetBlobContainerName);
                Console.WriteLine("blobConnectionString:    {0}", blobConnectionString);

                blobServiceClient = new BlobServiceClient(blobConnectionString);
                sourceBlobContainerClient = new BlobContainerClient(blobConnectionString, sourceBlobContainerName);
                targetBlobContainerClient = new BlobContainerClient(blobConnectionString, targetBlobContainerName);
                Console.WriteLine("sourceBlobContainerClient: {0}", sourceBlobContainerClient);
                Console.WriteLine("targetBlobContainerClient: {0}", targetBlobContainerClient);
            }
            catch (Exception e) {
                Console.WriteLine("Exception in start(): {0}", e);
            }
        }

        static void populateStorageBlobs() {
            string[] csvLines = readCsvDataFile();
            int randomness = populateStorageRandomness();
            Console.WriteLine("populateStorageBlobs(), randomness: {0}", randomness);

            for (int i = 0; i < csvLines.Length; i++) {
                if (i > 0) {
                    string line = csvLines[i];
                    string[] tokens = line.Split('|');
                    if (tokens.Length == 16) {
                        int x = random.Next(100);
                        if (x < randomness) {
                            //Console.WriteLine("x: {0}, randomness: {1}", x, randomness);
                            string runnerName = tokens[2].Replace(' ', '_').ToLower();
                            string blobName = (runnerName + "_" + epochTime() + ".csv");
                            Console.WriteLine("{0} -> {1}", blobName, line);
                            sourceBlobContainerClient.UploadBlob(
                                blobName, generateStreamFromString(line));
                        }
                    }
                }
            }
        }

        public static Stream generateStreamFromString(string s) {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        static long epochTime() {
            return (long)(DateTime.UtcNow - epochDateTime).TotalSeconds;
        }

        static int populateStorageRandomness() {
            try {
                return int.Parse(EnvVar("POPULATE_STORAGE_RANDOMNESS", "" + DEFAULT_RANDOMNESS));
            }
            catch (Exception e) {
                Console.WriteLine("Exception in populateStorageRandomness(): {0}", e);
                return DEFAULT_RANDOMNESS;
            }
        }

        static string[] readCsvDataFile() {
            string dir = Directory.GetCurrentDirectory();
            string infile = dir + "/data/20190317-asheville-marathon.csv";
            Console.WriteLine("readCsvDataFile: {0}", infile);
            return System.IO.File.ReadAllLines(infile);
        }

        static void logicAppProcessBlob() {
            processedBlobContainerName = EnvVar("processedBlobContainerName", DEFAULT_TARGET_CONTAINER);

            Console.WriteLine("logicAppProcessBlob() finish, sleep for: {0}", TEN_SECONDS);
            sleep(TEN_SECONDS);
        }

        static string getSourceBlobContainerName() {
            char[] charSeparators = new char[] { '/' };
            string path = EnvVar("SOURCE_BLOB_PATH");
            if (path == null) {
                return DEFAULT_SOURCE_CONTAINER;
            }
            else {
                return path.Split(charSeparators)[1];
            }
        }

        static void sleep(int milliseconds) {
            System.Threading.Thread.Sleep(milliseconds);
        }

        static void displayEnvVars() {
            foreach (DictionaryEntry de in Environment.GetEnvironmentVariables()) {
                Console.WriteLine("EnvVar: {0} = {1}", de.Key, de.Value);
            }
        }

        static string EnvVar(string name, string defaultValue=null) {
            if (name != null) {
                String value = Environment.GetEnvironmentVariable(name);
                if (value == null) {
                    return defaultValue;
                }
                else {
                    return value;
                }
            }
            else {
                return defaultValue;
            }
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
