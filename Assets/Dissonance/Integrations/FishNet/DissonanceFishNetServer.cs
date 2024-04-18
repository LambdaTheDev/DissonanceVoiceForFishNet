using System;
using System.Runtime.CompilerServices;
using Dissonance.Integrations.FishNet.Broadcasts;
using Dissonance.Integrations.FishNet.Utils;
using Dissonance.Networking;
using FishNet.Connection;
using FishNet.Transporting;

namespace Dissonance.Integrations.FishNet
{
	// A Server integration for Dissonance Voice
	public sealed class DissonanceFishNetServer : BaseServer<DissonanceFishNetServer, DissonanceFishNetClient, DissonanceFishNetConnection>
    {
        private readonly DissonanceFishNetComms _fishNetComms;


        public DissonanceFishNetServer(DissonanceFishNetComms network)
        {
            _fishNetComms = network;
        }
        
		// Registers Dissonance data broadcast & subscribes to NetworkManager events
		public override void Connect()
		{
			var serverManager = _fishNetComms.NetworkManager.ServerManager;
			serverManager.UnregisterBroadcast<DissonanceFishNetBroadcast>(DissonanceFishNetComms.NullBroadcastReceivedHandler);
			serverManager.RegisterBroadcast<DissonanceFishNetBroadcast>(OnDissonanceDataReceived);
			serverManager.OnRemoteConnectionState += FishNetServerOnOnRemoteConnectionState;
			base.Connect();
            
            LoggingHelper.Logger.Debug("Server is ready!");
		}

		// Unregister Dissonance data broadcast & unsubscribes from NetworkManager events
		public override void Disconnect()
		{
			var serverManager = _fishNetComms.NetworkManager.ServerManager;
			if (serverManager != null)
			{
				serverManager.UnregisterBroadcast<DissonanceFishNetBroadcast>(OnDissonanceDataReceived);
				serverManager.RegisterBroadcast<DissonanceFishNetBroadcast>(DissonanceFishNetComms.NullBroadcastReceivedHandler);
				serverManager.OnRemoteConnectionState -= FishNetServerOnOnRemoteConnectionState;
			}
			base.Disconnect();
            
            LoggingHelper.Logger.Debug("Server stopped!");
		}
		
		// Sends data in a reliable way. Aggressive inlined due to it's just a wrapper
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override void SendReliable(DissonanceFishNetConnection connection, ArraySegment<byte> packet)
		{
			if (!connection.FishNetConnection.IsActive) return;
            DissonanceFishNetBroadcast broadcast = BroadcastHelper.CreateFromOriginalData(packet);
			connection.FishNetConnection.Broadcast(broadcast);
            broadcast.ReleaseBuffer();
		}

		// Sends data in an unreliable way. Aggressive inlined due to it's just a wrapper
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override void SendUnreliable(DissonanceFishNetConnection connection, ArraySegment<byte> packet)
		{
			if (!connection.FishNetConnection.IsActive) return;
            DissonanceFishNetBroadcast broadcast = BroadcastHelper.CreateFromOriginalData(packet);
			connection.FishNetConnection.Broadcast(broadcast, true, Channel.Unreliable);
            broadcast.ReleaseBuffer();
        }

        // Not needed in FishNet
		protected override void ReadMessages() { }

		// Callback when Dissonance broadcast arrives
		private void OnDissonanceDataReceived(NetworkConnection connection, DissonanceFishNetBroadcast broadcast, Channel channel)
		{
			// Wrap FishNet connection into Dissonance one, pass a packet & release buffer
			DissonanceFishNetConnection dissonanceConn = new DissonanceFishNetConnection(connection);
			NetworkReceivedPacket(dissonanceConn, broadcast.Payload);
			broadcast.ReleaseBuffer();
		}

		// Called when FishNet client connects or disconnects
		private void FishNetServerOnOnRemoteConnectionState(NetworkConnection fishNetConnection, RemoteConnectionStateArgs newState)
		{
			// Client disconnected
			if (newState.ConnectionState != RemoteConnectionState.Stopped) return;
			DissonanceFishNetConnection dissonanceConnection = new DissonanceFishNetConnection(fishNetConnection);
			ClientDisconnected(dissonanceConnection);
            LoggingHelper.Logger.Info("Remote client with ID: {0} has disconnected!", fishNetConnection.ClientId);
        }
	}
}