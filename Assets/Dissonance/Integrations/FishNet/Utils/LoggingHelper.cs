using Dissonance.Networking;

namespace Dissonance.Integrations.FishNet.Utils
{
    // Helper class for Dissonance events logging
    internal static class LoggingHelper
    {
        public static readonly Log Logger = Logs.Create(LogCategory.Network, "FishNet");

        private const string RunningAsTemplate = "Running as: {0}!";
        private static readonly string RunningAsClient = string.Format(RunningAsTemplate, "CLIENT");
        private static readonly string RunningAsServer = string.Format(RunningAsTemplate, "SERVER");
        private static readonly string RunningAsHost = string.Format(RunningAsTemplate, "HOST");

        private const string StoppingAsTemplate = "Stopping as: {0}!";
        private static readonly string StoppingAsClient = string.Format(StoppingAsTemplate, "CLIENT");
        private static readonly string StoppingAsServer = string.Format(StoppingAsTemplate, "SERVER");
        private static readonly string StoppingAsHost = string.Format(StoppingAsTemplate, "HOST");
        

        public static void RunningAs(NetworkMode mode)
        {
            switch (mode)
            {
                case NetworkMode.Client:
                    Logger.Info(RunningAsClient);
                    break;

                case NetworkMode.DedicatedServer:
                    Logger.Info(RunningAsServer);
                    break;

                case NetworkMode.Host:
                    Logger.Info(RunningAsHost);
                    break;
            }
        }

        public static void StoppingAs(NetworkMode mode)
        {
            switch (mode)
            {
                case NetworkMode.Client:
                    Logger.Info(StoppingAsClient);
                    break;

                case NetworkMode.DedicatedServer:
                    Logger.Info(StoppingAsServer);
                    break;

                case NetworkMode.Host:
                    Logger.Info(StoppingAsHost);
                    break;
            }        
        }
    }
}