using UnityEngine;

namespace UI
{
	public class Crosshair : MonoBehaviour
	{
		[SerializeField] private HitEvent HitEvent = null;
		[SerializeField] private Animator m_animator = null;
		private RectTransform m_rect = null;
		private int m_blink = Animator.StringToHash("Blink");

		private void Start()
		{
			m_rect = GetComponent<RectTransform>();
			m_animator = GetComponent<Animator>();

			Cursor.visible = false;

			if (HitEvent != null)
			{
				HitEvent.AddListener(PlayEffect);
			}
		}

		private void Update()
		{
			m_rect.position = Input.mousePosition;
		}

		private void PlayEffect()
		{
			if (m_animator != null)
			{
				m_animator.SetTrigger(m_blink);
			}
		}

		private void OnEnable()
		{
			if (HitEvent != null)
			{
				HitEvent.RemoveListener(PlayEffect);
			}

			Cursor.visible = true;
		}
	}
}