using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System;
using System.IO;

namespace blobs
{
    // Instances of this class execute one of the following main functions:
    // 1) It is used to populate the triggering storage container with CSV data from 
    //    randomly selected rows in the 'data/20190317-asheville-marathon.csv' file.
    //    This is done to trigger the Azure Logic App.
    // 2) Is is also the logic in the Azure Container Instance (ACI) that is executed
    //    by the Azure Logic app, as a result of the Logic App being triggered by a
    //    new csv-blob in the Azure Storage container.  This logic will parse the csv,
    //    execute M26-library pace and speed calculations, and store the resulting JSON
    //    object in the target Storage container.
    //
    // Chris Joakim, Microsoft, 2020/02/28

    class Program
    {
        static Random random = new Random();
        static DateTime epochDateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        static string runtype = null;
        static string sourceBlobContainerName = null;
        static string targetBlobContainerName = null;
        static string storageConnectionString = null;

        static BlobServiceClient blobServiceClient = null;  // in Azure.Storage.Blobs
        static BlobContainerClient sourceBlobContainerClient = null;
        static BlobContainerClient targetBlobContainerClient = null;

        static void Main(string[] args)
        {
            start();
            switch (runtype)
            {
                case "env":
                    break;
                case "populate_storage_blobs":
                    populateStorageBlobs();
                    break;
                case "delete_source_blobs":
                    deleteSourceBlobs();
                    break;
                case "download_target_blobs":
                    downloadTargetBlobs();
                    break;
                default:
                    logicAppProcessBlob();
                    break;
            }
        }

        static void start()
        {
            try
            {
                EnvVars.displayAll();
                runtype = EnvVars.value(Constants.RUNTYPE, "undefined");
                Console.WriteLine("start() runtype is: {0}", runtype);
                sourceBlobContainerName = getSourceBlobContainerName();
                targetBlobContainerName = EnvVars.value(Constants.TARGET_BLOB_CONTAINER, Constants.DEFAULT_TARGET_CONTAINER);
                storageConnectionString = EnvVars.value(Constants.AZURE_STORAGE_CONNECTION_STRING);

                Console.WriteLine("sourceBlobContainerName: {0}", sourceBlobContainerName);
                Console.WriteLine("targetBlobContainerName: {0}", targetBlobContainerName);
                Console.WriteLine("storageConnectionString: {0}", storageConnectionString);

                blobServiceClient = new BlobServiceClient(storageConnectionString);
                sourceBlobContainerClient = new BlobContainerClient(storageConnectionString, sourceBlobContainerName);
                targetBlobContainerClient = new BlobContainerClient(storageConnectionString, targetBlobContainerName);
                Console.WriteLine("sourceBlobContainerClient: {0}", sourceBlobContainerClient);
                Console.WriteLine("targetBlobContainerClient: {0}", targetBlobContainerClient);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception in start(): {0}", e);
            }
        }

        static void populateStorageBlobs()
        {
            string[] csvLines = readCsvDataFile();
            int randomness = populateStorageRandomness();
            int maxCount = EnvVars.intValue(Constants.POPULATE_STORAGE_MAX_COUNT, 999);
            int sleepMs = EnvVars.intValue(Constants.POPULATE_STORAGE_DELAY, Constants.DEFAULT_POPULATE_STORAGE_DELAY);
            int actualCount = 0;
            Console.WriteLine("populateStorageBlobs(), randomness: {0}", randomness);
            Console.WriteLine("populateStorageBlobs(), maxCount: {0}", maxCount);
            Console.WriteLine("populateStorageBlobs(), sleepMs: {0}", sleepMs);

            for (int i = 1; i < csvLines.Length; i++)
            {
                if (actualCount < maxCount)
                {
                    string line = csvLines[i];
                    string[] tokens = line.Split('|');
                    if (tokens.Length == 16)
                    {
                        int x = random.Next(100);
                        if (x < randomness)
                        {
                            string runnerName = tokens[2].Replace(' ', '_').ToLower();
                            string blobName = (runnerName + "_" + epochTime() + ".csv");
                            Console.WriteLine("{0} -> {1}", blobName, line);
                            sourceBlobContainerClient.UploadBlob(
                                blobName, generateStreamFromString(line));
                            actualCount++;
                            sleep(sleepMs);
                        }
                    }
                }
            }
            Console.WriteLine("populateStorageBlobs; blobs written this run: {0}", actualCount);
        }

        public static Stream generateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        static long epochTime()
        {
            return (long)(DateTime.UtcNow - epochDateTime).TotalSeconds;
        }

