// https://stackoverflow.com/questions/55046717/json-deserialization-how-do-i-add-remaining-items-into-a-dictionary

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GitHubExplorer {
    public class GithubApiClient {
        static readonly HttpClient client = new HttpClient();
        private IGitHubAPI gitHubApi = null;
        private IUser user = null;

        public GithubApiClient() {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
            client.DefaultRequestHeaders.Add("User-Agent", "korv");
            gitHubApi = new GithubApi(client);
        }

        public void Run() {
            ShowInstructions();
            while (true) {
                string input = PromptUserInstructions();
                if (QuitRequested(input))
                    break;
                ProcessUserInput(input);
            }
        }
        
        private void ProcessUserInput(string input) {
            string[] words = input.Split(' ');
            
            if (words.Length == 1) {
                switch (words[0]) {
                    case "description":
                        if (user != null)
                            Console.WriteLine(user.Description);
                        else
                            Console.WriteLine("no user selected");
                        break;
                    case "repos":
                        if (user != null)
                            foreach (var repo in user.Repositories()) {
                                Console.WriteLine(repo);
                            }
                        else
                            Console.WriteLine("no user selected");
                        break;
                    case "help":
                        ShowInstructions();
                        break;
                    default:
                        Console.WriteLine("undefined command");
                        break;
                }    
            }
            else if (words.Length == 2) {
                switch (words[0]) {
                    case "user":
                        this.user = gitHubApi.GetUser(words[1]);
                        break;
                    case "repo":
                        if (this.user == null)
                            Console.WriteLine("no user selected");
                        else
                            user.GetRepository(words[1]);
                        break;
                    case "unmanaged":
                        break;
                    default:
                        Console.WriteLine("undefined command");
                        break;
                }
            }
            else {
                Console.WriteLine("undefined command");
            }
        }
        
        private string PromptUserInstructions() {
            Console.WriteLine(this.user == null ? $"(base) GithubAPI >" : $"({this.user.Name}) GithubAPI >");
            return Console.ReadLine();
        }

        private void ShowInstructions() {
            Console.WriteLine($"'unmanaged options' to navigate unmanaged options");
            Console.WriteLine($"'user <user>' to navigate to new user");
            Console.WriteLine($"'description' shows description");
            Console.WriteLine($"'repos' shows current users repos");
            Console.WriteLine($"'repo <repo>' shows repo for selected user");
            Console.WriteLine($"'help' for list commands");
            Console.WriteLine($"'exit' quit application");
        }
        
        private static async Task<T> ProcessResponse<T>(string uri) {
            Task<string> stringTask = client.GetStringAsync(uri);
            //return JsonSerializer.Deserialize<T>(await stringTask);
            return JsonConvert.DeserializeObject<T>(await stringTask);
        }

        private static async Task<dynamic> ProcessDynamic(string uri) {
            Task<string> stringTask = client.GetStringAsync(uri);
            return JsonConvert.DeserializeObject<dynamic>(await stringTask);
        }
        
        private bool QuitRequested(string input) {
            return input == "exit";
        }
    }

    public class MainPageUris {
        [JsonPropertyAttribute("organizations_url")]
        public Uri OrganizationsUrl { get; set; }
        [JsonPropertyAttribute("repos_url")]
        public Uri ReposUrl { get; set; }
        private IDictionary<string, JToken> _additionalData;
    }

    public class GithubApi : IGitHubAPI {
        private static HttpClient client;
        public GithubApi(HttpClient c) {
            client = c;
        }
        public IUser GetUser(string userName) {
            try {
                return ProcessResponse<User>(UserUri(userName)).GetAwaiter().GetResult();
            }
            catch (Exception e) {
                Console.WriteLine("Error message: " + e.Message);
                Console.WriteLine($"Returning null for input {userName}");
                return null;
            }
        }

        public static List<Repository> GetRepositories(string userName) {
            try {
                return ProcessResponse<List<Repository>>(ReposUri(userName)).GetAwaiter().GetResult();
            }
            catch (Exception e) {
                Console.WriteLine("Error message: " + e.Message);
                Console.WriteLine($"Returning null for input {userName}");
                return null;
            }
        }

        public static Repository GetRepository(string userName, string repoName) {
            return GetRepository(RepoUri(userName, repoName));
        }

        public static Repository GetRepository(string url) {
            try {
                return ProcessResponse<Repository>(url).GetAwaiter().GetResult();
            }
            catch (Exception e) {
                Console.WriteLine("Error message: " + e.Message);
                Console.WriteLine($"Returning null for input {url}");
                return null;
            }
        }

        private static string UserUri(string userName) {
            return $"https://api.github.com/users/{userName}";
        }

        private static string ReposUri(string userName) {
            return $"https://api.github.com/users/{userName}/repos";
        }

        private static string RepoUri(string userName, string repoName) {
            return $"https://api.github.com/repos/{userName}/{repoName}";
        }
        
        private static async Task<T> ProcessResponse<T>(string uri) {
            Task<string> stringTask = client.GetStringAsync(uri);
            return JsonConvert.DeserializeObject<T>(await stringTask);
        }
    }

    public class User : IUser {
        [JsonPropertyAttribute("login")] public string Login { get; set; }
        [JsonPropertyAttribute("name")] public string Name { get; set; }
        [JsonPropertyAttribute("location")] public string Location { get; set; }
        //[JsonPropertyName("organizations_url")]
        //public Uri OrganizationsUrl { get; set; }
        //[JsonPropertyAttribute("repos_url")] public string ReposUrl { get; set; }
        [JsonPropertyAttribute("company")] public string Company { get; set; }
        [JsonExtensionData] public IDictionary<string, JToken> AdditionalData { get; set; }
        //[JsonExtensionData] public IDictionary<string, object> AdditionalData { get; set; }
        public string Description => $"({Login}) Name: {Name}, Location: {Location}, Company: {Company}";

        public IRepository GetRepository(string repository) {
            //TODO
            //https://api.github.com/repos/marczaku/CityBuilder
            return null;
        }

        //temp test
        public void PrintAdditionalData() {
            foreach (var kvp in AdditionalData) {
                Console.WriteLine($"{kvp.Key} ({kvp.Value.Type})");
            }
        }

        public IEnumerable<IRepository> Repositories() {
            var list = GithubApi.GetRepositories(Login);
            if (list == null)
                yield return null;
            
            foreach (var repo in list) {
                yield return repo;
            }
        }
    }

    public class Repository : IRepository {
        [JsonPropertyAttribute("name")]
        public string Name { get; set; }
        
        [JsonPropertyAttribute("url")]
        public string Url { get; set; }
        
        [JsonPropertyAttribute("description")]
        public string Description { get; set; }
        public override string ToString() {
            return $"{Name}, {Description}";
        }
    }

    public class UnmanagedClass {
        [System.Text.Json.Serialization.JsonExtensionData]
        private IDictionary<string, JToken> _additionalData;
    }
}
// Some legacy code kept for reference
// PropertyInfo[] propInfos = mainPageUris.GetType().GetProperties();
// int index = 0;
// foreach (PropertyInfo prop in propInfos) {
//     Console.WriteLine($"{index} --> {prop.Name} ({prop.PropertyType.Name}): {prop.GetValue(mainPageUris)}");
//     //Uri uri = (Uri) prop.GetValue(mainPageUris);
//     index++;
// }

//Console.WriteLine("length: " + propInfos.Length);