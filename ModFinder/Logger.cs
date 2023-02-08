using ModFinder.Interfaces;

namespace ModFinder
{
    internal class Logger : ILogger
    {
        public void LogMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(message);
        }

        public void LogInformation(string message)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(message);
        }

        public void LogSuccess(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
        }

        public void LogError(string message)
        {

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
        }

    }
}
