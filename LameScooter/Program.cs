using System;

namespace LameScooter
{
    class Program {
        private const string localJsonPath = "scooters.json";
        // static async Task Main(string[] args)
        static void Main(string[] args)
        {
            //IRental rental = new OfflineLameScooterRental(localJsonPath);
            //IRental rental = new OfflineLameScooterRental();
            IRentalAsync rental = new OfflineLameScooterRental();
            //rental.Init(localJsonPath);
            rental.InitAsync(localJsonPath).GetAwaiter().GetResult();
            foreach (var arg in args) {
                Console.WriteLine($"station {arg} has {rental.GetScooterCountInStation(arg)}");
            }
            
            //Address address = JsonConvert.DeserializeObject<Address>(json);
            
            //var count = await rental.GetScooterCountInStation(args[0]); // Replace with command line argument.
        }
    }
}
