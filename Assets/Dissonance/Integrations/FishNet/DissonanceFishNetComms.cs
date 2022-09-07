using Dissonance.Integrations.FishNet.Broadcasts;
using Dissonance.Integrations.FishNet.Utils;
using Dissonance.Networking;
using FishNet;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Transporting;
using UnityEngine;

namespace Dissonance.Integrations.FishNet
{
	// Comms implementation for Dissonance Voice. Used Unit, due to FishNet is connected while using Dissonance Voice
	public sealed class DissonanceFishNetComms : BaseCommsNetwork<DissonanceFishNetServer, DissonanceFishNetClient, DissonanceFishNetConnection, Unit, Unit>
	{
		public static DissonanceFishNetComms Instance { get; private set; }

		public DissonanceComms Comms { get; private set; }
        internal NetworkManager NetworkManager { get; private set; }
        
        private bool _subscribed;


		private void Awake()
		{
            // Check if there is no double comms instance
            if (Instance != null)
            {
                LoggingHelper.Logger.Error("Only one DissonanceFishNetComms instance is allowed to exist at the time! This one will not work.");
                return;
            }

            // Initialize this comms instance & log
			Instance = this;
			Comms = GetComponent<DissonanceComms>();
            NetworkManager = InstanceFinder.NetworkManager;
            LoggingHelper.Logger.Info("FishNet comms initialized successfully!");
        }

        private void OnEnable()
        {
            ManageNetworkEvents(true);
        }

        protected override void OnDisable()
        {
            ManageNetworkEvents(false);
        }

        protected override void Initialize()
		{
            // Register no broadcast handler so errors can be captured easier
            NetworkManager.ServerManager.RegisterBroadcast<DissonanceFishNetBroadcast>(NullBroadcastReceivedHandler);
			NetworkManager.ClientManager.RegisterBroadcast<DissonanceFishNetBroadcast>(NullBroadcastReceivedHandler);

            // Now, start Dissonance Voice, depending on current FishNet state
            if (NetworkManager.IsServer)
            {
                // Run host or dedicated server
                if (NetworkManager.IsHost)
                    RunAsHost(Unit.None, Unit.None);
                
                else
                    RunAsDedicatedServer(Unit.None);

                // Log
                LoggingHelper.RunningAs(NetworkManager.IsHost ? NetworkMode.Host : NetworkMode.DedicatedServer);
            }
            else
            {
                RunAsClient(Unit.None);
                LoggingHelper.RunningAs(NetworkMode.Client);
            }
        }

		protected override DissonanceFishNetServer CreateServer(Unit connectionParameters)
		{
            LoggingHelper.Logger.Trace("Creating FishNet server...");
			return new DissonanceFishNetServer(this);
		}

		protected override DissonanceFishNetClient CreateClient(Unit connectionParameters)
		{
            LoggingHelper.Logger.Trace("Creating FishNet client...");
			return new DissonanceFishNetClient(this);
		}

        // Helper method that subscribes or unsubscribed 
        private void ManageNetworkEvents(bool subscribe)
        {
            if (subscribe && !_subscribed)
            {
                NetworkManager.ServerManager.OnServerConnectionState += ServerManagerOnOnServerConnectionState;
                NetworkManager.ClientManager.OnClientConnectionState += ClientManagerOnOnClientConnectionState;

                _subscribed = true;
            }
            else if(!subscribe && _subscribed)
            {
                NetworkManager.ServerManager.OnServerConnectionState -= ServerManagerOnOnServerConnectionState;
                NetworkManager.ClientManager.OnClientConnectionState -= ClientManagerOnOnClientConnectionState;

                _subscribed = false;
            }
        }

        #region Dissonance network peer status callbacks

        private void ClientManagerOnOnClientConnectionState(ClientConnectionStateArgs obj)
        {
            if (obj.ConnectionState == LocalConnectionState.Stopped)
            {
                Stop();
                LoggingHelper.StoppingAs(NetworkMode.Client);
            }
        }

        private void ServerManagerOnOnServerConnectionState(ServerConnectionStateArgs obj)
        {
            if (obj.ConnectionState == LocalConnectionState.Stopped)
            {
                // Stop dissonance & log
                Stop();
                LoggingHelper.StoppingAs(NetworkManager.IsHost ? NetworkMode.Host : NetworkMode.DedicatedServer);
            }
        }

        #endregion

        #region Debugging

        internal static void NullBroadcastReceivedHandler(NetworkConnection source, DissonanceFishNetBroadcast broadcast) => NullBroadcastLogger(broadcast);
        internal static void NullBroadcastReceivedHandler(DissonanceFishNetBroadcast broadcast) => NullBroadcastLogger(broadcast);

        private static void NullBroadcastLogger(DissonanceFishNetBroadcast broadcast)
        {
            LoggingHelper.Logger.Debug("Dissonance comms instance has not been initialized! Disregarding incoming packet.");
            broadcast.ReleaseBuffer();
        }

        #endregion
    }
}