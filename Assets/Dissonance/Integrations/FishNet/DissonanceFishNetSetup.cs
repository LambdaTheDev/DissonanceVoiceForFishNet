using System;
using FishNet;
using FishNet.Object;
using FishNet.Transporting;
using UnityEngine;

namespace Dissonance.Integrations.FishNet
{
    // A drag-and-drop solution for Dissonance Voice chat setup with FishNet
    // NOTE: This prevents incidental creation of Dissonance while FishNet is offline!
    public class DissonanceFishNetSetup : MonoBehaviour
    {
        [Tooltip("If this GameObject should be made DDOL, make this option true.")] 
        public bool makeDontDestroy = true;
        
        [Tooltip("Keep it true unless you know what you are doing. If true, it checks if Dissonance object is root in hierarchy.")]
        public bool ensureRootObject = true;

        private bool _subscribedToFn;
        
        public DissonanceComms DissonanceComms { get; private set; }
        public DissonanceFishNetComms FishNetComms { get; private set; }
        

        private void Awake()
        {
            // Check if this object is root
            // ReSharper disable once Unity.InefficientPropertyAccess
            bool rootObject = transform.parent == null;
            
            if (ensureRootObject && !rootObject)
                throw new Exception("This GameObject is not a root GameObject! You can try disabling ensureRootObject option, if you know what you are doing.");

            if (makeDontDestroy)
            {
                if(!rootObject) throw new Exception("Only root objects can be made DontDestroyOnLoad! You can try disabling makeDontDestroy option!");
                DontDestroyOnLoad(gameObject);
            }
            
            // Check if NetworkObject component is present. It must NOT be one
            NetworkObject networkObj = transform.root.GetComponent<NetworkObject>();
            if (networkObj != null)
                throw new Exception("GameObject with Dissonance Voice cannot be a NetworkObject!");
            
            // Setup FishNetworking
            SetupWithFishNet();
        }

        private void SetupWithFishNet()
        {
            // Optimistic scenario - FishNet is already online. Now, we will only add Dissonance components
            if (!InstanceFinder.IsOffline)
            {
                AddDissonanceComponents();
                return;
            }
            
            // Now, that worse scenario, we must detect when network starts...
            ManageFishNetEvents(true);
        }

        private void ManageFishNetEvents(bool subscribe)
        {
            if(_subscribedToFn && subscribe)
                return;

            if (!_subscribedToFn && !subscribe)
                return;

            if (subscribe)
            {
                InstanceFinder.ClientManager.OnClientConnectionState += ClientManagerOnOnClientConnectionState;
                InstanceFinder.ServerManager.OnServerConnectionState += ServerManagerOnOnServerConnectionState;
            }
            else
            {
                InstanceFinder.ClientManager.OnClientConnectionState -= ClientManagerOnOnClientConnectionState;
                InstanceFinder.ServerManager.OnServerConnectionState -= ServerManagerOnOnServerConnectionState;
            }

            _subscribedToFn = !_subscribedToFn;
        }

        private void ServerManagerOnOnServerConnectionState(ServerConnectionStateArgs obj) =>
            OnFishNetStateDirty(obj.ConnectionState);

        private void ClientManagerOnOnClientConnectionState(ClientConnectionStateArgs obj) =>
            OnFishNetStateDirty(obj.ConnectionState);

        // Fired when FishNet state got dirty (in our case, when it starts/is started)
        private void OnFishNetStateDirty(LocalConnectionState state)
        {
            // If anything started, add Dissonance components. Later adjustments will be handled by the integration code
            if (state != LocalConnectionState.Started) return;
            
            // From now, DissonanceFishNetComms will track networking state.
            ManageFishNetEvents(false);
            AddDissonanceComponents();
        }

        private void AddDissonanceComponents()
        {
            // WARN: FishNet comms MUST be first
            FishNetComms = gameObject.AddComponent<DissonanceFishNetComms>();
            DissonanceComms = gameObject.AddComponent<DissonanceComms>();
            FishNetComms.InitializeDissonanceComms();
        }
    }
}