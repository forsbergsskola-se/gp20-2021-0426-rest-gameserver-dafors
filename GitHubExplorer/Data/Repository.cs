using Newtonsoft.Json;

namespace GitHubExplorer.Data {
    public class Repository : IRepository {
        [JsonProperty("name")]
        public string Name { get; set; }
        
        [JsonProperty("url")]
        public string Url { get; set; }
        
        [JsonProperty("description")]
        public string Description { get; set; }
        public override string ToString() {
            return $"{Name}, {Description}";
        }
    }
}