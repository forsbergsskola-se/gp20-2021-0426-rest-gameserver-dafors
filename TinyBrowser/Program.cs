namespace TinyBrowser
{
    static class Program
    {
        static void Main(string[] args) {
            Browser browser = new Browser();
            bool running = true;
            while (running)
                browser.Browse();
        }
    }
}