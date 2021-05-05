using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace TinyBrowser {
    public class Browser {
        private string hostName; // = "www.acme.com";
        private string path; // = "/";
        private int port;

        private TcpClient tcpClient;
        private Stream stream;
        private StreamWriter streamWriter;
        private Stack<Uri> backStack;
        private Stack<Uri> forwardStack;

        private string GetRequest => $"GET {path} HTTP/1.1\r\nHost: {hostName}\r\n\r\n";

        public Browser(string hostName = "acme.com", string path = "/", int port = 80) {
            this.hostName = hostName;
            this.path = path;
            this.port = port;
            this.backStack = new Stack<Uri>();
            this.forwardStack = new Stack<Uri>();
        }
        
        public bool Browse() {
            ConnectToHost();
            string response = StreamHostData();
            DisplayTitle(response);
            
            var hrefDict = GetHrefDict(response);
            DisplayHrefs(hrefDict);
            
            string input = AwaitUserInput(hrefDict.Count-1);
            
            if (IsValidCommandInput(input)) {
                bool quitRequested = HandleCommand(input);
                return quitRequested;
            }
            
            int requestedNr = Convert.ToInt32(input);
            Uri requestedUri = InterpretInput(requestedNr, hrefDict);
            this.hostName = requestedUri.Host;
            this.path = requestedUri.LocalPath;
            return false;
        }

        private bool HandleCommand(string input) {
            char c = char.Parse(input);
            bool quitRequested = false;
            if (c == 'b') {
                Console.WriteLine("back requested");
                
                //TODO 
            }
            else if (c == 'f') {
                Console.WriteLine("forward requested");
                //TODO 
            }
            else if (c == 'q') {
                Console.WriteLine("quit requested");
                CloseConnection();
                quitRequested = true;
            }
            return quitRequested;
        }

        private void CloseConnection() {
            Console.WriteLine("closing connection...");
            stream.Close();
            tcpClient.Close();
        }

        private static void DisplayTitle(string response) {
            string title = FindTextBetweenTags(response, Tags.titleStartTag, Tags.titleEndTag);
            Console.WriteLine($"{title}");
        }

        private static void DisplayHrefs(Dictionary<int, (string, string)> hrefDict) {
            if (hrefDict.Count <= 0) {
                Console.WriteLine("------------------ No hrefs found ------------------");
            }
            else {
                Console.WriteLine("------------------ Printing hrefs ------------------");
                foreach (var kvp in hrefDict) {
                    Console.WriteLine($"{kvp.Key} -> {kvp.Value.Item1}, {kvp.Value.Item2}");
                }                
            }
        }

        private void ConnectToHost() {
            var uriBuilder = new UriBuilder(null, hostName) {Path = path};
            Console.WriteLine( $"------------------ Connecting to {uriBuilder} ------------------");
            tcpClient = new TcpClient(hostName, port);
            stream = tcpClient.GetStream();
            streamWriter = new StreamWriter(stream, Encoding.ASCII);
            streamWriter.Write(GetRequest); // add data to the buffer
            streamWriter.Flush();           // send the buffered data
        }
        
        private string StreamHostData() {
            var streamReader = new StreamReader(stream);
            return streamReader.ReadToEnd();
        }

        private string AwaitUserInput(int numOptions) {
            Console.WriteLine($"select navigation target (number between {0} and {numOptions}). >");
            Console.WriteLine($"or enter 'b' to go backward");
            Console.WriteLine($"or enter 'f' to go forward");
            Console.WriteLine($"or enter 'q' to quit");
            
            string inputString = Console.ReadLine();;
            while (!IsValidInput(inputString, numOptions)) {
                inputString = Console.ReadLine();
            }
            return inputString;
        }

        private bool IsValidInput(string inputString, int numOptions) {
            return IsValidCommandInput(inputString) || IsValidNumericInput(inputString, numOptions);
        }

        private bool IsValidCommandInput(string inputString) {
            if (inputString.Length != 1)
                return false;
            char c = char.Parse(inputString);
            return c == 'b' || c == 'f' || c == 'q';
        }

        private bool IsValidNumericInput(string input, int maxVal) {
            if (!int.TryParse(input, out int inputNumeric))
                return false;
            return 0 <= inputNumeric && inputNumeric <= maxVal;
        }
        
        private Uri InterpretInput(int input, in Dictionary<int, (string,string)> hrefDict) {
            string attribute = hrefDict[input].Item1;
            Console.WriteLine($"Interpreting {input}, {attribute}");
            return InterpretAttribute(attribute);
        }

        private Uri InterpretAttribute(in string attribute) {
            UriBuilder result = new UriBuilder();

            if (attribute.Contains("http") || attribute.Contains("www.")) {
                GetHostAndPath(attribute, ref result);
            }
            else {
                result.Path = attribute[0] == '/' ? attribute : '/' + attribute;
                result.Host = this.hostName;
            }
            return result.Uri;
        }

        private void GetHostAndPath(in string str, ref UriBuilder uriBuilder) {
            Uri uri = new Uri(str);
            uriBuilder.Host = uri.Host;
            uriBuilder.Path = uri.PathAndQuery;
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