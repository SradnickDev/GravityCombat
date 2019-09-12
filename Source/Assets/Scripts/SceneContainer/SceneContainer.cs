using UnityEngine;
using Object = UnityEngine.Object;

namespace SceneHandling
{
	[CreateAssetMenu()]
	public class SceneContainer : ScriptableObject
	{
		public Object Scene = null;
		public string MapName = string.Empty;
		public Sprite MapPreview = null;
		[TextArea(5, 10)] public string MapDescription = string.Empty;
		public bool IsMenuScene = false;
		public int SceneIndex = 0;
	}
}