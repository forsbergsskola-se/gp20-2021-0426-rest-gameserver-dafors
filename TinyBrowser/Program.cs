using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
namespace TinyBrowser
{
    static class Program
    {
        static void Main(string[] args) {
            const string titleStartTag = "<title>";
            const string titleEndTag = "</title>";
            const string host = "acme.com";
            const string uri = "/";
            
            var tcpClient = new TcpClient("acme.com", 80);
            var stream = tcpClient.GetStream();
            var streamWriter = new StreamWriter(stream, Encoding.ASCII);
            var request = $"GET {uri} HTTP/1.1\r\nHost: {host}\r\n\r\n";
            streamWriter.Write(request); // add data to the buffer
            streamWriter.Flush();        // send the buffered data
            
            var streamReader = new StreamReader(stream);
            var response = streamReader.ReadToEnd();
            
            var uriBuilder = new UriBuilder(null, host) {Path = uri};
            
            //Console.WriteLine(response);
            Console.WriteLine( $"------------------Opened {uriBuilder}------------------");
            string title = FindTextBetweenTags(response, titleStartTag, titleEndTag);
            Console.WriteLine($"Title is: {title}");
            // Console.WriteLine($"---------Strings from all occurrences of '{hrefStartTag}' and '{hrefEndTag}'---------");
            // foreach (string s in GetAllBetweenTags(response, hrefStartTag, hrefEndTag)) {
            //     Console.WriteLine(s);
            // }
            Console.WriteLine("------------------Printing hrefs------------------");
            var hrefDict = GetHrefDict(response);
            foreach (var kvp in hrefDict) {
                Console.WriteLine($"{kvp.Key} -> {kvp.Value.Item1}, {kvp.Value.Item2}");
            }
            Console.WriteLine($"select navigation target (number between {0} and {hrefDict.Count}). >");

            int input = -1;
            while (!(0 <= input && input <= hrefDict.Count)) {
                input = Convert.ToInt32(Console.ReadLine());
                if (!(0 <= input && input <= hrefDict.Count))
                    Console.WriteLine($"invalid input, input (number between {0} and {hrefDict.Count}). >");
            }
            Console.WriteLine($"Interpreting {input}...");
        }

        public static string FindTextBetweenTags(string inputText, string startTag, string endTag, int startOffset = 0) {
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
    }
}