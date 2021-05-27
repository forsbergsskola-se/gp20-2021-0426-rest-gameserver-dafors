using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using LameScooter.CustomExceptions;
using LameScooter.JSonTemplates;

namespace LameScooter.RentalServices {
    public class OfflineLameScooterRental : IRentalAsync  {
        private Dictionary<string, LameScooterStation> stationLookup = null;
        public void Init(string uri) {
            InitAsync(uri).GetAwaiter().GetResult();
        }

        public async Task InitAsync(string uri) {
            
            Console.WriteLine($"Loading from: {uri}");
            if (!File.Exists(uri)){
                Console.WriteLine("no file in path");
                return;
            }
            
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            
            await using FileStream openStream = File.OpenRead(uri);
            var list = await JsonSerializer.DeserializeAsync<List<LameScooterStation>>(openStream, options);

            stationLookup = new Dictionary<string, LameScooterStation>();
            foreach (var station in list) {
                stationLookup[station.name] = station;
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