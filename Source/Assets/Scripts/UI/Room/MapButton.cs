using UnityEngine;
using UnityEngine.UI;

namespace UI.Room
{
	/// <summary>
	/// Simple Preview for GameMode in Menu.
	/// </summary>
	public class MapButton : MonoBehaviour
	{
		public Button Button = null;
		[SerializeField] private Image Image = null;
		[SerializeField] private Text NameLabel = null;

		public string MapName
		{
			set { NameLabel.text = value; }
		}

		public Sprite MapPreview
		{
			set { Image.sprite = value; }
		}

		private void OnEnable()
		{
			Deselected();
		}

		public void Selected()
		{
			Image.CrossFadeColor(Color.white, 0.1f, true, false);
		}

		public void Deselected()
		{
			Image.CrossFadeColor(Color.gray, 0.1f, true, false);
		}
	}
}