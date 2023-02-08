namespace ModFinder.Interfaces
{
    internal interface ILogger
    {
        void LogInformation(string message);
        void LogSuccess(string message);
        void LogError(string message);
        void LogMessage(string message);
    }
}