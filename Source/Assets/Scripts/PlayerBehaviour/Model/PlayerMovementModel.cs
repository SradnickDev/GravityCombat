using System;
using System.Collections.Generic;
using PlayerBehaviour.Utilities;
using UI.Options;
using UnityEngine;

namespace PlayerBehaviour.Model
{
	public class PlayerMovementModel : PlayerBaseModel
	{
		#region Events

		public Action OnCollisionHit;

		#endregion

		[Header("Movement")] public Camera Camera;

		[SerializeField] private float MovementSpeed = 10;
		[SerializeField] private float RotationSpeed = 200;

		[Header("Collision")] public LayerMask GroundLayer;

		[SerializeField] private float RotationDegreesOnHit = 10;
		[SerializeField] private float GroundDistance = 1.5f;

		[Header("Collision Force")] [SerializeField]
		private ForceMode CollisionForceMode = ForceMode.VelocityChange;

		[SerializeField] private float CollisionForce = 5;

		#region RaySetup

		[SerializeField] private RayHelper.RayProperties RayDownRightProperties =
			new RayHelper.RayProperties(new Vector2(0.7f, -0.5f), 0, 3, true,
										RayHelper.RayProperties.RayDirection.Down);

		[SerializeField] private RayHelper.RayProperties RayDownLeftProperties =
			new RayHelper.RayProperties(new Vector2(-0.7f, -0.5f), 0, 3, true,
										RayHelper.RayProperties.RayDirection.Down);

		[SerializeField] private RayHelper.RayProperties RayLeftProperties =
			new RayHelper.RayProperties(new Vector2(-0.5f, 0), 45, 3, true, RayHelper.RayProperties.RayDirection.Left);

		[SerializeField] private RayHelper.RayProperties RayRightProperties =
			new RayHelper.RayProperties(new Vector2(0.5f, 0), -45, 3, true, RayHelper.RayProperties.RayDirection.Right);

		[Header("Edge Detection")] [SerializeField, Range(0, 1f)]
		private float RayDirectionWeight = 0.5f;

		[SerializeField] private float RayEdgeLength = 5.0f;

		#endregion

		[Header("IK Shoulder")] [SerializeField]
		private Transform PlayerModel = null;

		[SerializeField] private Transform RightShoulderBone = null;
		[SerializeField] private Transform RightShoulderIK = null;

		public bool IsGrounded { get; private set; }
		public Vector3 AimPoint { get; private set; }
		public bool MovingForward { get; private set; }
		public Vector3 MoveDirection { get; private set; }

		public bool IsMoving
		{
			get
			{
				if (m_wasdMovement)
				{
					return MoveDirection.magnitude != 0;
				}
				else
				{
					return MoveDirection.x != 0;
				}
			}
		}

		private Vector3 m_aimDampingVelocity = new Vector3(0, 0, 0);
		private Vector3 m_groundNormal = new Vector3(0, 0, 0);
		private GameObject m_ikHelper = null;
		private Vector3 m_previousPosition = new Vector3();
		private float m_rotationVelocity = 0.0f;
		private bool m_wasdMovement = true;

		public override void Start()
		{
			base.Start();
			SetupRigidbody();
			SetupIK();

			if (PlayerModel == null)
			{
				Debug.LogWarning("Player Movement Model needs a Model to proceed, please assign one !");
			}
		}

		/// <summary>GameObject to simplify IK positioning/rotation.</summary>
		private void SetupIK()
		{
			m_ikHelper = new GameObject {name = transform.root.name + "IK Helper"};
			m_ikHelper.transform.SetParent(transform);
		}

		/// <summary>Set Constrains, DetectionMode.</summary>
		private void SetupRigidbody()
		{
			Rigidbody.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX |
									RigidbodyConstraints.FreezeRotationY;
			Rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
		}

		private void Update()
		{
			//If this Player is not mine/Local dont do anything
			if (!PhotonView.IsMine) return;

			m_wasdMovement = Options.GetBool(GameSettings.MovementPref, false);

			CheckFacingDirection();

			if (Boots.IsActive)
			{
				GroundCheck();
				Rigidbody.angularDrag = 200;
			}
			else
			{
				Rigidbody.angularDrag = 0;
			}

			if (IsGrounded && Boots.IsActive)
			{
				EdgeCheck();
			}
		}

