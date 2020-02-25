
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System;
using System.Collections;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Joakimsoftware.M26;


namespace blobs
{

    class Program
    {
        static int TEN_SECONDS = 10000;
        static string DEFAULT_SOURCE_CONTAINER = "simulations";
        static string DEFAULT_TARGET_CONTAINER = "processed";
        static int DEFAULT_RANDOMNESS = 5;  // a percentage, 5% of csv rows read
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
                default:
                    logicAppProcessBlob();
                    break;
            }
        }

        static void start()
        {
            try
            {
                displayEnvVars();
                runtype = EnvVar("RUNTYPE", "undefined");
                Console.WriteLine("start() runtype is: {0}", runtype);
                sourceBlobContainerName = getSourceBlobContainerName();
                targetBlobContainerName = EnvVar("TARGET_BLOB_CONTAINER", DEFAULT_TARGET_CONTAINER);
                storageConnectionString = EnvVar("AZURE_STORAGE_CONNECTION_STRING");
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
            Console.WriteLine("populateStorageBlobs(), randomness: {0}", randomness);

            for (int i = 0; i < csvLines.Length; i++)
            {
                if (i > 0)
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
                        }
                    }
                }
            }
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
                return int.Parse(EnvVar("POPULATE_STORAGE_RANDOMNESS", "" + DEFAULT_RANDOMNESS));
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception in populateStorageRandomness(): {0}", e);
                return DEFAULT_RANDOMNESS;
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
            string sourceBlobName = EnvVar("SOURCE_BLOB_NAME");
            if (sourceBlobName != null)
            {
                Console.WriteLine("logicAppProcessBlob: {0} in {1}", sourceBlobName, sourceBlobContainerName);
                BlobClient blob = sourceBlobContainerClient.GetBlobClient(sourceBlobName);

                string content = null;
                using (var memoryStream = new MemoryStream())
                {
                    blob.DownloadTo(memoryStream);
                    content = System.Text.Encoding.UTF8.GetString(memoryStream.ToArray());
                }

                if (content != null)
                {
                    Console.WriteLine("downloaded blob content: {0}", content);
                    char[] charSeparators = new char[] { '|' };
                    string path = EnvVar("SOURCE_BLOB_PATH");
                    string[] tokens = content.Split("|");
                    if (tokens.Length == 16) {
                        //   0          1       2     3      4   5     6        7            8            9        10       11        12           13         14       15
                        // place,overall_place,name,city_st,bib,age,10K_rank,10K_time,bridge1_rank,bridge1_time,23m_rank,23m_time,finish_rank,finish_time,chip_time,gun_time

                        Calculation c = new Calculation(content);
                        Console.WriteLine("json:\n{0}", c.toJSON());

                        //string name = tokens[2];
                        //string[] gunTimeTokens = tokens[15].Split(":");
                        //Distance d = new Distance(26.2);
                        //ElapsedTime et = new ElapsedTime(
                        //    int.Parse(gunTimeTokens[0]),
                        //    int.Parse(gunTimeTokens[1]),
                        //    (int)decimal.Parse(gunTimeTokens[2]));

                        //Speed sp = new Speed(d, et);
                        //Console.WriteLine("name: {0}", name);
                        //double mph = sp.mph();
                        //double kph = sp.kph();
                        //double yph = sp.yph();
                        //double spm = sp.secondsPerMile();
                        //string ppm = sp.pacePerMile();
                        //Console.WriteLine($"Speed - mph:             {mph}");
                        //Console.WriteLine($"Speed - kph:             {kph}");
                        //Console.WriteLine($"Speed - yph:             {yph}");
                        //Console.WriteLine($"Speed - secondsPerMile:  {spm}");
                        //Console.WriteLine($"Speed - pacePerMile:     {ppm}");

                    }
                }
            }



            Console.WriteLine("logicAppProcessBlob() finish, sleep for: {0}", TEN_SECONDS);
            sleep(TEN_SECONDS);
        }

        static int toInt(string s) {
            return (int)decimal.Parse(s);
        }

        static string getSourceBlobContainerName()
        {
            char[] charSeparators = new char[] { '/' };
            string path = EnvVar("SOURCE_BLOB_PATH");
            if (path == null)
            {
                return DEFAULT_SOURCE_CONTAINER;
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

        static void displayEnvVars()
        {
            foreach (DictionaryEntry de in Environment.GetEnvironmentVariables())
            {
                Console.WriteLine("EnvVar: {0} = {1}", de.Key, de.Value);
            }
        }

        static string EnvVar(string name, string defaultValue = null)
        {
            if (name != null)
            {
                String value = Environment.GetEnvironmentVariable(name);
                if (value == null)
                {
                    return defaultValue;
                }
                else
                {
                    return value;
                }
            }
            else
            {
                return defaultValue;
            }
        }
    }

    class Calculation {

        // 16 input csv fields:
        public int    place { get; set; }
        public int    overallPlace { get; set; }
        public string name { get; set; }
        public string citySt { get; set; }
        public int    bib { get; set; }
        public int    age { get; set; }
        public int    rank10K { get; set; }
        public string time10K { get; set; }
        public int    rankBridge { get; set; }
        public string timeBridge { get; set; }
        public int    rank23M { get; set; }
        public string time23M { get; set; }
        public int    finishRank { get; set; }
        public string finishTime { get; set; }
        public string chipTime { get; set; }
        public string gunTime { get; set; }

        // additional calculated fields:
        public double miles { get; set; }

        public Calculation(string csvLine) {
            string[] tokens = csvLine.Split("|");
            if (tokens.Length == 16) {
                place        = toInt(tokens[0]);
                overallPlace = toInt(tokens[1]);
                name         = tokens[2];
                citySt       = tokens[3];
                bib          = toInt(tokens[4]);
                age          = toInt(tokens[5]);
                rank10K      = toInt(tokens[6]);
                time10K      = tokens[7];
                rankBridge   = toInt(tokens[8]);
                timeBridge   = tokens[9];
                rank23M      = toInt(tokens[10]);
                time23M      = tokens[11];
                finishRank   = toInt(tokens[12]);
                finishTime   = tokens[13];
                chipTime     = tokens[14];
                gunTime      = tokens[15];

                //string name = tokens[2];
                string[] gunTimeTokens = tokens[15].Split(":");
                Distance d = new Distance(26.2);
                this.miles = d.asMiles();

                //ElapsedTime et = new ElapsedTime(
                //    int.Parse(gunTimeTokens[0]),
                //    int.Parse(gunTimeTokens[1]),
                //    (int)decimal.Parse(gunTimeTokens[2]));

                //Speed sp = new Speed(d, et);
                //Console.WriteLine("name: {0}", name);
                //double mph = sp.mph();
                //double kph = sp.kph();
                //double yph = sp.yph();
                //double spm = sp.secondsPerMile();
                //string ppm = sp.pacePerMile();
                //Console.WriteLine($"Speed - mph:             {mph}");
                //Console.WriteLine($"Speed - kph:             {kph}");
                //Console.WriteLine($"Speed - yph:             {yph}");
                //Console.WriteLine($"Speed - secondsPerMile:  {spm}");
                //Console.WriteLine($"Speed - pacePerMile:     {ppm}");

            }
        }

        int toInt(string s) {
            return (int)decimal.Parse(s);
        }

        public string toJSON() {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            return JsonSerializer.Serialize<Calculation>(this, options);
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
