using System;

namespace blobs
{
    static class Constants
    {
        // Environment variable names:
        public const string RUNTYPE = "RUNTYPE";
        public const string AZURE_STORAGE_CONNECTION_STRING = "AZURE_STORAGE_CONNECTION_STRING";
        public const string TARGET_BLOB_CONTAINER = "TARGET_BLOB_CONTAINER";
        public const string POPULATE_STORAGE_RANDOM_PCT = "POPULATE_STORAGE_RANDOM_PCT";
        public const string POPULATE_STORAGE_DELAY = "POPULATE_STORAGE_DELAY";
        public const string POPULATE_STORAGE_MAX_COUNT = "POPULATE_STORAGE_MAX_COUNT";
        public const string SOURCE_BLOB_NAME = "SOURCE_BLOB_NAME";
        public const string SOURCE_BLOB_PATH = "SOURCE_BLOB_PATH";

        // Default and other values:
        public const string APP_VERSION = "2020/02/26 18:00";
        public const string DEFAULT_SOURCE_CONTAINER = "simulations";
        public const string DEFAULT_TARGET_CONTAINER = "processed";
        public const int    DEFAULT_POPULATE_STORAGE_RANDOM_PCT = 5;  // randomly choose 5% of the csv rows read
        public const int    DEFAULT_POPULATE_STORAGE_DELAY = 3;  // delay between blob writes
        public const int    LOGIC_APP_SHUTDOWN_DELAY_MS = 10000;
    }
}
