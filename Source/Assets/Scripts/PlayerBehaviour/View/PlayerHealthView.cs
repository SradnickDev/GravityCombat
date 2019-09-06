using Network.Extensions;
using Photon.Pun;
using PlayerBehaviour.General;
using PlayerBehaviour.Model;
using SCT;
using UI.KillFeed;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerBehaviour.View
{
	[RequireComponent(typeof(PlayerHealthModel))]
	public class PlayerHealthView : MonoBehaviour
	{
		[Header("Positioning")] [SerializeField]
		private Vector2 PositionOffset = new Vector2(0, 0);

		[Header("References")] [SerializeField]
		private PlayerHealthModel PlayerHealthModel = null;

		[SerializeField] private ParticleSystem RegenerationEffect = null;
		[SerializeField] private PhotonView PhotonView = null;
		[SerializeField] private Image FillBar = null;
		[SerializeField] private Transform HealthBar = null;
		[SerializeField] private Text AmountView = null;
		[SerializeField] private MaterialFlicker Flicker = null;

		private Photon.Realtime.Player m_owner = null;
		private KillFeed m_killFeed = null;

		private void OnEnable()
		{
			PlayerHealthModel.OnChangeHealthEvent += HealthChangedEvent;
			PlayerHealthModel.OnChangeHealthEvent += SetHealthText;
			PlayerHealthModel.OnReceivedDamage += ReceivedDamage;
			PlayerHealthModel.OnPlayerDeath += Death;
			PlayerHealthModel.OnLocalHit += LocalHit;
		}

		private void Start()
		{
			m_owner = PhotonView.Owner;
			PlayerHealthModel.CurrentTeam = m_owner.GetTeam();

			if (!PhotonView.IsMine)
			{
				IsFriendly();
			}

			transform.SetParent(null);
			m_killFeed = KillFeed.Instance;
		}

		/// <summary>Changed Color based on Team.</summary>
		private void IsFriendly()
		{
			if (m_owner.GetTeam() == Team.Aggressive ||
				m_owner.GetTeam() != PhotonNetwork.LocalPlayer.GetTeam() || m_owner.GetTeam() == Team.None)
			{
				FillBar.color = Color.red;
			}
			else
			{
				FillBar.color = Color.yellow;
			}
		}

		private void Update()
		{
			if (PhotonView != null && HealthBar != null && PlayerHealthModel.Camera != null)
			{
				HealthBar.position =
					PlayerHealthModel.Camera.WorldToScreenPoint(PlayerHealthModel.transform.position +
																new Vector3(PositionOffset.x, PositionOffset.y, 0));
			}

			if (PlayerHealthModel == null)
			{
				Destroy(gameObject);
			}

			Regenerate(PlayerHealthModel.IsRegenerating);
		}

		private void ReceivedDamage(float damage)
		{
			ScriptableTextDisplay.Instance.InitializeStackingScriptableText(2, PlayerHealthModel.transform.position,
																			"-" + damage, "damaged");
			if (Flicker != null)
			{
				Flicker.Play();
			}
		}

		private void LocalHit()
		{
			if (Flicker != null)
			{
				Flicker.Play();
			}

			if (RegenerationEffect.isPlaying)
			{
				RegenerationEffect.Stop(true);
			}
		}

		private void HealthChangedEvent(float currentHealth, float maxHealth)
		{
			var fillAmount = currentHealth / maxHealth;
			FillBar.fillAmount = fillAmount;
		}

		private void SetHealthText(float current, float max)
		{
			if (AmountView != null)
			{
				AmountView.text = $"{current:f0} / {max:f0}";
			}
		}

		private void Regenerate(bool reg)
		{
			if (RegenerationEffect == null) return;

			if (reg)
			{
				if (!RegenerationEffect.isPlaying)
				{
					RegenerationEffect.Play(true);
				}
			}
			else
			{
				if (RegenerationEffect.isPlaying)
				{
					RegenerationEffect.Stop(true);
				}
			}
		}

		private void Death(Photon.Realtime.Player lastHit)
		{
			ScriptableTextDisplay.Instance.InitializeScriptableText(3, PlayerHealthModel.transform.position,
																	"Killed by " + lastHit.NickName + "!");
			if (m_killFeed != null)
			{
				m_killFeed.AddFeed(lastHit);
			}

			Destroy(gameObject);
		}

		private void OnDisable()
		{
			PlayerHealthModel.OnChangeHealthEvent -= HealthChangedEvent;
			PlayerHealthModel.OnReceivedDamage -= ReceivedDamage;
			PlayerHealthModel.OnPlayerDeath -= Death;
		}
	}
}