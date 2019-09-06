using UnityEngine;
using UnityEngine.UI;

namespace UI.Utilities
{
	/// <summary>
	/// Combines Sprite,Color,Text into one.
	/// </summary>
	public class ValueDisplaySet : MonoBehaviour
	{
		[SerializeField] private Text TextDisplay = null;
		[SerializeField] private Image Image = null;

		public void SetText(string txt)
		{
			TextDisplay.text = txt;
		}

		public void SetImageColor(Color color)
		{
			Image.color = color;
		}

		public void SetImage(Sprite sprite)
		{
			Image.sprite = sprite;
		}
	}
}