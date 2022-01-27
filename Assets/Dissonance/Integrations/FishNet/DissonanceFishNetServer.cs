using System;
using System.Runtime.CompilerServices;
using Dissonance.Integrations.FishNet.Broadcasts;
using Dissonance.Networking;
using FishNet;
using FishNet.Connection;
using FishNet.Managing.Server;
using FishNet.Transporting;

namespace Dissonance.Integrations.FishNet
{
    // A Server integration for Dissonance Voice
    public sealed class DissonanceFishNetServer : BaseServer<DissonanceFishNetServer, DissonanceFishNetClient, DissonanceFishNetConnection>
    {
        // Registers Dissonance data broadcast & subscribes to NetworkManager events
        public override void Connect()
        {
            ServerManager fishNetServer = InstanceFinder.ServerManager;
            fishNetServer.RegisterBroadcast<DissonanceFishNetBroadcast>(OnDissonanceDataReceived);
            fishNetServer.OnRemoteConnectionState += FishNetServerOnOnRemoteConnectionState;
            base.Connect();
        }

        // Unregister Dissonance data broadcast & unsubscribes from NetworkManager events
        public override void Disconnect()
        {
            ServerManager fishNetServer = InstanceFinder.ServerManager;
            fishNetServer.UnregisterBroadcast<DissonanceFishNetBroadcast>(OnDissonanceDataReceived);
            fishNetServer.OnRemoteConnectionState -= FishNetServerOnOnRemoteConnectionState;
            base.Disconnect();
        }


        // Sends data in a reliable way. Aggressive inlined due to it's just a wrapper
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override void SendReliable(DissonanceFishNetConnection connection, ArraySegment<byte> packet)
        {
            DissonanceFishNetBroadcast broadcast = new DissonanceFishNetBroadcast(packet);
            connection.FishNetConnection.Broadcast(broadcast);
        }
        
        // Sends data in an unreliable way. Aggressive inlined due to it's just a wrapper
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override void SendUnreliable(DissonanceFishNetConnection connection, ArraySegment<byte> packet)
        {
            DissonanceFishNetBroadcast broadcast = new DissonanceFishNetBroadcast(packet);
            connection.FishNetConnection.Broadcast(broadcast, true, Channel.Unreliable);
        }

        // I am unsure if I need it. Docs say that this method polls Dissonance-related packets.
        // If something won't work with networking, etc, I will take more care for this method...
        protected override void ReadMessages() { }
        
        // Callback when Dissonance broadcast arrives
        private void OnDissonanceDataReceived(NetworkConnection connection, DissonanceFishNetBroadcast broadcast)
        {
            // Wrap FishNet connection into Dissonance one, pass a packet & release buffer
            DissonanceFishNetConnection dissonanceConn = new DissonanceFishNetConnection(connection);
            NetworkReceivedPacket(dissonanceConn, broadcast.Payload);
            broadcast.RelaseBuffer();
        }
        
        // Called when FishNet client connects or disconnects
        private void FishNetServerOnOnRemoteConnectionState(NetworkConnection fishNetConnection, RemoteConnectionStateArgs newState)
        {
            // Client disconnected
            if (newState.ConnectionState == RemoteConnectionStates.Stopped)
            {
                DissonanceFishNetConnection dissonanceConnection = new DissonanceFishNetConnection(fishNetConnection);
                ClientDisconnected(dissonanceConnection);
            }
        }
    }
}