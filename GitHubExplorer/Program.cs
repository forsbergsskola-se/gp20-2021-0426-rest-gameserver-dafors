// microsoft docs used for this repo: https://docs.microsoft.com/en-us/dotnet/csharp/tutorials/console-webapiclient
using System;
using System.Threading.Tasks;

namespace GitHubExplorer
{
    static class Program
    {
        static async Task Main(string[] args) {
            GithubApiClient githubApiClient = new GithubApiClient();
            bool quitRequested = false;
            while (!quitRequested) {
                quitRequested = await githubApiClient.Run();
            }
            Console.WriteLine("Application closing...");
        }
    }
}
