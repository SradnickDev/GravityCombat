using Photon.Pun;
using PlayerBehaviour.Model;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerBehaviour.Weapon
{
	/// <summary>
	/// Handles Ammo Bar.
	/// </summary>
	[RequireComponent(typeof(PhotonView))]
	[RequireComponent(typeof(Image))]
	[RequireComponent(typeof(PlayerHealthModel))]
	public class WeaponHud : MonoBehaviour
	{
		[Header("References")] [SerializeField]
		private PhotonView PhotonView = null;

		[SerializeField] private Image FillBar = null;
		[SerializeField] private Text AmountView = null;
		[SerializeField] private Transform AmmoBar = null;

		private bool m_reload = false;
		private float m_timer = 0.0f;
		private float m_reloadTime = 0.0f;

		private void Start()
		{
			if (!PhotonView.IsMine)
			{
				AmmoBar.gameObject.SetActive(false);
				AmountView.enabled = false;
			}
		}

		/// <summary>Instant display current Ammo Status.</summary>
		/// <param name="currentAmount">in clip</param>
		/// <param name="maxAmount">max clip</param>
		public void SetAmmoAmount(float currentAmount, float maxAmount)
		{
			FillBar.fillAmount = GetAmount(currentAmount, maxAmount);

			SetAmmoText($"{currentAmount} / {maxAmount}");
		}

		private void SetAmmoText(string text)
		{
			if (AmountView != null)
			{
				AmountView.text = text;
			}
		}

		/// <summary>Start Reload Animation.</summary>
		/// <param name="reloadTime">Duration.</param>
		public void Reload(float reloadTime)
		{
			m_reloadTime = reloadTime;
			m_timer = 0;
			m_reload = true;
		}

		private void Update()
		{
			if (!m_reload) return;

			ReloadBarAnimation();
		}

		/// <summary> Increase Reload Bar 'till Reload is done.</summary>
		private void ReloadBarAnimation()
		{
			if (m_timer >= m_reloadTime)
			{
				m_reload = false;
			}

			m_timer = Mathf.Clamp(m_timer += Time.deltaTime, 0, m_reloadTime);

			FillBar.fillAmount = GetAmount(m_timer, m_reloadTime);

			var timeLeft = m_reloadTime - m_timer;
			SetAmmoText(timeLeft.ToString("F1"));
		}

		private float GetAmount(float current, float max)
		{
			return current / max;
		}

		public void Reset()
		{
			m_reload = false;
			m_timer = 0;
			FillBar.fillAmount = 1;
		}
	}
}