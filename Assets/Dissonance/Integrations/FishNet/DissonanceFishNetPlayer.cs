using System.Runtime.CompilerServices;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

namespace Dissonance.Integrations.FishNet
{
    // A Player object wrapper for Dissonance Voice
    public sealed class DissonanceFishNetPlayer : NetworkBehaviour, IDissonancePlayer
    {
        // SyncVar ensures that all observers know player ID, even late joiners
        [SyncVar] private string _syncedPlayerId;

        public string PlayerId
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _syncedPlayerId;
        }

        public Vector3 Position
        {
            // IMPORTANT NOTE: When Punfish finishes child NetworkObjects, I will have 2 options:
            // A) Make this transform.root.position - less performance
            // B) Make a check if it's a root object & not allow new FishNet's functionality
            //
            // But we will see in future, now this should work.
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => transform.position;
        }

        public Quaternion Rotation
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => transform.rotation;
        }

        public NetworkPlayerType Type => IsOwner ? NetworkPlayerType.Local : NetworkPlayerType.Remote;

        public bool IsTracking { get; private set; }
    }
}