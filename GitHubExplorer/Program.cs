// microsoft docs used for this repo: https://docs.microsoft.com/en-us/dotnet/csharp/tutorials/console-webapiclient
using System;
namespace GitHubExplorer
{
    static class Program
    {
        static void Main(string[] args) {
            GithubApiClient githubApiClient = new GithubApiClient();
            githubApiClient.Run();
            Console.WriteLine("Application closing...");
        }
    }
}
