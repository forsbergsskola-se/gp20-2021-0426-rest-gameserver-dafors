using System;
using LameScooter.CustomExceptions;
using LameScooter.RentalServices;

namespace LameScooter
{
    class Program {
        static void Main(string[] args)
        {
            string stationToQuery = args[0];
            string rentalDatabase = args[1];

            IRental rental;
            switch (rentalDatabase) {
                case "realtime":
                    rental = new RealTimeLameScooterRental();
                    break;
                case "offline":
                    rental = new OfflineLameScooterRental();
                    break;
                case "deprecated":
                    rental = new DeprecatedLameScooterRental();
                    break;
                case "mongodb":
                    rental = new MongoDBLameScooterRental();
                    break;
                default:
                    Console.WriteLine($"Database {rentalDatabase} not found, using 'offline' to lookup query");
                    rental = new OfflineLameScooterRental();
                    break;
            }
            rental.Init();

            try {
                Console.WriteLine($"station {stationToQuery} has {rental.GetScooterCountInStation(stationToQuery)}");
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
    }
}
