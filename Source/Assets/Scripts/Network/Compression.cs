using System;
using UnityEngine;

namespace Network
{
	public static class Compression
	{
		public enum Axis
		{
			X,Y,Z,XY,YZ,XZ
		}
		
		private const float MaxAbsValue = 500f;
		private const float MaxPrecisionValue = 10f;
		
		public static int PackVector(Vector3 vector)
		{
			return PackFloats(vector.x, vector.y);
		}

		public static Vector3 UnpackVector(int vector)
		{
			var (num1, num2) = UnpackFloats(vector);
			return new Vector3(num1, num2, 0);
		}

		public static float Quaternion(Quaternion quaternion,Axis axis)
		{
			switch (axis)
			{
				case Axis.X:
					return quaternion.eulerAngles.x;
				case Axis.Y:
					return quaternion.eulerAngles.y;
				case Axis.Z:
					return quaternion.eulerAngles.z;
				case Axis.XY:
					return PackFloats(quaternion.eulerAngles.x, quaternion.eulerAngles.y);
				case Axis.YZ:
					return PackFloats(quaternion.eulerAngles.y, quaternion.eulerAngles.z);
				case Axis.XZ:
					return PackFloats(quaternion.eulerAngles.x, quaternion.eulerAngles.z);
				default:
					return 0;
			}
		}

		public static Quaternion DecompressQuaternion(float angle)
		{
			return UnityEngine.Quaternion.Euler(0f, 0f, angle);
		}

		private static int PackFloats(float num1, float num2)
		{
			var lhs = Mathf.RoundToInt((num1 + MaxAbsValue) * MaxPrecisionValue);
			var rhs = Mathf.RoundToInt((num2 + MaxAbsValue) * MaxPrecisionValue);
			return lhs << 16 | rhs;
		}

		private static Tuple<float, float> UnpackFloats(int packedFloats)
		{
			var lhs = packedFloats >> 16 & 65535;
			var rhs = packedFloats & 65535;
			var x = lhs / MaxPrecisionValue - MaxAbsValue;
			var y = rhs / MaxPrecisionValue - MaxAbsValue;
			return new Tuple<float, float>(x, y);
		}

		public static int PackQuaternion(Quaternion quaternion)
		{
			var lhs = Mathf.RoundToInt((quaternion.x + 255) * MaxPrecisionValue);
			var lhm = Mathf.RoundToInt((quaternion.y + 255) * MaxPrecisionValue);
			var rhs = Mathf.RoundToInt((quaternion.z + 255) * MaxPrecisionValue);
			var rhm = Mathf.RoundToInt((quaternion.w + 255) * MaxPrecisionValue);
			return lhs << 8 | lhm << 8 | rhm << 8 | rhs;
		}

		public static Quaternion UnpackQuaternion(int packedQuat)
		{
			var lhs = packedQuat & 255;
			var lhm = packedQuat >> 8 & 255;
			var rhm = packedQuat >> 16 & 255;
			var rhs = packedQuat >> 24 & 255;
			var x = lhs / MaxPrecisionValue - MaxAbsValue;
			var y = lhm / MaxPrecisionValue - MaxAbsValue;
			var z = rhm / MaxPrecisionValue - MaxAbsValue;
			var w = rhs / MaxPrecisionValue - MaxAbsValue;
			return new Quaternion(x,y,z,w);
		}
	}
}