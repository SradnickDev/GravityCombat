using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PlayerBehaviour.Model
{
	/// <summary>
	/// Handel Boots Mechanics.
	/// </summary>
	public class PlayerBootsModel : PlayerBaseModel
	{
		#region Events

		public Action BootsGrounded;
		public Action Activated;
		public Action Deactivated;
		public Action InUse;

		#endregion

		[Header("Boots")] [SerializeField] private float AttractForce = 1.5f;

		[Header("Jump")] [SerializeField] private float InitialJumpForce = 10;
		[SerializeField] private float InitialJumpRotationForce = 10;
		[SerializeField] private ForceMode JumpForceMode = ForceMode.VelocityChange;

		private bool m_previousGrounded = false;

		public bool IsActive { get; private set; } = true;

		private void Update()
		{
			Grounded();
		}

		private void FixedUpdate()
		{
			if (IsActive)
			{
				AttractToGround();
			}
		}

		/// <summary>
		/// To Activate ot Deactivate the Boots.
		/// </summary>
		internal void Use()
		{
			IsActive = !IsActive;
			if (!IsActive && MovementModel.IsGrounded)
			{
				InitialJump(MovementModel.MoveDirection);
			}

			if (IsActive)
			{
				Activated?.Invoke();
			}
			else
			{
				Deactivated?.Invoke();
			}
		}

		/// <summary>
		/// Add Force in given Direction.
		/// </summary>
		/// <param name="initialDirection"></param>
		private void InitialJump(Vector3 initialDirection)
		{
			var direction = (transform.up + initialDirection).normalized;
			Rigidbody.AddForce(direction * InitialJumpForce, JumpForceMode);

			if (initialDirection == Vector3.zero)
			{
				initialDirection.z = Random.value;
			}

			Rigidbody.AddTorque(initialDirection * InitialJumpRotationForce, JumpForceMode);
		}

		/// <summary>
		/// Using directly Velocity to move the Player in specific direction.
		/// </summary>
		private void AttractToGround()
		{
			var direction = -transform.up;
			Rigidbody.velocity = direction * AttractForce;
		}

		/// <summary>
		/// Check if the Player reached the ground.
		/// </summary>
		private void Grounded()
		{
			if (MovementModel.IsGrounded && !m_previousGrounded)
			{
				BootsGrounded?.Invoke();
			}

			if (!MovementModel.IsGrounded && IsActive)
			{
				InUse?.Invoke();
			}

			m_previousGrounded = MovementModel.IsGrounded;
		}
	}
}