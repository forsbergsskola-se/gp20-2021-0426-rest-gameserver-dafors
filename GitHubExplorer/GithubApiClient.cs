// https://stackoverflow.com/questions/55046717/json-deserialization-how-do-i-add-remaining-items-into-a-dictionary
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GitHubExplorer {
    public class GithubApiClient {
        static readonly HttpClient client = new HttpClient();
        private State currentState;
        //private string user = null;
        private IGitHubAPI gitHubApi = null;
        private IUser user = null;

        public GithubApiClient() {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
            client.DefaultRequestHeaders.Add("User-Agent", "korv");
            currentState = State.FindUser;
            gitHubApi = new GithubApi(client);
        }

        public enum State {
            FindUser,
            User,
            Repositories,
            Issues,
            Unsupported
        }

        public async Task<bool> Run() {
            bool quitRequested = false;

            switch (currentState) {
                case State.FindUser:
                    quitRequested = FindUser();
                    break;
                case State.User:
                    quitRequested = HandleUserState();
                    break;
                case State.Repositories:
                    break;
                case State.Issues:
                    break;
                case State.Unsupported:
                    break;
            }

            return quitRequested;
            var result = await ProcessResponse();
            UserJson userJson = result.Item1;
            MainPageUris mainPageUris = result.Item2;
            Console.WriteLine(userJson);
            PropertyInfo[] propInfos = mainPageUris.GetType().GetProperties();
            int index = 0;
            foreach (PropertyInfo prop in propInfos) {
                Console.WriteLine($"{index} --> {prop.Name} ({prop.PropertyType.Name}): {prop.GetValue(mainPageUris)}");
                //Uri uri = (Uri) prop.GetValue(mainPageUris);
                index++;
            }

            //Console.WriteLine("length: " + propInfos.Length);
            Console.WriteLine("navigation options: ");
            return true;
        }

        private bool FindUser() {
            bool quitRequested = false;
            while (!quitRequested && this.user == null)
                quitRequested = NavigateToRequestedUser();
            currentState = State.User;
            return quitRequested;
        }
        
        private bool NavigateToRequestedUser() {
            Console.WriteLine($"Enter user to navigate to.");
            Console.WriteLine("Enter 'q' to quit.");
            string input = Console.ReadLine();
            if (QuitRequested(input))
                return true;

            user = gitHubApi.GetUser(input);
            return false;
        }

        private bool HandleUserState() {
            bool quitRequested = false;
            bool stateTransition = false;
            
            while (!quitRequested && !stateTransition) {
                string input = PromptUserInstructions();
                switch (input) {
                    case "d":
                        Console.WriteLine(user.Description);
                        break;
                    case "r":
                        Console.WriteLine("TODO navigate to repos");
                        break;
                    case "c":
                        Console.WriteLine("TODO see all options");
                        break;
                    case "n":
                        currentState = State.FindUser;
                        stateTransition = true;
                        break;
                    case "q":
                        quitRequested = true;
                        break;
                    default:
                        Console.WriteLine("undefined command");
                        break;
                }
            }
            return quitRequested;
        }

        private string PromptUserInstructions() {
            Console.WriteLine($"enter 'd' for description");
            Console.WriteLine($"enter 'r' to navigate to repos");
            Console.WriteLine($"enter 'c' to see all (untested) options");
            Console.WriteLine($"enter 'n' to navigate to new user");
            Console.WriteLine($"enter 'q' to quit");
            return Console.ReadLine();
        }
        
        private bool HandleUserInstructions() {
            string input;
            
            return true;
        }


        // private static async Task<(UserJson, MainPageUris)> ProcessResponse(string user = "marczaku") {
        //     Task<string> stringTask = client.GetStringAsync($"https://api.github.com/users/{user}");
        //     //string message = await stringTask;
        //     //Task<Stream> streamTask = client.GetStreamAsync($"https://api.github.com/users/{user}");
        //     //UserInfo userInfo = await JsonSerializer.DeserializeAsync<UserInfo>(await streamTask);
        //     //MainPageUris mainPageUris = await JsonSerializer.DeserializeAsync<MainPageUris>(await streamTask);
        //     
        //     //UserJson userJson = JsonSerializer.Deserialize<UserJson>(await stringTask);
        //     //MainPageUris mainPageUris = JsonSerializer.Deserialize<MainPageUris>(await stringTask);
        //     
        //     // IDictionary<string, JToken> jsondata = JObject.Parse(await stringTask);
        //     // Console.WriteLine("KVPS");
        //     // foreach (var kvp in jsondata) {
        //     //     Console.WriteLine($"{kvp.Key} ({kvp.Value.Type})");
        //     // }
        //     return (null, null);
        //     //return (userJson, mainPageUris);
        // }

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
            return char.TryParse(input, out char c) && c == 'q';
        }
    }
    
    public class Repository : IRepository {
        public string Name { get; }
        public string Description { get; }
    }

    public class MainPageUris {
        [JsonPropertyAttribute("organizations_url")]
        public Uri OrganizationsUrl { get; set; }
        [JsonPropertyAttribute("repos_url")]
        public Uri ReposUrl { get; set; }
        private IDictionary<string, JToken> _additionalData;
    }

    public class UknownClass {
        [System.Text.Json.Serialization.JsonExtensionData]
        private IDictionary<string, JToken> _additionalData;
    }

    public class GithubApi : IGitHubAPI {
        private static HttpClient client;
        public GithubApi(HttpClient c) {
            client = c;
        }
        public IUser GetUser(string userName) {
            try {
                UserJson userJson = ProcessResponse<UserJson>(UserUri(userName)).GetAwaiter().GetResult();
                List<RepoJson> repoJsonList = ProcessResponse<List<RepoJson>>(ReposUri(userName)).GetAwaiter().GetResult();
                return new User(userJson, repoJsonList);
            }
            catch (Exception e) {
                Console.WriteLine("Error message: " + e.Message);
                Console.WriteLine($"Returning null for input {userName}");
                return null;
            }
        }

        private string UserUri(string userName) {
            return $"https://api.github.com/users/{userName}";
        }

        private string ReposUri(string userName) {
            return $"https://api.github.com/users/{userName}/repos";
        }
        
        private static async Task<T> ProcessResponse<T>(string uri) {
            Task<string> stringTask = client.GetStringAsync(uri);
            //return JsonSerializer.Deserialize<T>(await stringTask);
            return JsonConvert.DeserializeObject<T>(await stringTask);
        }
    }

    public class User : IUser {
        private IDictionary<string, JToken> _additionalData;
        private Dictionary<string, Repo> repoDictionary;
        private string _description;
        public string Name { get; }
        public string Description => _description;
        
        public User(UserJson userJson, List<RepoJson> repoJsonList) {
            this.Name = userJson.Name;
            this._description = $"Name: {Name}, Location: {userJson.Location}, Company: {userJson.Company}";
            this._additionalData = userJson.AdditionalData;
            
            this.repoDictionary = new Dictionary<string, Repo>();
            foreach (var repo in repoJsonList) {
                repoDictionary.Add(repo.Name, new Repo(repo.Description, repo.Url));
            }
        }
        
        public IRepository GetRepository(string repository) {
            throw new NotImplementedException();
            //https://api.github.com/repos/marczaku/CityBuilder
        }

        //temp test
        public void PrintAdditionalData() {
            foreach (var kvp in _additionalData) {
                Console.WriteLine($"{kvp.Key} ({kvp.Value.Type})");
            }
        }

        public void PrintRepos() {
            foreach (var kvp in repoDictionary) {
                Console.WriteLine($"{kvp.Key}, {kvp.Value.Description}");
            }
        }

        private struct Repo {
            public string Description;
            public string Url;
            public Repo(string description, string url) {
                Description = description;
                Url = url;
            }
        }
    }
    
    public class UserJson {
        [JsonPropertyAttribute("name")]
        public string Name { get; set; }
        [JsonPropertyAttribute("location")]
        public string Location { get; set; }
        //[JsonPropertyName("organizations_url")]
        //public Uri OrganizationsUrl { get; set; }
        [Newtonsoft.Json.JsonPropertyAttribute("repos_url")]
        public string ReposUrl { get; set; }
        [JsonPropertyAttribute("company")]
        public string Company { get; set; }
        
        [JsonExtensionData]
        public IDictionary<string, JToken> AdditionalData { get; set; }
        //[JsonExtensionData]
        //public IDictionary<string, object> AdditionalData { get; set; }        
    }

    public class RepoJson {
        [JsonPropertyAttribute("name")]
        public string Name { get; set; }
        
        [JsonPropertyAttribute("url")]
        public string Url { get; set; }
        
        [JsonPropertyAttribute("description")]
        public string Description { get; set; }
    }
}