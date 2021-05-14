using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GitHubExplorer.Data {
    public class Organization {
        [JsonProperty("login")]public string Login { get; set; }
        [JsonProperty("public_members_url")] public string PublicMembersUrl { get; set; }
        [JsonProperty("description")] public string Description { get; set; }
        [JsonExtensionData] private IDictionary<string, JToken> _additionalData;
        public override string ToString() {
            return $"({Login}) {Description}";
        }
    }
}