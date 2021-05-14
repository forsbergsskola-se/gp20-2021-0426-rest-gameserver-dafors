using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GitHubExplorer.Data {
    public class User : IUser {
        [JsonProperty("login")] public string Login { get; set; }
        [JsonProperty("name")] public string Name { get; set; }
        [JsonProperty("location")] public string Location { get; set; }
        [JsonProperty("company")] public string Company { get; set; }
        [JsonExtensionData] private IDictionary<string, JToken> AdditionalData { get; set; }
        
        public string Description => $"({Login}) Name: {Name}, Location: {Location}, Company: {Company}";

        public IRepository GetRepository(string repository) {
            return GithubApi.GetRepository(Login, repository);
        }

        public IEnumerable<IRepository> Repositories() {
            var list = GithubApi.GetRepositories(Login);
            if (list == null)
                yield return null;
            
            foreach (var repo in list) {
                yield return repo;
            }
        }

        public IEnumerable<Organization> Organizations() {
            var list = GithubApi.GetOrganizations(Login);
            if (list == null)
                yield return null;
            
            foreach (var repo in list) {
                yield return repo;
            }
        }
        
        public void PrintAdditionalData() {
            foreach (var kvp in AdditionalData) {
                string info = kvp.Value == null ? "" : kvp.Value.ToString();
                Console.WriteLine($"{kvp.Key} ({kvp.Value.Type}) {info}" );
            }
        }
    }
}