using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace LameScooter {
    public class OfflineLameScooterRental : IRental  {
        
        public OfflineLameScooterRental(string uri) {
            Console.WriteLine($"Loading from: {uri}");
            if (!File.Exists(uri)){
                Console.WriteLine("no file in path");
                return;
            }

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            
            var jsonString = File.ReadAllText(uri);
            var list = JsonSerializer.Deserialize<List<LameScooterStationList>>(jsonString, options);

            foreach (var a in list) {
                Console.WriteLine(a);
            }
            
            // async version:
            //using FileStream openStream = File.OpenRead(fileName);
            //weatherForecast = await JsonSerializer.DeserializeAsync<WeatherForecast>(openStream, options);
        }

    }
}