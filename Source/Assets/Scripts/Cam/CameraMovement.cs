using PlayerBehaviour.Utilities;
using UnityEngine;

namespace Cam
{
	public class CameraMovement : MonoBehaviour
	{
		public Transform Target { get; set; }

		[SerializeField] private Vector3 TargetOffset = new Vector3(0, 0, -25);
		[SerializeField] private float MaxDistanceFromTarget = 10.0f;
		[SerializeField, Range(0.0f, 1.0f)] private float SmoothTime = 0.5f;
		[SerializeField] private UnityEngine.Camera Camera = null;

		private Vector3 m_startPosition = new Vector3(0, 0, 0);
		private Vector3 m_targetPosition = new Vector3(0, 0, 0);
		private Vector3 m_referenceVelocity = new Vector3(0, 0, 0);

		private void Start()
		{
			Camera = GetComponentInChildren<UnityEngine.Camera>();
			m_startPosition = transform.position;
		}

		private void FixedUpdate()
		{
			FollowTarget();
		}

		/// <summary>
		/// Calculates a position between a given target + offset and a specific distance based on direction between mouse and target
		/// </summary>
		private void FollowTarget()
		{
			if (Target == null || Camera == null)
			{
				m_targetPosition = m_startPosition;
			}
			else
			{
				var mousePosition = Helper.GetMouseInWorld(Camera);

				var pos = (Target.position + TargetOffset);
				var heading = (mousePosition - Target.position).normalized;
				heading.z = 0;

				var dir = heading;

				m_targetPosition = pos + (dir * MaxDistanceFromTarget);
			}

			transform.position =
				Vector3.SmoothDamp(transform.position, m_targetPosition, ref m_referenceVelocity, SmoothTime);
		}
	}
}