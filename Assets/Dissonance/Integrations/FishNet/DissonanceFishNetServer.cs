using System;
using System.Diagnostics.CodeAnalysis;
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
			var serverManager = InstanceFinder.ServerManager;
			serverManager.UnregisterBroadcast<DissonanceFishNetBroadcast>(DissonanceFishNetComms.NullBroadcastReceivedHandler);
			serverManager.RegisterBroadcast<DissonanceFishNetBroadcast>(OnDissonanceDataReceived);
			serverManager.OnRemoteConnectionState += FishNetServerOnOnRemoteConnectionState;
			base.Connect();
		}

		// Unregister Dissonance data broadcast & unsubscribes from NetworkManager events
		public override void Disconnect()
		{
			var serverManager = InstanceFinder.ServerManager;
			if (serverManager != null)
			{
				serverManager.UnregisterBroadcast<DissonanceFishNetBroadcast>(OnDissonanceDataReceived);
				serverManager.RegisterBroadcast<DissonanceFishNetBroadcast>(DissonanceFishNetComms.NullBroadcastReceivedHandler);
				serverManager.OnRemoteConnectionState -= FishNetServerOnOnRemoteConnectionState;
			}
			base.Disconnect();
		}

		// Check if a connection is actually connected
		private static bool IsConnected(NetworkConnection conn)
		{
			return conn.FirstObject != null && conn.IsActive && InstanceFinder.ServerManager.Clients.ContainsKey(conn.ClientId);
		}


		// Sends data in a reliable way. Aggressive inlined due to it's just a wrapper
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override void SendReliable(DissonanceFishNetConnection connection, ArraySegment<byte> packet)
		{
			if (IsConnected(connection.FishNetConnection))
			{
				DissonanceFishNetBroadcast broadcast = new DissonanceFishNetBroadcast(packet);
				connection.FishNetConnection.Broadcast(broadcast);
			}
		}

		// Sends data in an unreliable way. Aggressive inlined due to it's just a wrapper
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override void SendUnreliable(DissonanceFishNetConnection connection, ArraySegment<byte> packet)
		{
			if (IsConnected(connection.FishNetConnection))
			{
				DissonanceFishNetBroadcast broadcast = new DissonanceFishNetBroadcast(packet);
				connection.FishNetConnection.Broadcast(broadcast, true, Channel.Unreliable);
			}
		}

        // Not needed in FishNet
		protected override void ReadMessages() { }

		// Callback when Dissonance broadcast arrives
		private void OnDissonanceDataReceived(NetworkConnection connection, DissonanceFishNetBroadcast broadcast)
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
			if (newState.ConnectionState == RemoteConnectionStates.Stopped)
			{
				DissonanceFishNetConnection dissonanceConnection = new DissonanceFishNetConnection(fishNetConnection);
				ClientDisconnected(dissonanceConnection);
			}
		}
	}
}