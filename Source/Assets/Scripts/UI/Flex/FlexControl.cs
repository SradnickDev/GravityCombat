using UnityEngine;
using UnityEngine.Events;

// Note:
// From an older Project, not entirely written for Gravity Combat.

namespace UI.Flex
{
	public class FlexControl : MonoBehaviour
	{
		#region Main Properties

		[SerializeField] protected FlexScreen m_startScreen;

		public FlexScreen StartScreen
		{
			get
			{
				if (m_startScreen == null)
				{
					var fallbackScreen = transform.GetChild(0).GetComponent<FlexScreen>();
					Debug.LogWarning("No Start Screen Found, please assign one!");
					return fallbackScreen;
				}
				else
				{
					return m_startScreen;
				}
			}
		}

		#endregion

		#region  Events

		public UnityEvent OnSwitchScreens;

		#endregion

		private FlexScreen[] m_screens;

		private FlexScreen m_previousScreen;

		public FlexScreen PreviousScreen
		{
			get { return m_previousScreen; }
		}

		private FlexScreen m_currentScreen;

		public FlexScreen CurrentScreen
		{
			get { return m_currentScreen; }
		}

		public virtual void Awake()
		{
			Initialize();
		}

		/// <summary>
		/// Disable all Children with FlexScreen Components
		/// </summary>
		private void Initialize()
		{
			m_screens = GetComponentsInChildren<FlexScreen>(true);

			foreach (var screen in m_screens)
			{
				screen.gameObject.SetActive(false);
			}
		}

		protected virtual void Start()
		{
			OnSwitchScreen(m_startScreen);
		}

		/// <summary>
		/// Disable previouse Screen enable new Screen
		/// </summary>
		/// <param name="screen">new Screen to enable</param>
		public void OnSwitchScreen(FlexScreen screen)
		{
			if (!screen) return;

			if (m_currentScreen)
			{
				m_currentScreen.Close();
				m_previousScreen = m_currentScreen;
			}

			m_currentScreen = screen;
			m_currentScreen.Open();

			if (OnSwitchScreens != null)
			{
				OnSwitchScreens.Invoke();
			}
		}
	}
}