using System;
using Dissonance.Integrations.FishNet.Constants;
using FishNet.Broadcast;
using FishNet.Utility.Performance;
using Unity.Collections.LowLevel.Unsafe;

namespace Dissonance.Integrations.FishNet.Broadcasts
{
    // Broadcast struct for Dissonance Voice data
    public readonly struct DissonanceFishNetBroadcast : IBroadcast
    {
        public readonly ArraySegment<byte> Payload;


        public DissonanceFishNetBroadcast(ArraySegment<byte> originalData)
        {
            byte[] rentedArray = ByteArrayPool.Retrieve(originalData.Count);

#if DISSONANCE_FOR_FISHNET_UNSAFE
            unsafe
            {
                // Copy original data content to rented byte array
                fixed(byte* rentedPtr = rentedArray)
                fixed (byte* originalPtr = &originalData.Array[originalData.Offset])
                {
                    UnsafeUtility.MemCpy(rentedPtr, originalPtr, originalData.Count);
                }
            }
            
            return;
#endif
            
            Buffer.BlockCopy(originalData.Array, originalData.Offset, rentedArray, 0, originalData.Count);
        }

        // Returns buffer content
        public void ReleaseBuffer()
        {
            // If array is not null, then return it to pool
            if(Payload.Array != null)
                ByteArrayPool.Store(Payload.Array);
        }
    }
}