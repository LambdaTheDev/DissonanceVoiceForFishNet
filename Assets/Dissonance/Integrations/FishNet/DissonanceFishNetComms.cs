using Dissonance.Integrations.FishNet.Broadcasts;
using Dissonance.Integrations.FishNet.Constants;
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

        private NetworkManager _networkManager;
        private bool _subscribed;


		private void Awake()
		{
			if (Instance != null)
				Debug.LogError("Dissonance Voice Chat for FishNet supports only one DissonanceComms object instance at time! " +
							   "If this is a big problem for you, contact me on Discord: " + DissonanceFishNetConstants.SupportDiscordServer);

			Instance = this;
			Comms = GetComponent<DissonanceComms>();
            _networkManager = InstanceFinder.NetworkManager;
        }

		protected override void Initialize()
		{
            // Register no broadcast handler so errors can be captured easier
            _networkManager.ServerManager.RegisterBroadcast<DissonanceFishNetBroadcast>(NullBroadcastReceivedHandler);
			_networkManager.ClientManager.RegisterBroadcast<DissonanceFishNetBroadcast>(NullBroadcastReceivedHandler);

            // Subscribe to NetworkManager events
            ManageNetworkEvents(true);
        }

		protected override DissonanceFishNetServer CreateServer(Unit connectionParameters)
		{
			return new DissonanceFishNetServer();
		}

		protected override DissonanceFishNetClient CreateClient(Unit connectionParameters)
		{
			return new DissonanceFishNetClient(this);
		}

        //
        // EXPERIMENT: GETTING RID OF THIS METHOD & CHECKING IF CALLBACKS ARE WORKING PROPERLY
        //
        
        // I'd like to get rid of this & rely on FishNet callbacks, but first I will make it work, then I'm gonna upgrade it
		// private void UpdateNetwork()
		// {
		// 	// Check if Dissonance is ready
		// 	if (IsInitialized)
		// 	{
		// 		var networkManager = InstanceFinder.NetworkManager;
		// 		// Check if the Network is ready
		// 		var networkActive = networkManager != null && !networkManager.IsOffline;
		// 		if (networkActive)
		// 		{
		// 			// Check what mode the Network is in
		// 			var server = networkManager.IsServer;
		// 			var client = networkManager.IsClient;
		//
		// 			// Check what mode Dissonance is in and if
		// 			// they're different then call the correct method
		// 			if (Mode.IsServerEnabled() != server
		// 				|| Mode.IsClientEnabled() != client)
		// 			{
		// 				// Network is server and client, so run as a non-dedicated
		// 				// host (passing in the correct parameters)
		// 				if (server && client)
		// 					RunAsHost(Unit.None, Unit.None);
		//
		// 				// Network is just a server, so run as a dedicated host
		// 				else if (server)
		// 					RunAsDedicatedServer(Unit.None);
		//
		// 				// Network is just a client, so run as a client
		// 				else if (client)
		// 					RunAsClient(Unit.None);
		// 			}
		// 		}
		// 		else if (Mode != NetworkMode.None)
		// 		{
		// 			//Network is not active, make sure Dissonance is not active
		// 			Stop();
		// 		}
		// 	}
		// }

        // Helper method that subscribes or unsubscribed 
        private void ManageNetworkEvents(bool subscribe)
        {
            if (subscribe && !_subscribed)
            {
                _networkManager.ServerManager.OnServerConnectionState += ServerManagerOnOnServerConnectionState;
                _networkManager.ClientManager.OnClientConnectionState += ClientManagerOnOnClientConnectionState;

                _subscribed = true;
            }
            else if(!subscribe && _subscribed)
            {
                _networkManager.ServerManager.OnServerConnectionState -= ServerManagerOnOnServerConnectionState;
                _networkManager.ClientManager.OnClientConnectionState -= ClientManagerOnOnClientConnectionState;

                _subscribed = false;
            }
        }

        #region Dissonance network peer status callbacks

        private void ClientManagerOnOnClientConnectionState(ClientConnectionStateArgs obj)
        {
            if (obj.ConnectionState == LocalConnectionState.Started)
            {
                RunAsClient(Unit.None);
            }
            else if (obj.ConnectionState == LocalConnectionState.Stopped)
            {
                Stop();
                ManageNetworkEvents(false);
            }
        }

        private void ServerManagerOnOnServerConnectionState(ServerConnectionStateArgs obj)
        {
            if (obj.ConnectionState == LocalConnectionState.Started)
            {
                if(_networkManager.IsHost)
                    RunAsHost(Unit.None, Unit.None);
                else
                    RunAsDedicatedServer(Unit.None);
            }
            else if (obj.ConnectionState == LocalConnectionState.Stopped)
            {
                Stop();
                ManageNetworkEvents(false);
            }
        }

        #endregion

        #region Debugging

        internal static void NullBroadcastReceivedHandler(NetworkConnection source, DissonanceFishNetBroadcast broadcast) => NullBroadcastLogger(broadcast);
        internal static void NullBroadcastReceivedHandler(DissonanceFishNetBroadcast broadcast) => NullBroadcastLogger(broadcast);

        private static void NullBroadcastLogger(DissonanceFishNetBroadcast broadcast)
        {
            if (Logs.GetLogLevel(LogCategory.Network) <= LogLevel.Trace)
                Debug.Log("Dissonance client not initialized - Discarding Dissonance server broadcast");

            broadcast.ReleaseBuffer();
        }

        #endregion
	}
}