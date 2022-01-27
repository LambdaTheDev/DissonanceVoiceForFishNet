using System;
using System.Runtime.CompilerServices;
using Dissonance.Integrations.FishNet.Broadcasts;
using Dissonance.Networking;
using FishNet;
using FishNet.Transporting;
using JetBrains.Annotations;

namespace Dissonance.Integrations.FishNet
{
    // A Client integration for Dissonance Voice
    public sealed class DissonanceFishNetClient : BaseClient<DissonanceFishNetServer, DissonanceFishNetClient, DissonanceFishNetConnection>
    {
        public DissonanceFishNetClient([NotNull] ICommsNetworkState network) : base(network)
        {
        }
        
        // Register broadcast & mark Dissonance client as connected
        public override void Connect()
        {
            // EXACTLY SAME PROBLEM AS WITH SENDING DATA!!!
            InstanceFinder.ClientManager.RegisterBroadcast<DissonanceFishNetBroadcast>(OnDissonanceDataReceived);
            Connected();
        }

        // Unregisters broadcast
        public override void Disconnect()
        {
            // EHH... I AM TIRED OF WRITING THIS. YOU KNOW, YE?
            InstanceFinder.ClientManager.UnregisterBroadcast<DissonanceFishNetBroadcast>(OnDissonanceDataReceived);
            base.Disconnect();
        }

        // Sends data in a reliable way
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override void SendReliable(ArraySegment<byte> packet)
        {
            // TODO: TODO: MULTI-SERVER INSTANCE SUPPORT NOW CAUSES A BIG PROBLEM.
            // Sending data will currently work only on the first (main Client-Server) networking instance
            // For now, I will just make it work, but this must be resolved.
            DissonanceFishNetBroadcast broadcast = new DissonanceFishNetBroadcast(packet);
            InstanceFinder.ClientManager.Broadcast(broadcast);
        }

        // Sends data in an unreliable way
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override void SendUnreliable(ArraySegment<byte> packet)
        {
            // SAME PROBLEM AS ABOVE
            DissonanceFishNetBroadcast broadcast = new DissonanceFishNetBroadcast(packet);
            InstanceFinder.ClientManager.Broadcast(broadcast, Channel.Unreliable);
        }
        
        // Look at this method equivalent in DissFishNetServer for more info why it's empty
        protected override void ReadMessages() { }

        // Callback when Dissonance broadcasts arrives
        private void OnDissonanceDataReceived(DissonanceFishNetBroadcast broadcast)
        {
            // I implemented IDisposable there for QOL proposes
            NetworkReceivedPacket(broadcast.Payload);
            broadcast.ReleaseBuffer();
        }
    }
}