		private void FixedUpdate()
		{
			//If this Player is not mine/Local dont do anything
			if (!PhotonView.IsMine) return;

			//Moving
			if (Boots.IsActive && IsGrounded)
			{
				Movement(MoveDirection);
			}

			//Floating -> rotating
			if (!Boots.IsActive)
			{
				Rotation(MoveDirection.x);

				//stopping physics rotation while input is given
				if (!Mathf.Approximately(MoveDirection.x, 0))
				{
					Rigidbody.angularVelocity = new Vector3(0, 0, 0);
				}
			}

			UpdateRotation();
		}

		/// <summary>
		/// Comparing movement direction with model orientation, to know if Player is moving forward or Backward. 
		/// </summary>
		private void CheckFacingDirection()
		{
			var fwd = PlayerModel.right;
			var moveDir = (transform.position - m_previousPosition).normalized;

			const float threshold = 0.025f;
			var forward = Vector3.Dot(fwd, moveDir) > threshold;
			var backward = Vector3.Dot(fwd, moveDir) < -threshold;

			if (forward)
			{
				MovingForward = true;
			}

			if (backward)
			{
				MovingForward = false;
			}

			m_previousPosition = Rigidbody.position;
		}

		#region Input

		/// <summary>Creates a Direction.</summary>
		/// <param name="horizontal">Axis input</param>
		/// /// <param name="vertical">Axis input</param>
		internal void PlayerInput(float horizontal, float vertical)
		{
			MoveDirection = new Vector3(horizontal, vertical, 0);
		}

		/// <summary>
		/// Creates direction, based on input direction and ground normal, to move the Rigidbody.
		/// </summary>
		/// <param name="input">Axis Input</param>
		private void Movement(Vector3 input)
		{
			var dir = m_wasdMovement ? Camera.transform.TransformDirection(input) : transform.TransformDirection(input);

			//transform direction accordingly to the ground normal
			var projectOnPlane = Vector3.ProjectOnPlane(dir, m_groundNormal);

			projectOnPlane = Vector3.ClampMagnitude(projectOnPlane, 1);
			Rigidbody.MovePosition(Rigidbody.position + Time.fixedDeltaTime * MovementSpeed * projectOnPlane);
		}

		/// <summary>
		/// Rotates the Rigidbody
		/// Rotation through Player input.
		/// </summary>
		/// <param name="input">Axis input</param>
		private void Rotation(float input)
		{
			var rotationDelta = RotationSpeed * Time.fixedDeltaTime * new Vector3(0, 0, input);
			Rigidbody.MoveRotation(Rigidbody.rotation * Quaternion.Euler(rotationDelta));
		}

		#endregion

		#region Rotation

		/// <summary>Update Rigidbody Rotation</summary>
		private void UpdateRotation()
		{
			var hits = new List<RaycastHit>();

			//Rotates player accordingly to the ground hits
			if (UpdateHits(ref hits))
			{
				Rotate(hits);
			}
		}

		/// <summary>
		/// Using Raycasts to gather information about surrounding environment.
		/// </summary>
		private bool UpdateHits(ref List<RaycastHit> hits)
		{
			//Check for collision under the player
			if (RayHelper.Raycast(transform, RayDownLeftProperties, out var rightHitInfo, GroundLayer) &&
				RayHelper.Raycast(transform, RayDownRightProperties, out var leftHitInfo, GroundLayer))
			{
				hits.Add(leftHitInfo);
				hits.Add(rightHitInfo);


				//Raycast to right only active if player is moving to the right
				if (MoveDirection.x > 0)
				{
					if (RayHelper.Raycast(transform, RayRightProperties, out var rightHit, GroundLayer))
					{
						hits.Add(rightHit);
					}
				}

				//Raycast to left only active if player is moving to the left
				if (MoveDirection.x < 0)
				{
					if (RayHelper.Raycast(transform, RayLeftProperties, out var leftHit, GroundLayer))
					{
						hits.Add(leftHit);
					}
				}

				return true;
			}

			return false;
		}

