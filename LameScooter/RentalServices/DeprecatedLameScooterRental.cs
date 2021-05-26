using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LameScooter.RentalServices {
    public class DeprecatedLameScooterRental : IRentalAsync  {
        private Dictionary<string, LameScooterStationList> stationLookup = null;

        public void Init(string uri) {
            InitAsync(uri).GetAwaiter().GetResult();
        }

        public async Task InitAsync(string uri) {
            stationLookup = new Dictionary<string, LameScooterStationList>();
            Console.WriteLine($"Loading from: {uri}");
            if (!File.Exists(uri)){
                Console.WriteLine("no file in path");
                return;
            }
            
            List<(string, int)> list;
            using (StreamReader sr = new StreamReader(uri)) {
                string line;
                while (sr.Peek() >= 0) {
                    line = sr.ReadLine();
                    int index = line.IndexOf(':');
                    string station = line.Substring(0, index - 1);
                    int bikesAvailable = int.Parse(line.Substring(index + 2));
                    stationLookup[station] = new LameScooterStationList(station, bikesAvailable);
                }
            }
        }

        public int GetScooterCountInStation(string nameOfStation) {
            if (ContainsDigit(nameOfStation))
                throw new System.ArgumentException($"{nameOfStation} contains numerals");

            if (stationLookup.TryGetValue(nameOfStation, out LameScooterStationList val)) {
                return val.bikesAvailable;    
            }
            throw new NotFoundException($"{nameOfStation} not found");
        }
        
        private static bool ContainsDigit(string s) {
            return s.Any(c => char.IsDigit(c));
        }
    }
}