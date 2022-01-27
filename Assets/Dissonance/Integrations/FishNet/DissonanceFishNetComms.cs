using Dissonance.Networking;

namespace Dissonance.Integrations.FishNet
{
    // Comms implementation for Dissonance Voice. Used Unit, due to FishNet is connected while using Dissonance Voice
    public sealed class DissonanceFishNetComms : BaseCommsNetwork<DissonanceFishNetServer, DissonanceFishNetClient, DissonanceFishNetConnection, Unit, Unit>
    {
        protected override DissonanceFishNetServer CreateServer(Unit connectionParameters)
        {
            return new DissonanceFishNetServer();
        }

        protected override DissonanceFishNetClient CreateClient(Unit connectionParameters)
        {
            return new DissonanceFishNetClient(this);
        }
    }
}