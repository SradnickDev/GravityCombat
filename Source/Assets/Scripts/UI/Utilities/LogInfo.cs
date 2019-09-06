using UnityEngine;
using UnityEngine.UI;

namespace UI.Utilities
{
	/// <summary>
	/// Used to show Messages in Menus.
	/// </summary>
	[RequireComponent(typeof(Text))]
	public class LogInfo : MonoBehaviour
	{
		public enum LogType
		{
			Default,
			Warning,
			Success
		}

		[Header("Log Settings")] [SerializeField]
		private Color DefaultInfo = Color.white;

		[SerializeField] private Color WarningInfo = Color.red;
		[SerializeField] private Color SuccessInfo = Color.green;
		[SerializeField] private Text Field;

		private void Start()
		{
			Field = GetComponent<Text>();
		}

		/// <summary>
		/// Output message with Color
		/// </summary>
		/// <param name="type">Specify Color</param>
		/// <param name="message">Message that will be displayed</param>
		public void Write(LogType type, string message)
		{
			if (!Field) return;

			switch (type)
			{
				case LogType.Default:
					Field.color = DefaultInfo;
					break;
				case LogType.Warning:
					Field.color = WarningInfo;
					break;
				case LogType.Success:
					Field.color = SuccessInfo;
					break;
			}

			Field.text = message;
		}
	}
}