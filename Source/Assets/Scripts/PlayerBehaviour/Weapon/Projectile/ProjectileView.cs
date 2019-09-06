using PlayerBehaviour.Weapon.Model;
using SCT;
using UnityEngine;

namespace PlayerBehaviour.Weapon.Projectile
{
	/// <summary>
	/// Using Events from ProjectileModel to show Player Effects.
	/// </summary>
	[RequireComponent(typeof(AudioSource))]
	[RequireComponent(typeof(ProjectileModel))]
	public class ProjectileView : MonoBehaviour
	{
		[SerializeField] private ParticleSystem HitEffect = null;
		[SerializeField] private GameObject TrailEffect = null;

		private AudioSource m_hitSound = null;
		private ProjectileModel m_projectileModel;

		private void Awake()
		{
			Setup();
		}

		private void Setup()
		{
			m_hitSound = GetComponent<AudioSource>();
			m_projectileModel = GetComponent<ProjectileModel>();
			m_projectileModel.OnCollision += HitEffects;
			m_projectileModel.OnApplyDamage += OnApplyDamage;
		}

		private void OnApplyDamage(float damage)
		{
			ScriptableTextDisplay.Instance.InitializeScriptableText(0, transform.position, damage.ToString());
		}

		private void HitEffects()
		{
			HideProjectile();

			if (HitEffect != null)
			{
				HitEffect.Play(true);
			}

			if (m_hitSound != null && m_hitSound.clip != null)
			{
				m_hitSound.Play();
			}
		}

		private void HideProjectile()
		{
			TrailEffect.SetActive(false);
		}

		private void OnDisable()
		{
			m_projectileModel.OnCollision -= HitEffects;
			m_projectileModel.OnApplyDamage -= OnApplyDamage;
		}
	}
}