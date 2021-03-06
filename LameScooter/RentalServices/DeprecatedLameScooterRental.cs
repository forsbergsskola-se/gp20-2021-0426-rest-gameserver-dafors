using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LameScooter.CustomExceptions;
using LameScooter.JSonTemplates;

namespace LameScooter.RentalServices {
    public class DeprecatedLameScooterRental : IRental  {
        private Dictionary<string, LameScooterStation> stationLookup = null;
        private const string uri = "Data/scooters.txt";
        public void Init() {
            stationLookup = new Dictionary<string, LameScooterStation>();
            Console.WriteLine($"Loading from: {uri}");
            if (!File.Exists(uri)){
                Console.WriteLine("no file in path");
                return;
            }
            
            using (StreamReader sr = new StreamReader(uri)) {
                string line;
                while (sr.Peek() >= 0) {
                    line = sr.ReadLine();
                    int index = line.IndexOf(':');
                    string station = line.Substring(0, index - 1);
                    int bikesAvailable = int.Parse(line.Substring(index + 2));
                    stationLookup[station] = new LameScooterStation(station, bikesAvailable);
                }
            }
        }

        public int GetScooterCountInStation(string nameOfStation) {
            if (ContainsDigit(nameOfStation))
                throw new System.ArgumentException($"{nameOfStation} contains numerals");

            if (stationLookup.TryGetValue(nameOfStation, out LameScooterStation val)) {
                return val.bikesAvailable;    
            }
            throw new NotFoundException($"{nameOfStation} not found");
        }
        
        private static bool ContainsDigit(string s) {
            return s.Any(c => char.IsDigit(c));
        }
    }
}