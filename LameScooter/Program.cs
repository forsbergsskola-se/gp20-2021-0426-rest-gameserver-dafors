using System;

namespace LameScooter
{
    class Program {
        private const string localJsonPath = "scooters.json";
        static void Main(string[] args)
        {
            
            Console.WriteLine(args[0]);
            //IRental rental = new OfflineLameScooterRental(localJsonPath);
            //IRental rental = new OfflineLameScooterRental();
            IRentalAsync rental = new OfflineLameScooterRental();
            //rental.Init(localJsonPath);
            rental.InitAsync(localJsonPath).GetAwaiter().GetResult();
            foreach (var arg in args) {
                Console.WriteLine($"station {arg} has {rental.GetScooterCount(arg)}");
            }
            
            //Address address = JsonConvert.DeserializeObject<Address>(json);
            
            //var count = await rental.GetScooterCountInStation(args[0]); // Replace with command line argument.
        }
    }
}
