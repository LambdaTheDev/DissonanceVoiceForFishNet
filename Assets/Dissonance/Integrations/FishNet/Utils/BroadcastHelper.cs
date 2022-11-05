using System;
using Dissonance.Integrations.FishNet.Broadcasts;
using FishNet.Utility.Performance;
using Unity.Collections.LowLevel.Unsafe;

namespace Dissonance.Integrations.FishNet.Utils
{
    // Internal class that helps with DissonanceFishNetBroadcast type
    internal static class BroadcastHelper
    {
        // Copies original data into a new buffer & creates a broadcast instance
        public static unsafe DissonanceFishNetBroadcast CreateFromOriginalData(ArraySegment<byte> data)
        {
            // Get buffer & fill it with data
            int originalDataCount = data.Count;
            byte[] rentedBuffer = ByteArrayPool.Retrieve(originalDataCount);
            fixed (byte* rentedPtr = rentedBuffer)
            {
                fixed (byte* originalPtr = data.Array)
                {
                    UnsafeUtility.MemCpy(rentedPtr, originalPtr + data.Offset, originalDataCount);
                }
            }
            
            // Create broadcast
            ArraySegment<byte> copiedData = new ArraySegment<byte>(rentedBuffer, 0, originalDataCount);
            return new DissonanceFishNetBroadcast(copiedData, true);
        }
    }
}