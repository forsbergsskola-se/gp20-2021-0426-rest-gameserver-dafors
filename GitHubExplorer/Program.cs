// microsoft docs used for this repo: https://docs.microsoft.com/en-us/dotnet/csharp/tutorials/console-webapiclient
using System;
using System.Threading.Tasks;

namespace GitHubExplorer
{
    static class Program
    {
        static async Task Main(string[] args) {
            ApiClient apiClient = new ApiClient();
            bool running = true;
            while (running) {
                running = await apiClient.Run();
            }
            Console.WriteLine("Application closing");
        }
    }
}
