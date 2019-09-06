using System.Collections;
using UnityEngine;

namespace PlayerBehaviour.General
{
	public class Ragdoll : MonoBehaviour
	{
		[SerializeField] private float DeleteAfterTime = 5.0f;
		[SerializeField] private float InitForce = 4.0f;
		[SerializeField] private ForceMode ForceMode = ForceMode.Impulse;
		[SerializeField] private Rigidbody Hip = null;
		[SerializeField] private GameObject Mesh = null;
		[SerializeField] private ParticleSystem ExplosionEffect = null;
		[SerializeField] private SkinnedMeshRenderer MeshRenderer = null;
		private readonly int m_albedo = Shader.PropertyToID("_Albedo");

		public void Setup(Texture texture)
		{
			MeshRenderer.material.SetTexture(m_albedo, texture);
			StartCoroutine(DelayView());
			PlayExplosionEffect();

			var randomDir = Random.insideUnitCircle;

			CreateForce(randomDir);

			StartCoroutine(Destroy());
		}

		private void CreateForce(Vector2 randomDir)
		{
			Hip.AddForce(new Vector3(randomDir.x, randomDir.y, 0) * InitForce, ForceMode);
			Hip.AddTorque(new Vector3(randomDir.x, randomDir.y, 0) * InitForce, ForceMode);
		}

		private void PlayExplosionEffect()
		{
			if (ExplosionEffect == null) return;

			ExplosionEffect.Play(true);
		}

		private IEnumerator Destroy()
		{
			yield return new WaitForSeconds(DeleteAfterTime);
			Destroy(this.gameObject);
		}

		private IEnumerator DelayView()
		{
			yield return new WaitForSeconds(0.25f);
			Mesh.SetActive(true);
		}
	}
}