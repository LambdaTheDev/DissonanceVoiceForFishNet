using System;

namespace Dissonance.Integrations.FishNet.Exceptions
{
    // Exception thrown when something goes wrong in integration
    public class DissonanceFishNetException : Exception
    {
        public DissonanceFishNetException(string msg) : base(msg) { }
    }
}