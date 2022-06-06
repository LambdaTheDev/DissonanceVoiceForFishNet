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
		public DissonanceFishNetClient([NotNull] ICommsNetworkState network) : base(network) { }

		// Register broadcast & mark Dissonance client as connected
		public override void Connect()
		{
			var clientManager = InstanceFinder.ClientManager;
			clientManager.UnregisterBroadcast<DissonanceFishNetBroadcast>(DissonanceFishNetComms.NullBroadcastReceivedHandler);
			clientManager.RegisterBroadcast<DissonanceFishNetBroadcast>(OnDissonanceDataReceived);
			Connected();
		}

		// Unregisters broadcast
		public override void Disconnect()
		{
			var clientManager = InstanceFinder.ClientManager;
			if (clientManager != null)
			{
				clientManager.UnregisterBroadcast<DissonanceFishNetBroadcast>(OnDissonanceDataReceived);
				clientManager.RegisterBroadcast<DissonanceFishNetBroadcast>(DissonanceFishNetComms.NullBroadcastReceivedHandler);
			}
			base.Disconnect();
		}

		// Sends data in a reliable way
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override void SendReliable(ArraySegment<byte> packet)
		{
			DissonanceFishNetBroadcast broadcast = new DissonanceFishNetBroadcast(packet);
			InstanceFinder.ClientManager.Broadcast(broadcast);
		}

		// Sends data in an unreliable way
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override void SendUnreliable(ArraySegment<byte> packet)
		{
			DissonanceFishNetBroadcast broadcast = new DissonanceFishNetBroadcast(packet);
			InstanceFinder.ClientManager.Broadcast(broadcast, Channel.Unreliable);
		}

        // Not needed in FishNet
		protected override void ReadMessages() { }

		// Callback when Dissonance broadcasts arrives
		private void OnDissonanceDataReceived(DissonanceFishNetBroadcast broadcast)
		{
			NetworkReceivedPacket(broadcast.Payload);
			broadcast.ReleaseBuffer();
		}
	}
}