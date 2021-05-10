using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GitHubExplorer {
    public class ApiClient {
        static readonly HttpClient client = new HttpClient();

        public ApiClient() {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
            client.DefaultRequestHeaders.Add("User-Agent", "korv");
        }

        public async Task<bool> Run() {
            Console.WriteLine("Enter user to navigate to");
            string input = Console.ReadLine();
            var result = await ProcessResponse(input);
            UserInfo userInfo = result.Item1;
            MainPageUris mainPageUris = result.Item2;
            Console.WriteLine(userInfo);
            PropertyInfo[] propInfos = mainPageUris.GetType().GetProperties();
            int index = 0;
            foreach (PropertyInfo prop in propInfos) {
                Console.WriteLine($"{index} --> {prop.Name} ({prop.PropertyType.Name}): {prop.GetValue(mainPageUris)}");
                index++;
            }
            //Console.WriteLine("length: " + propInfos.Length);
            Console.WriteLine("navigation options: ");
            return true;
        }
        
        private static async Task<(UserInfo, MainPageUris)> ProcessResponse(string user = "marczaku") {
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