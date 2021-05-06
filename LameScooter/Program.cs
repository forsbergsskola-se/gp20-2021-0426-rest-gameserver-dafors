using System;

namespace LameScooter
{
    class Program {
        private const string localJsonPath = "scooters.json";
        static void Main(string[] args)
        {
            
            Console.WriteLine(args[0]);
            IRental rental = new OfflineLameScooterRental(localJsonPath);
            
            //Address address = JsonConvert.DeserializeObject<Address>(json);
            
            //var count = await rental.GetScooterCountInStation(args[0]); // Replace with command line argument.
            Console.WriteLine("Number of Scooters Available at this Station: "); // Add the count that is returned above to the output.
        }
    }
}
