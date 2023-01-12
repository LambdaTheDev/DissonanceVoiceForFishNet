using Dissonance.Networking;

namespace Dissonance.Integrations.FishNet.Utils
{
    // Helper class for Dissonance events logging
    internal static class LoggingHelper
    {
        public static readonly Log Logger = Logs.Create(LogCategory.Network, "FishNet");

        private const string RunningAsTemplate = "Running as: {0}!";
        private const string StoppingAsTemplate = "Stopping as: {0}!";


        public static void RunningAs(NetworkMode mode)
        {
            Logger.Info(string.Format(RunningAsTemplate, mode.ToString()));
        }

        public static void StoppingAs(NetworkMode mode)
        {
            Logger.Info(string.Format(StoppingAsTemplate, mode.ToString()));
        }
    }
}