using UnityEngine;

namespace Utilities
{
	public class GizmoExtension
	{
		/// <summary>
		/// Draws a Capsule, using a Transforms Orientation.
		/// </summary>
		/// <param name="origin">Transform origins</param>
		/// <param name="radius">Max Radius the Capsule can have.Clamped min = 0, max = maxDistance/2 .</param>
		/// <param name="maxDistance">Max Distance in forward direction,</param>
		public static void DrawCapsule(Transform origin, float radius, float maxDistance)
		{
			var forward = origin.forward;
			var position = origin.position;
			var up = origin.up;
			var right = origin.right;
			radius = Mathf.Clamp(radius, 0, maxDistance / 2);

			var startCenter = position + forward * radius;
			var endCenter = position + forward * (maxDistance - radius);

			//Start ,End Sphere
			Gizmos.DrawWireSphere(startCenter, radius);
			Gizmos.DrawWireSphere(endCenter, radius);

			//Top, Bottom Line
			Gizmos.DrawLine(startCenter + up * radius, endCenter + up * radius);
			Gizmos.DrawLine(startCenter - up * radius, endCenter - up * radius);

			//Left, Right Line
			Gizmos.DrawLine(startCenter + right * radius, endCenter + right * radius);
			Gizmos.DrawLine(startCenter - right * radius, endCenter - right * radius);
		}
	}
}