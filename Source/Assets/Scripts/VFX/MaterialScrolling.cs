using UnityEngine;

namespace VFX
{
	public class MaterialScrolling : MonoBehaviour
	{
		[SerializeField] private Material Material = null;
		[SerializeField] private Vector2 Direction = new Vector2(0, 0);
		[SerializeField, Range(1, 100)] private float Speed = 1.0f;

		private void Update()
		{
			if (Material == null) return;
			Material.mainTextureOffset += Speed * Time.deltaTime * Direction;
		}

		private void OnDisable()
		{
			Material.mainTextureOffset = new Vector2(0, 0);
		}
	}
}