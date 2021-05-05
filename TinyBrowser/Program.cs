namespace TinyBrowser
{
    static class Program
    {
        static void Main(string[] args) {
            Browser browser = new Browser();
            bool quitRequested = false;
            
            while (!quitRequested)
                quitRequested = browser.Browse();
        }
    }
}