        static int populateStorageRandomness()
        {
            try
            {
                return EnvVars.intValue(
                        Constants.POPULATE_STORAGE_RANDOM_PCT,
                        Constants.DEFAULT_POPULATE_STORAGE_RANDOM_PCT);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception in populateStorageRandomness(): {0}", e);
                return Constants.DEFAULT_POPULATE_STORAGE_RANDOM_PCT;
            }
        }

        static string[] readCsvDataFile()
        {
            string dir = Directory.GetCurrentDirectory();
            string infile = dir + "/data/20190317-asheville-marathon.csv";
            Console.WriteLine("readCsvDataFile: {0}", infile);
            return System.IO.File.ReadAllLines(infile);
        }

        static void logicAppProcessBlob()
        {
            string sourceBlobName = EnvVars.value(Constants.SOURCE_BLOB_NAME);
            if (sourceBlobName != null)
            {
                Console.WriteLine("logicAppProcessBlob: {0} in {1}", sourceBlobName, sourceBlobContainerName);
                BlobClient sourceBlob = sourceBlobContainerClient.GetBlobClient(sourceBlobName);

                string content = null;
                using (var memoryStream = new MemoryStream())
                {
                    sourceBlob.DownloadTo(memoryStream);
                    content = System.Text.Encoding.UTF8.GetString(memoryStream.ToArray());
                }

                if (content != null)
                {
                    Console.WriteLine("downloaded blob content: {0}", content);
                    char[] charSeparators = new char[] { '|' };
                    string path = EnvVars.value(Constants.SOURCE_BLOB_PATH);
                    string[] tokens = content.Split("|");
                    if (tokens.Length == 16) {
                        Calculation c = new Calculation(content);
                        string json = c.toJSON();
                        string targetBlobName = sourceBlobName + ".json";
                        Console.WriteLine("Calculation json:\n{0}", json);

                        // Save the result of the calculation to a blob in the target container
                        targetBlobContainerClient.UploadBlob(
                            targetBlobName, generateStreamFromString(json));
                        Console.WriteLine("Target blob written: {0} {1}", targetBlobContainerName, targetBlobName);

                        // Cleanup; delete the source blob from the source container
                        sourceBlobContainerClient.DeleteBlob(sourceBlobName);
                        Console.WriteLine("Source blob deleted: {0} {1}", sourceBlobContainerName, sourceBlobName);
                    }
                }
            }

            Console.WriteLine("logicAppProcessBlob() finish, sleep for: {0}", Constants.LOGIC_APP_SHUTDOWN_DELAY_MS);
            sleep(Constants.LOGIC_APP_SHUTDOWN_DELAY_MS);
        }

        static int toInt(string s) {
            return (int) decimal.Parse(s);
        }

        static string getSourceBlobContainerName()
        {
            char[] charSeparators = new char[] { '/' };
            string path = EnvVars.value(Constants.SOURCE_BLOB_PATH);
            if (path == null)
            {
                return Constants.DEFAULT_SOURCE_CONTAINER;
            }
            else
            {
                return path.Split(charSeparators)[1];
            }
        }

        static void sleep(int milliseconds)
        {
            System.Threading.Thread.Sleep(milliseconds);
        }

        static void deleteSourceBlobs()
        {
            Console.WriteLine("deleteSourceBlobs");

            foreach(BlobItem blob in sourceBlobContainerClient.GetBlobs())
            {
                sourceBlobContainerClient.DeleteBlob(blob.Name);
                Console.WriteLine("Source blob deleted: {0} {1}", sourceBlobContainerName, blob.Name);
            }
        }

        static void downloadTargetBlobs()
        {
            Console.WriteLine("downloadTargetBlobs");
            string dir = Directory.GetCurrentDirectory();
            int delete = EnvVars.intValue(Constants.DELETE_AFTER_DOWNLOAD, 0);

            foreach (BlobItem blobItem in targetBlobContainerClient.GetBlobs())
            {
                string blobname = blobItem.Name;
                string outfile = dir + "/tmp/" + blobname;
                Console.WriteLine("downloading: {0} -> {1}", blobname, outfile);

                BlobClient blobClient = targetBlobContainerClient.GetBlobClient(blobname);
                BlobDownloadInfo download = blobClient.Download();
                using (FileStream file = File.OpenWrite(outfile))
                {
                    download.Content.CopyTo(file);
                    if (delete > 0)
                    {
                        Console.WriteLine("deleting: {0}", blobname);
                        targetBlobContainerClient.DeleteBlob(blobname);
                    }
                }
            }
        }
    }
}
