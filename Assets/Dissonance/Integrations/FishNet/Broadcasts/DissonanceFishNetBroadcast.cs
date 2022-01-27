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
            unsafe
            {
                byte[] rentedArray = ByteArrayPool.Retrieve(DissonanceFishNetConstants.MaxPacketSize);
              
                // Copy original data content to rented byte array
                fixed(byte* rentedPtr = rentedArray)
                fixed (byte* originalPtr = &originalData.Array[originalData.Offset])
                {
                    UnsafeUtility.MemCpy(rentedPtr, originalPtr, originalData.Count);
                }
            }
        }

        // Returns buffer content
        public void RelaseBuffer()
        {
            // If array is not null, then return it to pool
            if(Payload.Array != null)
                ByteArrayPool.Store(Payload.Array);
        }
    }
}