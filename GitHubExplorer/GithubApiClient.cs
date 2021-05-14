using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using GitHubExplorer.Data;
using Newtonsoft.Json;

namespace GitHubExplorer {
    public class GithubApiClient {
        static readonly HttpClient client = new HttpClient();
        private IGitHubApi gitHubApi = null;
        private IUser user = null;

        public GithubApiClient() {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
            client.DefaultRequestHeaders.Add("User-Agent", "korv");
            gitHubApi = new GithubApi(client);
        }

        public void Run() {
            Console.WriteLine("GithubAPI started. Type help for list of commands");
            while (true) {
                string input = PromptUserInstructions();
                if (QuitRequested(input))
                    break;
                ProcessUserInput(input);
            }
            client.Dispose();
        }
        
        private void ProcessUserInput(string input) {
            string[] words = input.Split(' ');
            
            if (words.Length == 1) {
                switch (words[0]) {
                    case "unmanaged":
                        if (user != null)
                            user.PrintAdditionalData();
                        else 
                            Console.WriteLine("no user selected");
                        break;
                    case "orgs":
                        if (user != null)
                            foreach (var org in user.Organizations()) {
                                Console.WriteLine(org);
                            }
                        else
                            Console.WriteLine("no user selected");
                        break;
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
                        user = gitHubApi.GetUser(words[1]);
                        break;
                    case "repo":
                        if (user == null)
                            Console.WriteLine("no user selected");
                        else
                            user.GetRepository(words[1]);
                        break;
                    case "goto":
                        IUnmanagedData page = GithubApi.GetUnmanaged(words[1]);
                        page?.Print();
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
            Console.WriteLine($"'goto <url>' to navigate freely");
            Console.WriteLine($"'unmanaged' to show unmanaged options for the current user");
            Console.WriteLine($"'user <user>' to navigate to new user");
            Console.WriteLine($"'description' shows description");
            Console.WriteLine($"'repos' shows current users repos");
            Console.WriteLine($"'orgs' shows current users organizations");
            Console.WriteLine($"'repo <repo>' shows repo for selected user");
            Console.WriteLine($"'help' for list commands");
            Console.WriteLine($"'exit' quit application");
        }
        
        private static async Task<T> ProcessResponse<T>(string uri) {
            Task<string> stringTask = client.GetStringAsync(uri);
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

    public class GithubApi : IGitHubApi {
        private static HttpClient client;
        public GithubApi(HttpClient c) {
            client = c;
        }
        public IUser GetUser(string userName) {
            try {
                return ProcessResponseAsync<User>(UserUrl(userName)).GetAwaiter().GetResult();
            }
            catch (Exception e) {
                Console.WriteLine($"Error message: {e.Message}");
                Console.WriteLine($"Returning null for input {userName}");
                return null;
            }
        }

        public static List<Repository> GetRepositories(string userName) {
            try {
                return ProcessResponseAsync<List<Repository>>(ReposUrl(userName)).GetAwaiter().GetResult();
            }
            catch (Exception e) {
                Console.WriteLine($"Error message: {e.Message}");
                Console.WriteLine($"Returning null for input {userName}");
                return null;
            }
        }
        
        public static IUnmanagedData GetUnmanaged(string url) {
            try {
                string result = client.GetStringAsync(url).GetAwaiter().GetResult();
                if (result.StartsWith('[')) {
                    var list = ProcessResponse<List<DynamicData>>(result);
                    return new DynamicListData(list);
                }
                return ProcessResponse<DynamicData>(result);
            }
            catch (JsonSerializationException e) {
                throw e;
            }
            catch (Exception e) {
                Console.WriteLine($"Error message: {e.Message}");
                Console.WriteLine(e.GetType());
                Console.WriteLine($"Returning null for input {url}");
                return null;
            }
        }

        public static Repository GetRepository(string userName, string repoName) {
            return GetRepository(RepoUrl(userName, repoName));
        }

        public static Repository GetRepository(string url) {
            try {
                return ProcessResponseAsync<Repository>(url).GetAwaiter().GetResult();
            }
            catch (Exception e) {
                Console.WriteLine($"Error message: {e.Message}");
                Console.WriteLine($"Returning null for input {url}");
                return null;
            }
        }
        
        public static IEnumerable<Organization> GetOrganizations(string userName) {
            try {
                return ProcessResponseAsync<List<Organization>>(OrganizationsUrl(userName)).GetAwaiter().GetResult();
            }
            catch (Exception e) {
                Console.WriteLine($"Error message: {e.Message}");
                Console.WriteLine($"Returning null for input {userName}");
                return null;
            }
        }

        private static string UserUrl(string userName) {
            return $"https://api.github.com/users/{userName}";
        }

        private static string ReposUrl(string userName) {
            return $"https://api.github.com/users/{userName}/repos";
        }

        private static string RepoUrl(string userName, string repoName) {
            return $"https://api.github.com/repos/{userName}/{repoName}";
        }

        private static string OrganizationsUrl(string userName) {
            return $"https://api.github.com/users/{userName}/orgs";
        }
        
        private static async Task<T> ProcessResponseAsync<T>(string uri) {
            Task<string> stringTask = client.GetStringAsync(uri);
            return JsonConvert.DeserializeObject<T>(await stringTask);
        }
        
        private static T ProcessResponse<T>(string stringToDeserialize) {
            return JsonConvert.DeserializeObject<T>(stringToDeserialize);
        }
    }
}