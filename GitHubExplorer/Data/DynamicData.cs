using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GitHubExplorer.Data {
    public class DynamicData : IUnmanagedData {
        [JsonExtensionData] private IDictionary<string, JToken> AdditionalData;
        public void Print() {
            foreach (var kvp in AdditionalData) {
                string info = kvp.Value == null ? "" : kvp.Value.ToString();
                Console.WriteLine($"{kvp.Key} ({kvp.Value.Type}) {info}" );
            }
        }
    }
}