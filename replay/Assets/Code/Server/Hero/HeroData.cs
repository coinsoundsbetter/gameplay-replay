using System;
using Unity.Collections;

namespace KillCam.Server {

	public struct HeroFireAckData : IDisposable {
		public NativeHashSet<uint> AckFireIds;
		public uint LastAckFireId;

		public void Dispose() {
			AckFireIds.Dispose();
		}
	}

	public struct HeroWeaponData {
		public uint AllowFireTick;
		public ushort AmmonInMag;
	}
}