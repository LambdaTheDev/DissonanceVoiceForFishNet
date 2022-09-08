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
        // SyncVar ensures that all observers know player ID, even late joiners
        [SyncVar(OnChange = nameof(OnPlayerIdHookFired))] private string _syncedPlayerId;

        // Captured DissonanceComms instance
        private DissonanceComms _comms;

        public string PlayerId => _syncedPlayerId;

        // IMPORTANT NOTE: When Punfish finishes child NetworkObjects, I will have 2 options:
        // A) Make this transform.root.position - less performance
        // B) Make a check if it's a root object & not allow new FishNet's functionality
        //
        // But we will see in future, now this should work.
        public Vector3 Position => transform.position;
        public Quaternion Rotation => transform.rotation;
        public NetworkPlayerType Type => IsOwner ? NetworkPlayerType.Local : NetworkPlayerType.Remote;

        public bool IsTracking { get; private set; }


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

            _comms = fishNetComms.Comms;
            _comms.TrackPlayerPosition(this);
        }

        // Sets Player ID to owner ID
        public override void OnStartServer()
        {
            base.OnStartServer();
            _syncedPlayerId = OwnerId.ToString();
        }

        // Invoked when Player ID changes (or is set by server)
        private void OnPlayerIdHookFired(string _, string __, bool asServer)
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
    }
}