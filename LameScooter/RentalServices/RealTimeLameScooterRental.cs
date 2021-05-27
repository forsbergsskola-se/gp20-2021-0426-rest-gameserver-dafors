using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using LameScooter.CustomExceptions;
using LameScooter.JSonTemplates;

namespace LameScooter.RentalServices {
    public class RealTimeLameScooterRental : IRentalAsync  {
        private Dictionary<string, LameScooterStation> stationLookup = null;
        static readonly HttpClient client = new HttpClient();
        private const string uri = "https://raw.githubusercontent.com/marczaku/GP20-2021-0426-Rest-Gameserver/main/assignments/scooters.json";
        public void Init() {
            InitAsync().GetAwaiter().GetResult();
        }

        public async Task InitAsync() {
            Console.WriteLine($"Loading from: {uri}");
            List<LameScooterStation> list = null;
            try	
            {
                list = (await client.GetFromJsonAsync<LameScooterContainer>(uri))?.Stations;
            }
            catch(HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");	
                Console.WriteLine("Message :{0} ",e.Message);
            }

            if (list == null)
                return;
            
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