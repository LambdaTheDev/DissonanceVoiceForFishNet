using Dissonance.Integrations.FishNet.Utils;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

namespace Dissonance.Integrations.FishNet
{
    // A Player object wrapper for Dissonance Voice
    public sealed class DissonanceFishNetPlayer : NetworkBehaviour, IDissonancePlayer
    {
        [Tooltip("This transform will be used in positional voice processing. If unset, then GameObject's transform will be used.")]
        [SerializeField] private Transform trackingTransform;
        
        // SyncVar ensures that all observers know player ID, even late joiners
        [SyncVar(OnChange = nameof(OnPlayerIdHookFired))] private string _syncedPlayerId;

        // Captured DissonanceComms instance
        private DissonanceComms _comms;

        public string PlayerId => _syncedPlayerId;
        public Vector3 Position => trackingTransform.position;
        public Quaternion Rotation => trackingTransform.rotation;
        public NetworkPlayerType Type => IsOwner ? NetworkPlayerType.Local : NetworkPlayerType.Remote;

        public bool IsTracking { get; private set; }


        private void Start()
        {
            if (trackingTransform == null) trackingTransform = transform;
        }

        // Called by FishNet when object is spawned on client with authority
        public override void OnOwnershipClient(NetworkConnection prevOwner)
        {
            base.OnOwnershipClient(prevOwner);
            
            DissonanceFishNetComms fishNetComms = DissonanceFishNetComms.Instance;
            if (fishNetComms == null)
            {
                LoggingHelper.Logger.Error("Could not find any DissonanceFishNetComms instance! This DissonancePlayer instance will not work!");
                return;
            }

            // First config player ID
            _comms = fishNetComms.Comms;
            ServerRpcSetPlayerId(_comms.LocalPlayerName);
            _comms.LocalPlayerNameChanged += ServerRpcSetPlayerId;
            
            _comms.TrackPlayerPosition(this);
            IsTracking = true;
        }

        // Invoked when Player ID changes (or is set by server)
        private void OnPlayerIdHookFired(string _, string __, bool ___)
        {
            if (_comms == null)
                return;
            
            if (IsTracking)
            {
                _comms.StopTracking(this);
                IsTracking = false;
            }
            
            _comms.TrackPlayerPosition(this);
            IsTracking = true;
        }

        [ServerRpc]
        private void ServerRpcSetPlayerId(string playerId)
        {
            _syncedPlayerId = playerId;
        }
    }
}