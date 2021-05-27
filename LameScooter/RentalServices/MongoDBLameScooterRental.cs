using System;
using System.Linq;
using LameScooter.CustomExceptions;
using LameScooter.JSonTemplates;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace LameScooter.RentalServices {
    public class MongoDBLameScooterRental : IRental {
        private MongoClient client;
        private IMongoDatabase db;
        private IMongoCollection<LameScooterStation> dbCollection;

        public void Init() {
            IConfigurationRoot config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
            IConfigurationProvider secretProvider = config.Providers.First();
            if (!secretProvider.TryGet("AtlasPwd", out string pwd)) {
                Console.WriteLine("Password for Mongo db could not be located");
                return;
            }
            client = new MongoClient($"mongodb+srv://dafors:{pwd}@cluster0.nleqv.mongodb.net/lame?retryWrites=true&w=majority");
            db = client.GetDatabase("lame");
            dbCollection = db.GetCollection<LameScooterStation>("stations");
        }

        public int GetScooterCountInStation(string nameOfStation) {
            if (ContainsDigit(nameOfStation))
                throw new System.ArgumentException($"{nameOfStation} contains numerals");
            FilterDefinitionBuilder<LameScooterStation> filter = Builders<LameScooterStation>.Filter;
            FilterDefinition<LameScooterStation> eqFilter = filter.Eq(x => x.name, nameOfStation);
            var asyncCursor = dbCollection.FindSync<LameScooterStation>(eqFilter);
            LameScooterStation station = asyncCursor.FirstOrDefault();
            if (station == default)
                throw new NotFoundException($"{nameOfStation} not found");
            return station.bikesAvailable;
        }
        
        private static bool ContainsDigit(string s) {
            return s.Any(c => char.IsDigit(c));
        }

        //only used once to populate the database. Saved for future reference.
        // private async Task PopulateMongoDB() {
        //     string uri = "Data/scooters.json";
        //     
        //     Console.WriteLine($"Loading from: {uri}");
        //     if (!File.Exists(uri)){
        //         Console.WriteLine("no file in path");
        //         return;
        //     }
        //     var options = new JsonSerializerOptions
        //     {
        //         PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        //     };
        //     
        //     await using FileStream openStream = File.OpenRead(uri);
        //     var list = await JsonSerializer.DeserializeAsync<List<LameScooterStation>>(openStream, options);
        //     await dbCollection.InsertManyAsync(list);
        // }
    }
}