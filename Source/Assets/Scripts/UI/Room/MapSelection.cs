using System;
using System.Collections.Generic;
using SceneContainer;
using UnityEngine;

namespace UI.Room
{
	/// <summary>
	/// Displays Maps in Menu.
	/// </summary>
	public class MapSelection : MonoBehaviour
	{
		public Action<SceneContainer.SceneContainer> OnMapSelected;

		[SerializeField] private SceneContainerDatabase SceneContainerDatabase = null;
		[SerializeField] private Transform MapContent = null;
		[SerializeField] private MapButton MapButton = null;

		private SceneContainer.SceneContainer m_currentSceneContainer = null;
		private Dictionary<int, MapButton> m_mapButtons = new Dictionary<int, MapButton>();
		private int m_prevIndex = -1;

		private void Start()
		{
			CreateMapButtons();
		}

		/// <summary>
		/// Creates for each Map which is not a Scene in Map Database a Button and connects it to Selection Event.
		/// </summary>
		private void CreateMapButtons()
		{
			if (MapContent == null || MapButton == null || SceneContainerDatabase == null)
			{
				Debug.LogWarningFormat("{0} missing References, please check the Inspector Fields.", this);
				return;
			}

			for (var i = 0; i < SceneContainerDatabase.Count; i++)
			{
				if (!SceneContainerDatabase.Maps[i].IsMenuScene)
				{
					var mapButton = Instantiate(MapButton.gameObject, MapContent).GetComponent<MapButton>();
					var map = SceneContainerDatabase.Maps[i];
					mapButton.MapPreview = map.MapPreview;
					mapButton.MapName = map.MapName;

					var index = i;
					mapButton.Button.onClick.AddListener(() => MapSelected(index));

					m_mapButtons.Add(index, mapButton);

					if (m_prevIndex == -1)
					{
						MapSelected(index);
					}
				}
			}
		}

		/// <summary>
		/// Fires Event.
		/// </summary>
		public void MapSelected(int idx)
		{
			SelectionChanged(idx);
			m_currentSceneContainer = SceneContainerDatabase.Maps[idx];
			OnMapSelected?.Invoke(m_currentSceneContainer);
		}

		private void SelectionChanged(int idx)
		{
			m_mapButtons[idx].Selected();

			if (m_prevIndex != -1 && m_prevIndex != idx)
			{
				m_mapButtons[m_prevIndex].Deselected();
			}

			m_prevIndex = idx;
		}
	}
}