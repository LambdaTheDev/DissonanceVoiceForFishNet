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
            switch (mode)
            {
                case NetworkMode.Client:
                    Logger.Info(string.Format(RunningAsTemplate, "CLIENT"));
                    break;

                case NetworkMode.DedicatedServer:
                    Logger.Info(string.Format(RunningAsTemplate, "SERVER"));
                    break;

                case NetworkMode.Host:
                    Logger.Info(string.Format(RunningAsTemplate, "HOST"));
                    break;
            }
        }

        public static void StoppingAs(NetworkMode mode)
        {
            switch (mode)
            {
                case NetworkMode.Client:
                    Logger.Info(string.Format(StoppingAsTemplate, "CLIENT"));
                    break;

                case NetworkMode.DedicatedServer:
                    Logger.Info(string.Format(StoppingAsTemplate, "SERVER"));
                    break;

                case NetworkMode.Host:
                    Logger.Info(string.Format(StoppingAsTemplate, "HOST"));
                    break;
            }        
        }
    }
}