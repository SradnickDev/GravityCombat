using System;
using UnityEngine;

namespace UI
{
	public class UiSelection : MonoBehaviour
	{
		private static UiSelection m_instance = null;

		public static UiSelection Instance
		{
			get
			{
				if (m_instance == null)
				{
					var newGbj = new GameObject("UI Selection");
					m_instance = newGbj.AddComponent<UiSelection>();
				}

				return m_instance;
			}
		}

		#region Events

		public event Action<GameObject> OnNewSelection;
		public event Action<GameObject> OnSelectionChanged;
		public event Action OnSelectionRemoved;

		#endregion

		private GameObject m_currentSelection = null;
		private GameObject m_previousSelection = null;

		/// <summary>
		/// True while no GameObject selected else false.
		/// </summary>
		public bool HasFocus
		{
			get { return m_currentSelection != null; }
		}

		private void Awake()
		{
			if (m_instance != this && m_instance != null)
			{
				Destroy(m_instance);
			}

			m_instance = this;
		}

		/// <summary>
		/// Object that be focused.
		/// Invokes event for new or changed selections.
		/// </summary>
		/// <param name="newSelection">Focused GameObject</param>
		public void AddSelection(GameObject newSelection)
		{
			m_currentSelection = newSelection;

			if (m_currentSelection != m_previousSelection)
			{
				m_previousSelection = m_currentSelection;
				OnSelectionChanged?.Invoke(m_currentSelection);
			}

			OnNewSelection?.Invoke(newSelection);
		}

		/// <summary>
		/// Remove selection to allow Input.
		/// </summary>
		public void RemoveSelection(GameObject selection)
		{
			if (m_currentSelection == selection)
			{
				m_currentSelection = null;
			}

			OnSelectionRemoved?.Invoke();
		}
	}
}