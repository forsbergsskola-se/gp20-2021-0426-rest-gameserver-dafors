namespace TinyBrowser
{
    static class Program
    {
        static void Main(string[] args) {
            Browser browser = new Browser();
            bool browsing = true;
           
            while (browsing)
                browsing = browser.Browse();
        }
    }
}