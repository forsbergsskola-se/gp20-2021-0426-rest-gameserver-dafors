using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace LameScooter {
    public class RealTimeLameScooterRental : IRentalAsync  {
        private Dictionary<string, LameScooterStationList> stationLookup = null;
        static readonly HttpClient client = new HttpClient();

        public void Init(string uri) {
            InitAsync(uri).GetAwaiter().GetResult();
        }

        public async Task InitAsync(string uri) {
            Console.WriteLine($"Loading from: {uri}");
            
            List<LameScooterStationList> list = null;
            try	
            {
                list = await client.GetFromJsonAsync<List<LameScooterStationList>>(uri);
            }
            catch(HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");	
                Console.WriteLine("Message :{0} ",e.Message);
            }

            if (list == null)
                return;
            
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