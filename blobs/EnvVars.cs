using System;
using System.Collections;

namespace blobs
{
    static class EnvVars
    {
        public static string value(string name, string defaultValue = null)
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

        public static int intValue(string name, int defaultValue = 0)
        {
            return int.Parse(value(name, "" + defaultValue));
        }

        public static int intValue(string name, string defaultValue = "0")
        {
            return int.Parse(value(name, defaultValue));
        }

        public static void displayAll()
        {
            foreach (DictionaryEntry de in Environment.GetEnvironmentVariables())
            {
                Console.WriteLine("EnvVar: {0} = {1}", de.Key, de.Value);
            }
        }
    }
}