		/// <summary>
		/// Rotate the Player according to the Average normal.
		/// </summary>
		/// <param name="hitInfos">Ground Hits</param>
		private void Rotate(List<RaycastHit> hitInfos)
		{
			var averageNormal = new Vector3(0, 0, 0);
			foreach (var hit in hitInfos)
			{
				averageNormal += hit.normal;
			}

			averageNormal /= hitInfos.Count;
			m_groundNormal = averageNormal;

			var targetRotation = Quaternion.FromToRotation(Vector3.up, averageNormal);
			var finalRotation = Quaternion.RotateTowards(Rigidbody.rotation, targetRotation,
														RotationDegreesOnHit * Time.deltaTime);

			Rigidbody.MoveRotation(Quaternion.Euler(0, 0, finalRotation.eulerAngles.z));
		}

		/// <summary>Determine model(mesh) rotation based on Model rotation and mouse position.s</summary>
		public void Facing()
		{
			//Direction to Aim Position / Mouse
			var directionToLook = (transform.position - AimPoint).normalized;

			const float threshold = 0.05f;
			var left = Vector3.Dot(directionToLook, transform.right) > threshold;
			var right = Vector3.Dot(directionToLook, transform.right) < -threshold;

			var targetAngle = 0;

			if (left)
			{
				targetAngle = 180;
			}

			if (right)
			{
				targetAngle = 0;
			}


			var y = Mathf.SmoothDampAngle(PlayerModel.localEulerAngles.y, targetAngle, ref m_rotationVelocity, 0.05f);

			PlayerModel.transform.localRotation = Quaternion.Euler(PlayerModel.transform.localRotation.x, y,
																	PlayerModel.transform.localRotation.z);
		}

		#endregion

		#region Collision

		/// <summary>Determine if is Grounded or not.</summary>
		private void GroundCheck()
		{
			Debug.DrawRay(transform.position, -transform.up * GroundDistance, Color.red);
			IsGrounded = Physics.Raycast(transform.position, -transform.up, GroundDistance, GroundLayer);
		}

		/// <summary>Using Raycast to check if Player is standing on an edge or moving to an Edge</summary>
		private void EdgeCheck()
		{
			var ray = new Ray(transform.position + (transform.right * (MoveDirection.x * RayDirectionWeight)),
							-transform.up);
			Debug.DrawRay(ray.origin, ray.direction * RayEdgeLength, Color.magenta);

			// if raycast not hit means player is on edge, set new position 
			if (!Physics.Raycast(ray, RayEdgeLength, GroundLayer, QueryTriggerInteraction.Ignore))
			{
				Rigidbody.position = transform.position + (transform.right * (MoveDirection.x * RayDirectionWeight));
			}
		}

		/// <summary>
		/// Adding force to the Player based on Collision point.
		/// </summary>
		/// <param name="other">Collision info</param>
		private void OnCollisionEnter(Collision other)
		{
			if (Boots == null) return;

			if (Boots.IsActive || other.gameObject.layer == 11) return;

			var contact = other.GetContact(0);
			var inDir = (contact.point - transform.position).normalized;
			var normal = contact.normal;
			var outDir = Vector3.Reflect(inDir, normal);

			OnCollisionHit?.Invoke();

			Rigidbody.AddForce(outDir * CollisionForce, CollisionForceMode);
			Rigidbody.AddTorque(new Vector3(0, 0, outDir.z) * CollisionForce, CollisionForceMode);
		}

		#endregion

		#region IK

		/// <summary>
		/// Inverse Kinematic, Shoulder Rotation.
		/// </summary>
		public void HandleShoulderRotation()
		{
			RightShoulderIK.LookAt(AimPoint, transform.up);

			var rightShoulderPos = RightShoulderBone.TransformPoint(Vector3.zero);
			m_ikHelper.transform.position = rightShoulderPos;
			m_ikHelper.transform.parent = transform;

			RightShoulderIK.position = m_ikHelper.transform.position;
		}

		/// <summary>
		/// Convert a Aim Position according to the Mouse position, little delayed.
		/// </summary>
		public void Aim()
		{
			var mousePos = Input.mousePosition;
			var ray = Camera.ScreenPointToRay(mousePos);
			var diff = -ray.origin.z / ray.direction.z;
			var target = ray.origin + ray.direction * diff;
			AimPoint = Vector3.SmoothDamp(AimPoint, target, ref m_aimDampingVelocity, 0.05f);
		}

		#endregion
	}
}