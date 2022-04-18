using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Dissonance.Integrations.FishNet.Constants;
using FishNet.Broadcast;
using FishNet.Serializing;
using FishNet.Utility.Performance;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace Dissonance.Integrations.FishNet.Broadcasts
{
	// Broadcast struct for Dissonance Voice data
	public readonly struct DissonanceFishNetBroadcast : IBroadcast
	{
		public readonly ArraySegment<byte> Payload;


		public DissonanceFishNetBroadcast(ArraySegment<byte> originalData)
		{
			if (!EnsurePacketSize(originalData.Count))
			{
				Payload = default;
				return;
			}

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
			Payload = new ArraySegment<byte>(rentedArray, 0, originalData.Count);
		}

		// Returns buffer content
		public void ReleaseBuffer()
		{
			// If array is not null, then return it to pool
			if (Payload.Array != null)
				ByteArrayPool.Store(Payload.Array);
		}

		// Method used to ensure that packets are within Packet treshold
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool EnsurePacketSize(int dataCount)
		{
			if (dataCount > DissonanceFishNetConstants.PacketSizeThreshold)
			{
				Debug.LogWarning("Attempted to send packet of size: " + dataCount +
								 "; max packet size is: " + DissonanceFishNetConstants.PacketSizeThreshold + "!");
				return false;
			}

			return true;
		}
	}

	public static class DissonanceFishNetBroadcastExtensions
	{
		public static void WriteDissonanceFishNetBroadcast([NotNull] this Writer writer, DissonanceFishNetBroadcast value)
		{
			writer.WriteUInt16((ushort)value.Payload.Count);
			writer.WriteBytes(value.Payload.Array, value.Payload.Offset, value.Payload.Count);
		}

		public static DissonanceFishNetBroadcast ReadDissonanceFishNetBroadcast([NotNull] this Reader reader)
		{
			var length = reader.ReadUInt16();
			var arr = ByteArrayPool.Retrieve(length);

			for (var i = 0; i < length; i++)
				arr[i] = reader.ReadByte();

			var broadcast = new DissonanceFishNetBroadcast(new ArraySegment<byte>(arr, 0, length));

			ByteArrayPool.Store(arr);

			return broadcast;
		}
	}
}