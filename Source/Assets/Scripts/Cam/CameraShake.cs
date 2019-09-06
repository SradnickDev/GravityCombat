using System.Collections;
using System.Collections.Generic;
using UI.Options;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Cam
{
	public class CameraShake : MonoBehaviour
	{
		#region Singelton

		private static CameraShake m_instance = null;

		public static CameraShake Instance
		{
			get
			{
				if (m_instance == null)
				{
					var target = FindObjectOfType<CameraShake>();
					m_instance = target;
				}

				return m_instance;
			}
		}

		#endregion

		/// <summary>
		/// Contains specific Camera Shake Settings
		/// </summary>
		[System.Serializable]
		public class ShakeProfile
		{
			[HideInInspector] public string Name = "";
			public string ShakeName = "";
			public AnimationCurve Modifier = AnimationCurve.EaseInOut(0, 0, 1, 1);
			public float Speed = 1;
			public float Strength = 1;
			public bool OverrideDuration = false;
			public float NewDuration = 0;
		}

		[SerializeField] private List<ShakeProfile> Profiles = new List<ShakeProfile>();

		private Vector3 m_cachedPosition = new Vector3(0, 0, 0);
		private Transform m_target = null;
		private Coroutine m_shakeCoroutine = null;

		private void Awake()
		{
			if (Instance != null && Instance != this)
			{
				Destroy(this.gameObject);
			}
			else
			{
				m_instance = this;
			}
		}

#if UNITY_EDITOR
		/// <summary>
		/// Sets Name + Index as a List Index Identifier.
		/// </summary>
		private void OnValidate()
		{
			for (var i = 0; i < Profiles.Count; i++)
			{
				Profiles[i].Name = "[" + i + "] " + Profiles[i].ShakeName;
			}
		}
#endif

		private void Start()
		{
			m_target = transform;
			m_cachedPosition = m_target.localPosition;
		}

		///<Summary>Start shake with index of ShakeSettings</Summary>
		public void StartShake(int shakeSettingsIndex)
		{
			if (!Options.GetBool(GameSettings.ScreenShakePref, true)) return;

			m_shakeCoroutine = StartCoroutine(EvaluateShake(shakeSettingsIndex));
		}

		///<Summary>Stop current Shake Animation.</Summary>
		public void StopShake()
		{
			if (m_shakeCoroutine != null)
			{
				StopCoroutine(m_shakeCoroutine);
			}
		}

		/// <summary>
		/// Evaluate the chosen settings to create a Camera Shake / Animation.
		/// </summary>
		/// <param name="shakeSettingsIndex">settings index</param>
		/// <returns></returns>
		private IEnumerator EvaluateShake(int shakeSettingsIndex)
		{
			var shakeSettings = Profiles[shakeSettingsIndex];     //used Settings
			var duration = shakeSettings.OverrideDuration == true //from Animation Curve last key's time
				? shakeSettings.NewDuration                       //as a duration if override not set to true
				: shakeSettings.Modifier.keys[shakeSettings.Modifier.length - 1].time;

			float timer = 0;

			while (timer <= duration)
			{
				var modifierCurve = shakeSettings.Modifier.Evaluate(timer);

				var randomInside = Random.insideUnitSphere * shakeSettings.Strength;
				var randomResult = randomInside * shakeSettings.Strength;

				m_target.localPosition = m_cachedPosition + (randomResult * modifierCurve);

				timer += Time.deltaTime * shakeSettings.Speed;
				yield return null;
			}

			m_target.localPosition = m_cachedPosition;
		}
	}
}