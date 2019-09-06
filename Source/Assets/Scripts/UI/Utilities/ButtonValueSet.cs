using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI.Utilities
{
	[System.Serializable]
	public class ButtonValueSetEvent : UnityEvent<float> { }

	/// <summary>
	/// Combines 2 Buttons with a Text to increase or decrease between given values.
	/// Using Unity Events to simplify communication.
	/// </summary>
	public class ButtonValueSet : MonoBehaviour
	{
		#region Events

		public ButtonValueSetEvent OnValueChanged;

		#endregion

		[Header("Values")] [SerializeField] private float Min = 0;
		[SerializeField] private float Max = 0;
		[SerializeField] private float DefaultValue = 0;
		[SerializeField] private float Steps = 0;

		[Header("References")] [SerializeField]
		private Button Subtract = null;

		[SerializeField] private Button Add = null;
		[SerializeField] private Text Value = null;

		private float m_currentValue = 0;

		private void Start()
		{
			if (Subtract == null || Add == null || Value == null)
			{
				Debug.LogWarningFormat("{0} missing its References please check the Inspector", this);
				return;
			}

			m_currentValue = DefaultValue;

			SetValue(m_currentValue);

			AddListeners();

			OnValueChanged?.Invoke(m_currentValue);
		}

		/// <summary>
		/// Connect Buttons to Functions.
		/// </summary>
		private void AddListeners()
		{
			if (Subtract != null)
			{
				Subtract.onClick.AddListener(OnClickSubtract);
			}

			if (Add != null)
			{
				Add.onClick.AddListener(OnClickAdd);
			}
		}

		private void OnClickSubtract()
		{
			ChangeValue(-Steps);
		}

		private void OnClickAdd()
		{
			ChangeValue(Steps);
		}

		/// <summary>
		/// Change cached Value based on given parameters.
		/// </summary>
		/// <param name="changed"></param>
		private void ChangeValue(float changed)
		{
			m_currentValue = Mathf.Clamp(m_currentValue += changed, Min, Max);
			SetValue(m_currentValue);
			OnValueChanged?.Invoke(m_currentValue);
		}

		/// <summary>
		/// Sets Value to Text Component.
		/// </summary>
		/// <param name="value"></param>
		private void SetValue(float value)
		{
			Value.text = value.ToString();
		}

		public void ResetToDefault()
		{
			m_currentValue = DefaultValue;
			SetValue(m_currentValue);
			OnValueChanged?.Invoke(m_currentValue);
		}
	}
}