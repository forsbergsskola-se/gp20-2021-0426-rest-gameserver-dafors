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
                try {
                    Console.WriteLine($"station {arg} has {rental.GetScooterCountInStation(arg)}");
                }
                catch (NotFoundException e) {
                    Console.WriteLine("An exception ({0}) occurred.", e.GetType().Name);
                    Console.WriteLine("Message:\n   {0}\n", e.Message);
                    Console.WriteLine("Stack Trace:\n   {0}\n", e.StackTrace);
                }
                catch (ArgumentException e) {
                    Console.WriteLine("An exception ({0}) occurred.", e.GetType().Name);
                    Console.WriteLine("Message:\n   {0}\n", e.Message);
                    Console.WriteLine("Stack Trace:\n   {0}\n", e.StackTrace);
                }
            }
            
            //Address address = JsonConvert.DeserializeObject<Address>(json);
            
            //var count = await rental.GetScooterCountInStation(args[0]); // Replace with command line argument.
        }
    }
}
