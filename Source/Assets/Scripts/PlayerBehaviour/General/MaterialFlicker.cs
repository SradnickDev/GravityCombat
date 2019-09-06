using System.Collections;
using UnityEngine;

namespace PlayerBehaviour.General
{
	/// <summary>
	/// Used to create a Blink/Flicker effect on Material/Shader with a float Property like bias.
	/// </summary>
	public class MaterialFlicker : MonoBehaviour
	{
		[SerializeField] private Renderer Renderer = null;
		[SerializeField] private string PropertyName = "_Rimlightbias";
		[SerializeField] private AnimationCurve AnimationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

		private Coroutine m_coroutine;
		private float m_defaultBias = 0.0f;

		private void Start()
		{
			if (Renderer.material != null)
			{
				m_defaultBias = Renderer.material.GetFloat(PropertyName);
			}
		}

		/// <summary>
		/// Start Effect.
		/// Reset if already playing before it starts again.
		/// </summary>
		public void Play()
		{
			if (m_coroutine != null)
			{
				StopCoroutine(m_coroutine);
				Reset();
			}

			if (gameObject.activeInHierarchy)
			{
				m_coroutine = StartCoroutine(PlayEffect());
			}
		}

		/// <summary>
		/// Evaluate the shader property with the Animation Curve Value.
		/// </summary>
		private IEnumerator PlayEffect()
		{
			var time = 0.0f;
			var animLength = AnimationCurve.keys[AnimationCurve.length - 1].time;

			while (time <= animLength)
			{
				var curveValue = AnimationCurve.Evaluate(time);
				Renderer.material.SetFloat(PropertyName, curveValue);

				time += Time.deltaTime;
				yield return null;
			}

			Reset();
		}

		/// <summary>
		/// Set to default.
		/// </summary>
		private void Reset()
		{
			if (Renderer.material != null)
			{
				Renderer.material.SetFloat(PropertyName, m_defaultBias);
			}
		}

		/// <summary>
		/// In case the of playing while destroying. Reset to default.
		/// </summary>
		private void OnDestroy()
		{
			Reset();
		}
	}
}