using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace LameScooter {
    public class OfflineLameScooterRental : IRentalAsync  {
        //List<LameScooterStationList> list = new List<LameScooterStationList>();
        private Dictionary<string, LameScooterStationList> stationLookup = null;
        // public OfflineLameScooterRental(string uri) {
        //     Console.WriteLine($"Loading from: {uri}");
        //     if (!File.Exists(uri)){
        //         Console.WriteLine("no file in path");
        //         return;
        //     }
        //
        //     var options = new JsonSerializerOptions
        //     {
        //         PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        //     };
        //     
        //     var jsonString = File.ReadAllText(uri);
        //     var list = JsonSerializer.Deserialize<List<LameScooterStationList>>(jsonString, options);
        //
        //     foreach (var a in list) {
        //         Console.WriteLine(a);
        //     }
        //     
        //     // async version:
        //     //using FileStream openStream = File.OpenRead(fileName);
        //     //weatherForecast = await JsonSerializer.DeserializeAsync<WeatherForecast>(openStream, options);
        // }

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
            
            using FileStream openStream = File.OpenRead(uri);
            var list = await JsonSerializer.DeserializeAsync<List<LameScooterStationList>>(openStream, options);

            stationLookup = new Dictionary<string, LameScooterStationList>();
            foreach (var station in list) {
                stationLookup[station.name] = station;
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