using System;
using System.Collections;
using Photon.Pun;
using PlayerBehaviour.General;
using PlayerBehaviour.Interface;
using UI;
using UnityEngine;

namespace PlayerBehaviour.Weapon.Model
{
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(Collider))]
	[RequireComponent(typeof(PhotonView))]
	public class ProjectileModel : MonoBehaviour
	{
		#region Events

		public Action OnCollision;
		public Action<float> OnApplyDamage;

		#endregion

		[SerializeField] private HitEvent HitEvent = null;
		[SerializeField] private float LifeTime = 3.0f;
		[SerializeField] private float MoveSpeed = 40;

		private bool m_hitted = false;
		private PhotonView m_ownerView = null;
		private float m_damage = 0;
		private bool m_isMoving = false;
		private Rigidbody m_rigidbody = null;
		private Vector3 m_direction = new Vector3(0, 0, 0);
		private Collider m_collider = null;
		private float m_currentLifeTime = 0.0f;
		private GameObject m_ownerObject = null;
		private Photon.Realtime.Player m_ownerPlayer = null;

		private void Awake()
		{
			Setup();
		}

		private void Setup()
		{
			m_currentLifeTime = LifeTime;
			m_collider = GetComponent<Collider>();
			m_rigidbody = GetComponent<Rigidbody>();
			OnCollision += HitEffects;
		}

		public void Init(Vector3 direction, Photon.Realtime.Player player, GameObject owner, float damage)
		{
			m_ownerObject = owner;
			m_ownerPlayer = player;
			m_damage = damage;
			m_direction = direction;
			m_ownerView = owner.GetComponent<PhotonView>();
			transform.LookAt(transform.position + direction * 100);
			m_isMoving = true;
		}

		private void FixedUpdate()
		{
			if (!m_isMoving) return;

			m_currentLifeTime -= Time.deltaTime;
			if (m_currentLifeTime <= 0)
			{
				m_isMoving = false;
				Collision();
			}

			m_rigidbody.MovePosition(m_rigidbody.position + MoveSpeed * Time.deltaTime * m_direction);
		}

		public void OnCollisionEnter(Collision other)
		{
			if (m_ownerObject == other.gameObject || m_hitted) return;

			if (other.gameObject.ValidateTarget() && m_ownerView.IsMine)
			{
				var hitted = other.gameObject.GetComponent<IDamageable>().ApplyDamage(m_damage);
				if (hitted)
				{
					HitEvent.Invoke();
					OnApplyDamage?.Invoke(m_damage);
				}
			}

			m_isMoving = false;
			Collision();
			m_hitted = true;
		}

		private void StopProjectile()
		{
			m_collider.enabled = false;
		}

		private void HitEffects()
		{
			StopProjectile();

			StartCoroutine(DestroyWithDelay());
		}

		private IEnumerator DestroyWithDelay()
		{
			yield return new WaitForSeconds(0.25f);
			Destroy(gameObject);
		}

		private void Collision()
		{
			OnCollision?.Invoke();
		}
	}
}