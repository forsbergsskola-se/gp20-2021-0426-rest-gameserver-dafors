using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace TinyBrowser {
    public class Browser {
        private string hostName; // = "acme.com";
        private string path; // = "/";
        private int port;

        private TcpClient tcpClient;
        private Stream stream;
        private StreamWriter streamWriter;

        private string GetRequest => $"GET {path} HTTP/1.1\r\nHost: {hostName}\r\n\r\n";
        
        public Browser(string hostName = "acme.com", string path = "/", int port = 80) {
            this.hostName = hostName;
            this.path = path;
            this.port = port;
        }
        
        public void Browse() {
            ConnectToHost();
            
            string response = StreamHostData();
            DisplayTitle(response);
            
            var hrefDict = GetHrefDict(response);
            DisplayHrefs(hrefDict);
            
            int input = AwaitUserInput(hrefDict.Count);
            var requestedUri = InterpretInput(input, hrefDict);

            this.hostName = requestedUri.Host;
            this.path = requestedUri.LocalPath;
            Browse();
        }

        private static void DisplayTitle(string response) {
            string title = FindTextBetweenTags(response, Tags.titleStartTag, Tags.titleEndTag);
            Console.WriteLine($"{title}");
        }

        private static void DisplayHrefs(Dictionary<int, (string, string)> hrefDict) {

            Console.WriteLine("------------------Printing hrefs------------------");

            foreach (var kvp in hrefDict) {
                Console.WriteLine($"{kvp.Key} -> {kvp.Value.Item1}, {kvp.Value.Item2}");
            }
        }

        private void ConnectToHost() {
            var uriBuilder = new UriBuilder(null, hostName) {Path = path};
            Console.WriteLine( $"------------------Connecting to {uriBuilder}...------------------");
            tcpClient = new TcpClient(hostName, port);
            stream = tcpClient.GetStream();
            streamWriter = new StreamWriter(stream, Encoding.ASCII);
            streamWriter.Write(GetRequest); // add data to the buffer
            streamWriter.Flush(); // send the buffered data
        }
        
        private string StreamHostData() {
            var streamReader = new StreamReader(stream);
            return streamReader.ReadToEnd();
        }

        private int AwaitUserInput(int numOptions) {
            int input = -1;
            Console.WriteLine($"select navigation target (number between {0} and {numOptions}). >");

            while (!(0 <= input && input <= numOptions)) {
                input = Convert.ToInt32(Console.ReadLine());
                if (!(0 <= input && input <= numOptions))
                    Console.WriteLine($"invalid input, input (number between {0} and {numOptions}). >");
            }
            return input;
        }
        
        private Uri InterpretInput(int input, in Dictionary<int, (string,string)> hrefDict) {
            Console.WriteLine($"Interpreting {input}...");
            string attribute = hrefDict[input].Item1;
            return InterpretAttribute(attribute);
        }

        private Uri InterpretAttribute(in string attribute) {
            UriBuilder result = new UriBuilder();
            if (attribute.EndsWith('/')) {
                result.Path = attribute[0] == '/' ? attribute : '/' + attribute;
                result.Host = hostName;
            }
            return result.Uri;
        }
        
        private static string FindTextBetweenTags(string inputText, string startTag, string endTag, int startOffset = 0) {
            string result = string.Empty;
            var startIndex = inputText.IndexOf(startTag, startOffset, StringComparison.OrdinalIgnoreCase);
            if (startIndex != -1) {
                startIndex += startTag.Length;
                var endIndex = inputText.IndexOf(endTag, startIndex, StringComparison.OrdinalIgnoreCase); // find next occurence of endTag
                result = inputText[startIndex..endIndex]; //range lookup
            }
            return result;
        }

        static IEnumerable<string> GetAllBetweenTags(string inputText, string startTag, string endTag) {
            int currentIndex = 0;
            Console.WriteLine($"finding occurrences between {startTag} and {endTag}...");
            while (true) {
                var startIndex = inputText.IndexOf(startTag, currentIndex);
                if (startIndex == -1)
                    yield break;
                startIndex += startTag.Length;
                var endIndex = inputText.IndexOf(endTag, startIndex);
                if (endIndex == -1)
                    yield break;
                yield return inputText[(startIndex)..endIndex];
                currentIndex = endIndex;
            }
        }
        
        static Dictionary<int, (string, string)> GetHrefDict(string inputText) {
            const string hrefStartTag = "<a href=\""; 
            const string attributeDelim = "\"";
            const string hrefEndTag = "</a>";
            
            Dictionary<int, (string, string)> hrefDict = new Dictionary<int, (string, string)>();

            int dictIndex = 0;
            int currentIndex = 0;
            while (true) {
                var startIndex = inputText.IndexOf(hrefStartTag, currentIndex);
                if (startIndex == -1)
                    break;
                startIndex += hrefStartTag.Length;
                var endIndex = inputText.IndexOf(attributeDelim, startIndex);
                if (endIndex == -1)
                     break;
                var attribute = inputText[(startIndex)..endIndex];
                startIndex = endIndex + attributeDelim.Length;
                startIndex = inputText.IndexOf('>', startIndex);
                if (startIndex == -1)
                    break;
                startIndex += 1;
                endIndex = inputText.IndexOf(hrefEndTag, startIndex);
                if (endIndex == -1)
                    break;
                var htmlContent = inputText[startIndex..endIndex];
                hrefDict.Add(dictIndex, (attribute, htmlContent));
                dictIndex++;
                currentIndex = endIndex;
            }
            return hrefDict;
        }

        private static class Tags {
            public const string titleStartTag = "<title>";
            public const string titleEndTag = "</title>";
        }
    }
}