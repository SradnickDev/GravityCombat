using UnityEngine;

namespace PlayerBehaviour.Utilities
{
	public static class RayHelper
	{
		[System.Serializable]
		public class RayProperties
		{
			public Vector2 Offset;
			public float Angle;
			public float Length;
			public bool ShowDebugLine;

			public enum RayDirection
			{
				Left,
				Right,
				Up,
				Down
			}

			[SerializeField] private RayDirection Direction;

			public Vector3 GetDirection(Transform transform)
			{
				switch (Direction)
				{
					case RayDirection.Left:
						return -transform.right;
					case RayDirection.Right:
						return transform.right;
					case RayDirection.Up:
						return transform.up;
					case RayDirection.Down:
						return -transform.up;
					default:
						return Vector3.zero;
				}
			}

			public RayProperties(Vector3 offset, float angle, float length, bool debug, RayDirection direction)
			{
				Offset = offset;
				Angle = angle;
				Length = length;
				ShowDebugLine = debug;
				Direction = direction;
			}
		}

		/// <summary>
		/// Creating a Ray with given Properties.
		/// </summary>
		/// <param name="transform">For position and Direction</param>
		/// <param name="rayProps">Ray Properties</param>
		/// <param name="hitInfo">RaycastHit</param>
		/// <param name="layerMask">Layer</param>
		/// <returns>True if it something.</returns>
		public static bool Raycast(Transform transform, RayProperties rayProps, out RaycastHit hitInfo,
			LayerMask layerMask)
		{
			var rot = Quaternion.Euler(0, 0, rayProps.Angle) * rayProps.GetDirection(transform);
			var origin = transform.position + transform.up * rayProps.Offset.y + transform.right * rayProps.Offset.x;
			var ray = new Ray(origin, rayProps.GetDirection(transform) + rot);

			if (rayProps.ShowDebugLine)
			{
				Debug.DrawRay(ray.origin, ray.direction * rayProps.Length, Color.red);
			}

			return Physics.Raycast(ray, out hitInfo, rayProps.Length, layerMask);
		}
	}
}