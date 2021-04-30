using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
namespace TinyBrowser
{
    class Program
    {
        static void Main(string[] args) {
            const string titleStartTag = "<title>";
            const string titleEndTag = "</title>";
            var host = "acme.com";
            var uri = "/";
            //string eol = Environment.NewLine;
            Console.WriteLine("Hello World!");
            var tcpClient = new TcpClient("acme.com", 80);
            var stream = tcpClient.GetStream();
            var streamwriter = new StreamWriter(stream, Encoding.ASCII);
            var request = $"GET {uri} HTTP/1.1\r\nHost: {host}\r\n\r\n";
            streamwriter.Write(request);
            var streamReader = new StreamReader(stream);
            var response = streamReader.ReadToEnd();

            var uriBuilder = new UriBuilder(null, host) {Path = uri};
            
            Console.WriteLine(response);
            Console.WriteLine( $"Opened {uriBuilder}");
            string title = string.Empty;
            
            Console.WriteLine($"Title is: {title}");
        }
        
        
            
        public static string FindTextBetweenTags(string content, string startTag, string endTag) {
            string result = string.Empty;
            var titleStartIndex = content.IndexOf(startTag);
            if (titleStartIndex != -1) {
                titleStartIndex += startTag.Length;
                var titleEndIndex = content.IndexOf(endTag);
                if (titleEndIndex > titleStartIndex) {
                    result = content[titleStartIndex..titleEndIndex]; //range lookup
                    //title = response.Substring(titleStartIndex , titleEndIndex - titleStartIndex );
                }
            }
            return result;
        }
    }
    

}
