using System;
using System.Runtime.CompilerServices;
using FishNet.Connection;

namespace Dissonance.Integrations.FishNet
{
    // A Connection wrapper for Dissonance Voice
    public readonly struct DissonanceFishNetConnection : IEquatable<DissonanceFishNetConnection>
    {
        // Public field to actual NetworkConnection
        public readonly NetworkConnection FishNetConnection;


        // Constructor
        public DissonanceFishNetConnection(NetworkConnection fishNetConn)
        {
            FishNetConnection = fishNetConn;
        }


        #region IEquatable implementation

        /*
         * IMPORTANT NOTES:
         * - FishNet's NetworkConnection also implements IEquitable, so I will just
         *   call Equals(...) methods instead of some IDs comparisons etc
         * - I put here [MethodImpl(AggressiveInlining)], due to it's just a NetworkConnection wrapper 
         */
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(DissonanceFishNetConnection other)
        {
            return FishNetConnection.Equals(other.FishNetConnection);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;

            return obj is DissonanceFishNetConnection other && Equals(other);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
        {
            return FishNetConnection.GetHashCode();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(DissonanceFishNetConnection left, DissonanceFishNetConnection right)
        {
            return left.Equals(right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(DissonanceFishNetConnection left, DissonanceFishNetConnection right)
        {
            return !left.Equals(right);
        }

        #endregion
    }
}