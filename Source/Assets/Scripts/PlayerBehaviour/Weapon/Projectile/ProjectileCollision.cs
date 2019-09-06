using UnityEngine;

//Source from https://wiki.unity3d.com/index.php/DontGoThroughThings
namespace PlayerBehaviour.Weapon.Projectile
{
	public class ProjectileCollision : MonoBehaviour
	{
		[SerializeField] private LayerMask GroundMask = new LayerMask();

		private float m_minimumExtent = 0;
		private float m_partialExtent = 0;
		private float m_sqrMinimumExtent = 0;
		private Vector3 m_previousPosition = new Vector3(0, 0, 0);
		private Rigidbody m_rigidbody = null;
		private Collider m_collider = null;
		private float m_skinWidth = 0.1f;

		private void Start()
		{
			m_rigidbody = GetComponent<Rigidbody>();
			m_collider = GetComponent<Collider>();
			m_previousPosition = m_rigidbody.position;

			var bounds = m_collider.bounds;
			m_minimumExtent = Mathf.Min(Mathf.Min(bounds.extents.x, bounds.extents.y),
										bounds.extents.z);

			m_partialExtent = m_minimumExtent * (1.0f - m_skinWidth);
			m_sqrMinimumExtent = m_minimumExtent * m_minimumExtent;
		}

		private void FixedUpdate()
		{
			CollisionDetection();
		}

		private void CollisionDetection()
		{
			var step = m_rigidbody.position - m_previousPosition;
			var stepMagnitude = step.sqrMagnitude;

			if (stepMagnitude > m_sqrMinimumExtent)
			{
				var movementMagnitude = Mathf.Sqrt(stepMagnitude);

				if (Physics.Raycast(m_previousPosition, step, out var hitInfo, movementMagnitude,
									GroundMask.value))
				{
					if (hitInfo.collider)
					{
						MoveToHitPoint(hitInfo, step, movementMagnitude);
					}
				}
			}

			m_previousPosition = m_rigidbody.position;
		}

		private void MoveToHitPoint(RaycastHit hitInfo, Vector3 movementThisStep, float movementMagnitude)
		{
			m_rigidbody.position = hitInfo.point - (movementThisStep / movementMagnitude) * m_partialExtent;
		}
	}
}