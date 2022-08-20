using System.Runtime.CompilerServices;
using Dissonance.Integrations.FishNet.Exceptions;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

namespace Dissonance.Integrations.FishNet{
    // A Player object wrapper for Dissonance Voice
    public sealed class DissonanceFishNetPlayer : NetworkBehaviour, IDissonancePlayer{
        private DissonanceComms _comms;

        public bool IsTracking { get; private set; }
        [SyncVar]
        private string _playerId;
        public string PlayerId { get { return _playerId; } }

        public Vector3 Position
        {
            get { return transform.position; }
        }

        public Quaternion Rotation
        {
            get { return transform.rotation; }
        }

        public NetworkPlayerType Type
        {
            get
            {
                if (_comms == null || _playerId == null)
                    return NetworkPlayerType.Unknown;
                return _comms.LocalPlayerName.Equals(_playerId) ? NetworkPlayerType.Local : NetworkPlayerType.Remote;
            }
        }

        public void OnDestroy()
        {
            if (_comms != null)
                _comms.LocalPlayerNameChanged -= SetPlayerName;
        }

        public void OnEnable()
        {
            DissonanceFishNetComms fishNetComms = DissonanceFishNetComms.Instance;
            _comms = fishNetComms.Comms;
        }

        public void OnDisable()
        {
            if (IsTracking)
                StopTracking();
        }

        public override void OnOwnershipClient(NetworkConnection prevOwner)
        {
            base.OnOwnershipClient(prevOwner);

            DissonanceFishNetComms fishNetComms = DissonanceFishNetComms.Instance;
            _comms = fishNetComms.Comms;
            if (_comms == null){
                Debug.Log("NO COMMS");
            }

            Debug.Log("Tracking " + _comms.LocalPlayerName);

            // This method is called on the client which has control authority over this object. This will be the local client of whichever player we are tracking.
            if (_comms.LocalPlayerName != null)
                SetPlayerName(_comms.LocalPlayerName);

            //Subscribe to future name changes (this is critical because we may not have run the initial set name yet and this will trigger that initial call)
            _comms.LocalPlayerNameChanged += SetPlayerName;
        }

        private void SetPlayerName(string playerName)
        {
            //We need the player name to be set on all the clients and then tracking to be started (on each client).
            //To do this we send a command from this client, informing the server of our name. The server will pass this on to all the clients (with an RPC)
            // Client -> Server -> Client

            //We need to stop and restart tracking to handle the name change
            if (IsTracking)
                StopTracking();

            //Perform the actual work
            _playerId = playerName;
            StartTracking();

            //Inform the server the name has changed
            if (base.IsOwner)
                CmdSetPlayerName(playerName);
        }

        public override void OnStartClient()
        {
            base.OnStartClient();

            //A client is starting. Start tracking if the name has been properly initialised.
            if (!string.IsNullOrEmpty(PlayerId))
                StartTracking();
        }

        /// <summary>
        /// Invoking on client will cause it to run on the server
        /// </summary>
        /// <param name="playerName"></param>
        [ServerRpc]
        private void CmdSetPlayerName(string playerName)
        {
            _playerId = playerName;

            //Now call the RPC to inform clients they need to handle this changed value
            RpcSetPlayerName(playerName);
        }

        /// <summary>
        /// Invoking on the server will cause it to run on all the clients
        /// </summary>
        /// <param name="playerName"></param>
        [ObserversRpc]
        private void RpcSetPlayerName(string playerName)
        {
            //received a message from server (on all clients). If this is not the local player then apply the change
            if (!base.IsOwner)
                SetPlayerName(playerName);
        }

        private void StartTracking()
        {
            if (IsTracking)
                Debug.Log("Attempting to start player tracking, but tracking is already started");

            if (_comms != null)
            {
                _comms.TrackPlayerPosition(this);
                IsTracking = true;
            }
        }

        private void StopTracking()
        {
            if (!IsTracking)
                Debug.Log("Attempting to stop player tracking, but tracking is not started");

            if (_comms != null)
            {
                _comms.StopTracking(this);
                IsTracking = false;
            }
        }

    }
}