using System.Text.Json;
using Joakimsoftware.M26;

namespace blobs
{
    // This is a POCO class that executes the M26-based Pace and Speed calculations
    // of the given CSV Blob data.  The return 'toJSON()' value is populated in the
    // target storage container.
    // Chris Joakim, Microsoft, 2020/02/28

    class Calculation {

        // raw csv line and its' 16 fields:
        public string rawCsv { get; set; }
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
        public string appVersion { get; set; }
        public double miles { get; set; }
        public string elapsedTime { get; set; }
        public string pacePerMile { get; set; }
        public double mph { get; set; }
        public double kph { get; set; }
        public double secondsPerMile { get; set; }

        public Calculation(string csvLine) {
            this.rawCsv = csvLine;
            string[] tokens = this.rawCsv.Split("|");
            if (tokens.Length == 16) {
                // 16 input csv fields:
                place = toInt(tokens[0]);
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

                // additional calculated fields:
                this.appVersion = Constants.APP_VERSION;
                Distance d = new Distance(26.2);
                this.miles = d.asMiles();

                string[] gunTimeTokens = tokens[15].Split(":");
                ElapsedTime et = new ElapsedTime(
                    int.Parse(gunTimeTokens[0]),
                    int.Parse(gunTimeTokens[1]),
                    (int)decimal.Parse(gunTimeTokens[2]));
                this.elapsedTime = et.asHHMMSS();

                Speed speed = new Speed(d, et);
                this.pacePerMile = speed.pacePerMile();
                this.mph = speed.mph();
                this.kph = speed.kph();
                this.secondsPerMile = speed.secondsPerMile();
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
