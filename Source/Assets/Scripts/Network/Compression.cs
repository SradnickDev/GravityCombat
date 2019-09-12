using UnityEngine;

namespace Network
{
	public static class Compression
	{
		private const float MaxAbsValue = 500f;
		private const float MaxPrecisionValue = 10f;
	
		/// <summary>
		/// Bit shifts two floats (X,Y, z ignored) into 1 int.
		/// </summary>
		/// <param name="vector"></param>
		/// <returns></returns>
		public static int PackVector(Vector3 vector)
		{
			var lhs = Mathf.RoundToInt((vector.x + MaxAbsValue) * MaxPrecisionValue);
			var rhs = Mathf.RoundToInt((vector.y + MaxAbsValue) * MaxPrecisionValue);
			return lhs << 16 | rhs;
		}

		/// <summary>
		/// Unpack two floats from 1 int.
		/// </summary>
		/// <param name="vector"></param>
		/// <returns></returns>
		public static Vector3 UnpackVector(int vector)
		{
			var lhs = vector >> 16 & 65535;
			var rhs = vector & 65535;
			var x = lhs / MaxPrecisionValue - MaxAbsValue;
			var y = rhs / MaxPrecisionValue - MaxAbsValue;
			return new Vector3(x, y,0);
		}

		public static float Quaternion(Quaternion quaternion)
		{
			return quaternion.eulerAngles.z;
		}

		public static Quaternion DecompressQuaternion(float angle)
		{
			return UnityEngine.Quaternion.Euler(0f, 0f, angle);
		}
	}
}