using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
namespace TinyBrowser
{
    static class Program
    {
        static void Main(string[] args) {
            const string titleStartTag = "<title>";
            const string titleEndTag = "</title>";
            const string hrefStartTag = "<a href=\""; // works?
            const string hrefEndTag = "\"";
            const string host = "acme.com";
            const string uri = "/";
            
            //string eol = Environment.NewLine;
            Console.WriteLine("Hello World!");
            var tcpClient = new TcpClient("acme.com", 80);
            var stream = tcpClient.GetStream();
            var streamWriter = new StreamWriter(stream, Encoding.ASCII);
            var request = $"GET {uri} HTTP/1.1\r\nHost: {host}\r\n\r\n";
            streamWriter.Write(request); // add data to the buffer
            streamWriter.Flush();        // send the buffered data
            
            var streamReader = new StreamReader(stream);
            var response = streamReader.ReadToEnd();
            
            var uriBuilder = new UriBuilder(null, host) {Path = uri};
            
            Console.WriteLine(response);
            Console.WriteLine( $"Opened {uriBuilder}");
            string title = FindTextBetweenTags(response, titleStartTag, titleEndTag);
            Console.WriteLine($"Title is: {title}");
            Console.WriteLine($"Strings from all occurrences of '{hrefStartTag}' and '{hrefEndTag}'");
            foreach (string s in GetTextBetweenCharsFromString(response, hrefStartTag, hrefEndTag)) {
                Console.WriteLine(s);
            }
        }

        public static string FindTextBetweenTags(string inputText, string startTag, string endTag, int startOffset = 0) {
            string result = string.Empty;
            var startIndex = inputText.IndexOf(startTag, startOffset);
            if (startIndex != -1) {
                startIndex += startTag.Length;
                var endIndex = inputText.IndexOf(endTag, startIndex); // find next occurence of endTag
                result = inputText[startIndex..endIndex]; //range lookup
            }
            return result;
        }

        static IEnumerable<string> GetTextBetweenCharsFromString(string inputText, string startTag, string endTag) {
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
    }
}