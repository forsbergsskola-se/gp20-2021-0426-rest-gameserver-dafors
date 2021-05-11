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
            currentState = State.User;
            gitHubApi = new GithubApi(client);
        }

        public enum State {
            User,
            Repositories,
            Issues,
            Unsupported
        }

        public async Task<bool> Run() {
            bool quitRequested = false;
            switch (currentState) {
                case State.User:
                    return HandleUserState();
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

        private bool HandleUserState() {
            bool quitRequested = false;
            while (!quitRequested && this.user == null)
                quitRequested = NavigateToRequestedUser();
            
            //Temp test
            (user as User)?.PrintAdditionalData();
            

            return true;
            bool viableInstructions = false;
            while (!quitRequested && !viableInstructions) {
                //PromptUserInstructions();
            }
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

        private string PromptUserInstructions(int numOptions) {
            Console.WriteLine($"select navigation target (number between {0} and {numOptions}). >");
            Console.WriteLine($"or enter 'n' to navigate to new user");
            Console.WriteLine($"or enter 'q' to quit");
            return Console.ReadLine();
        }
        
        private bool HandleUserInstructions() {
            string input;
            
            return true;
        }


        private static async Task<(UserJson, MainPageUris)> ProcessResponse(string user = "marczaku") {
            Task<string> stringTask = client.GetStringAsync($"https://api.github.com/users/{user}");
            //string message = await stringTask;
            //Task<Stream> streamTask = client.GetStreamAsync($"https://api.github.com/users/{user}");
            //UserInfo userInfo = await JsonSerializer.DeserializeAsync<UserInfo>(await streamTask);
            //MainPageUris mainPageUris = await JsonSerializer.DeserializeAsync<MainPageUris>(await streamTask);
            
            //UserJson userJson = JsonSerializer.Deserialize<UserJson>(await stringTask);
            //MainPageUris mainPageUris = JsonSerializer.Deserialize<MainPageUris>(await stringTask);
            
            // IDictionary<string, JToken> jsondata = JObject.Parse(await stringTask);
            // Console.WriteLine("KVPS");
            // foreach (var kvp in jsondata) {
            //     Console.WriteLine($"{kvp.Key} ({kvp.Value.Type})");
            // }
            return (null, null);
            //return (userJson, mainPageUris);
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
            return char.TryParse(input, out char c) && c == 'q';
        }
    }
    
    public class Repository : IRepository {
        public string Name { get; }
        public string Description { get; }
    }

    public class MainPageUris {
        [Newtonsoft.Json.JsonPropertyAttribute("organizations_url")]
        public Uri OrganizationsUrl { get; set; }
        [Newtonsoft.Json.JsonPropertyAttribute("repos_url")]
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
                return new User(userJson);
            }
            catch (Exception e) {
                Console.WriteLine("User not, returning null Error message: " + e.Message);
                return null;
            }
        }

        private string UserUri(string userName) {
            return $"https://api.github.com/users/{userName}";
        }
        private static async Task<T> ProcessResponse<T>(string uri) {
            Task<string> stringTask = client.GetStringAsync(uri);
            //return JsonSerializer.Deserialize<T>(await stringTask);
            return JsonConvert.DeserializeObject<T>(await stringTask);
        }
    }

    public class User : IUser {
        private IDictionary<string, JToken> _additionalData;
        private string _repos_url; //"https://api.github.com/users/marczaku/repos"
        private string _description;
        public string Name { get; }
        public string Description { get; }
        
        public User(UserJson userJson) {
            this.Name = userJson.Name;
            this._repos_url = userJson.ReposUrl;
            this._description = $"Name: {Name}, Location: {userJson.Location}, Company: {userJson.Company}";
            this._additionalData = userJson.AdditionalData;
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
    }
    
    public class UserJson {
        [Newtonsoft.Json.JsonPropertyAttribute("name")]
        public string Name { get; set; }
        [Newtonsoft.Json.JsonPropertyAttribute("location")]
        public string Location { get; set; }
        //[JsonPropertyName("organizations_url")]
        //public Uri OrganizationsUrl { get; set; }
        [Newtonsoft.Json.JsonPropertyAttribute("repos_url")]
        public string ReposUrl { get; set; }
        [Newtonsoft.Json.JsonPropertyAttribute("company")]
        public string Company { get; set; }
        
        [JsonExtensionData]
        public IDictionary<string, JToken> AdditionalData { get; set; }
        //[JsonExtensionData]
        //public IDictionary<string, object> AdditionalData { get; set; }        
    }
}