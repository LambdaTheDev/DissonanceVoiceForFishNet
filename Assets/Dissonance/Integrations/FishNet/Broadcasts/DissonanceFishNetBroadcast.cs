using System;
using System.Runtime.CompilerServices;
using Dissonance.Integrations.FishNet.Constants;
using FishNet.Broadcast;
using FishNet.Serializing;
using FishNet.Utility.Performance;
using UnityEngine;

namespace Dissonance.Integrations.FishNet.Broadcasts
{
	// Broadcast struct for Dissonance Voice data
	public readonly struct DissonanceFishNetBroadcast : IBroadcast
	{
		public readonly ArraySegment<byte> Payload;
        public readonly bool IsRentedBuffer;


		public DissonanceFishNetBroadcast(ArraySegment<byte> data, bool rentedBuffer)
        {
            Payload = data;
            IsRentedBuffer = rentedBuffer;
        }

		// Returns buffer content
		public void ReleaseBuffer()
		{
			// If array is not null, then return it to pool
			if (Payload.Array != null && IsRentedBuffer)
				ByteArrayPool.Store(Payload.Array);
		}

		// Method used to ensure that packets are within Packet threshold
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
		public static void WriteDissonanceFishNetBroadcast(this Writer writer, DissonanceFishNetBroadcast value)
		{
			writer.WriteArraySegmentAndSize(value.Payload);
		}

		public static DissonanceFishNetBroadcast ReadDissonanceFishNetBroadcast(this Reader reader)
        {
            // todo: When Martin responds, make a final decision.
            // If packets are handled instantly, then Ill just wrap Reader's memory into broadcast & go
            // Otherwise, if packets may be processed later & buffer get overriden, I'll make a data copy
            ArraySegment<byte> readSegment = reader.ReadArraySegmentAndSize();
            return new DissonanceFishNetBroadcast(readSegment, false);
        }
	}
}