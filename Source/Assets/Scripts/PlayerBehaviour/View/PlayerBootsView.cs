using PlayerBehaviour.Model;
using UnityEngine;

namespace PlayerBehaviour.View
{
	/// <summary>
	/// Controls Boots Visuals(Effects).
	/// </summary>
	public class PlayerBootsView : MonoBehaviour
	{
		[SerializeField] private PlayerBootsModel PlayerBootsModel = null;
		[SerializeField] private ParticleSystem DockEffect = null;
		[SerializeField] private ParticleSystem ActivateEffect = null;
		[SerializeField] private ParticleSystem GuideArrow = null;
		[SerializeField] private Renderer MeshRenderer = null;

		[SerializeField, ColorUsage(true, true)]
		private Color Activated = Color.green;

		[SerializeField, ColorUsage(true, true)]
		private Color Deactivated = Color.red;

		[SerializeField] private string ColorPropertyName = "_Emissiontintcolor";

		private void OnEnable()
		{
			PlayerBootsModel.BootsGrounded += OnBootsGrounded;
			PlayerBootsModel.Activated += OnActivated;
			PlayerBootsModel.Deactivated += OnDeactivated;
			PlayerBootsModel.InUse += InUse;
			MeshRenderer.material.SetColor(ColorPropertyName, Color.white);
		}

		private void InUse()
		{
			if (GuideArrow != null)
			{
				GuideArrow.Play(true);
			}
		}

		private void OnBootsGrounded()
		{
			if (GuideArrow != null)
			{
				GuideArrow.Stop(true);
			}

			if (DockEffect != null)
			{
				DockEffect.Play(true);
			}
		}

		private void OnActivated()
		{
			if (ActivateEffect != null)
			{
				ActivateEffect.Play(true);
			}

			if (GuideArrow != null)
			{
				GuideArrow.Play(true);
			}

			if (MeshRenderer != null)
			{
				MeshRenderer.material.SetColor(ColorPropertyName, Activated);
			}
		}

		private void OnDeactivated()
		{
			if (GuideArrow != null)
			{
				GuideArrow.Stop(true);
			}

			if (MeshRenderer != null)
			{
				MeshRenderer.material.SetColor(ColorPropertyName, Deactivated);
			}
		}

		private void OnDisable()
		{
			PlayerBootsModel.BootsGrounded -= OnBootsGrounded;
			PlayerBootsModel.Activated -= OnActivated;
			PlayerBootsModel.Deactivated -= OnDeactivated;
			MeshRenderer.material.SetColor(ColorPropertyName, Color.white);
		}
	}
}