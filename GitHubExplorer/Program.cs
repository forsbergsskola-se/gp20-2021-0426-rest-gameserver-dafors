// microsoft docs used for this repo: https://docs.microsoft.com/en-us/dotnet/csharp/tutorials/console-webapiclient
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GitHubExplorer
{
    class Program
    {
        static readonly HttpClient client = new HttpClient();
        static async Task Main(string[] args) {
            var result = await ProcessResponse();
            UserInfo userInfo = result.Item1;
            MainPageUris mainPageUris = result.Item2;
            Console.WriteLine(userInfo);
        }

        private static async Task<(UserInfo, MainPageUris)> ProcessResponse(string user = "marczaku") {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
            client.DefaultRequestHeaders.Add("User-Agent", "korv");
            
            Task<string> stringTask = client.GetStringAsync($"https://api.github.com/users/{user}");
            //string message = await stringTask;
            //Task<Stream> streamTask = client.GetStreamAsync($"https://api.github.com/users/{user}");
            //UserInfo userInfo = await JsonSerializer.DeserializeAsync<UserInfo>(await streamTask);
            //MainPageUris mainPageUris = await JsonSerializer.DeserializeAsync<MainPageUris>(await streamTask);
            UserInfo userInfo = JsonSerializer.Deserialize<UserInfo>(await stringTask);
            MainPageUris mainPageUris = JsonSerializer.Deserialize<MainPageUris>(await stringTask);
            return (userInfo, mainPageUris);
        }
    }
    
    public class UserInfo {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        
        [JsonPropertyName("company")]
        public string Company { get; set; }
        
        [JsonPropertyName("location")]
        public string Location { get; set; }

        public override string ToString() {
            return $"User: {Name} \n" +
                   $"Company: {Company} \n" +
                   $"Location: {Location}";
        }
    }

    public class MainPageUris {
        [JsonPropertyName("organizations_url")]
        public Uri OrganizationsUrl { get; set; }

        [JsonPropertyName("repos_url")]
        public Uri ReposUrl { get; set; }
    }
}
