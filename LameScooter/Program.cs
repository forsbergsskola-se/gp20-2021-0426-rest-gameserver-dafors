﻿using System;
namespace LameScooter
{
    class Program {
        
        // static async Task Main(string[] args)
        static void Main(string[] args)
        {
            string stationToQuery = args[0];
            string rentalDatabase = args[1];

            string fileUri;
            IRentalAsync rental;
            switch (rentalDatabase) {
                case "offline":
                    rental = new OfflineLameScooterRental();
                    fileUri = "scooters.json";
                    break;
                case "deprecated":
                    rental = new DeprecatedLameScooterRental();
                    fileUri = "scooters.txt";
                    break;
                default:
                    Console.WriteLine($"Database {rentalDatabase} not found, using 'offline' to lookup query");
                    rental = new OfflineLameScooterRental();
                    fileUri = "scooters.json";
                    break;
            }
            
            rental.InitAsync(fileUri).GetAwaiter().GetResult();

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
            
            //var count = await rental.GetScooterCountInStation(args[0]); // Replace with command line argument.
        }
        
    }
}
