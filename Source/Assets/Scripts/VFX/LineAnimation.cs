using System.Collections;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace VFX
{
	[RequireComponent(typeof(LineRenderer))]
	public class LineAnimation : MonoBehaviour
	{
		#region Events

		public UnityEvent Started;
		public UnityEvent Stopped;

		#endregion

		[SerializeField] private float Duration = 1.0f;

		[Header("Evaluated while Animated")] [SerializeField]
		private AnimationCurve Width = AnimationCurve.EaseInOut(0, 0, 1, 1);

		[SerializeField] private Gradient Color = new Gradient();
		private LineRenderer m_lineRenderer = null;
		private Coroutine m_animation = null;
		private bool m_playing = false;

		private void OnValidate()
		{
			if (m_lineRenderer == null)
			{
				m_lineRenderer = GetComponent<LineRenderer>();
			}
		}

		private void Start()
		{
			if (m_lineRenderer == null)
			{
				m_lineRenderer = GetComponent<LineRenderer>();
			}
		}

		[Button()]
		public void Play()
		{
			if (m_playing) return;

			m_animation = StartCoroutine(EvaluateAnimation());
		}

		private IEnumerator EvaluateAnimation()
		{
			var currentTime = 0.0f;
			var prevLineWidth = m_lineRenderer.widthMultiplier;
			var prevStartColor = m_lineRenderer.startColor;
			var prevEndColor = m_lineRenderer.endColor;


			m_playing = true;
			Started?.Invoke();

			while (currentTime < Duration)
			{
				var percentage = currentTime / Duration;
				m_lineRenderer.startWidth = m_lineRenderer.widthMultiplier * Width.Evaluate(percentage);
				m_lineRenderer.endWidth = m_lineRenderer.widthMultiplier * Width.Evaluate(percentage);
				m_lineRenderer.startColor = Color.Evaluate(percentage);
				m_lineRenderer.endColor = Color.Evaluate(percentage);


				currentTime += Time.deltaTime;
				yield return null;
			}

			Stopped?.Invoke();
			m_lineRenderer.widthMultiplier = prevLineWidth;
			m_lineRenderer.startColor = prevStartColor;
			m_lineRenderer.endColor = prevEndColor;
			m_playing = false;
		}
	}
}