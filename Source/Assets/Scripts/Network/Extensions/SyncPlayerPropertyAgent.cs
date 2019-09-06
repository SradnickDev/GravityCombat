using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Network.Extensions
{
	public struct PropertyData
	{
		public object Data;

		public PropertyData(object data)
		{
			Data = data;
		}
	}

	[System.Serializable]
	public class OnValueChange : UnityEvent<PropertyData> { }

	public class SyncPlayerPropertyAgent : MonoBehaviourPunCallbacks
	{
		[System.Serializable]
		public class PropertyAgent
		{
			public string Name = "";
			public string Key = "";
			public OnValueChange OnValueChange;
		}

		[SerializeField] private PhotonView OwnerView = null;
		[SerializeField] private List<PropertyAgent> PropertyAgents = new List<PropertyAgent>();

		private Player m_owner = null;

		private void Start()
		{
			if (OwnerView == null)
			{
				gameObject.SetActive(false);
				Debug.Log("No PhotonView Assigned!");
			}

			m_owner = OwnerView.Owner;
		}

		/// <summary>
		/// Receiving changed properties through photon Callback.
		/// Determine if changed property are interesting for this Agent.  
		/// </summary>
		/// <param name="targetPlayer">player that changed something</param>
		/// <param name="changedProps">changed data</param>
		public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
		{
			if (!targetPlayer.Equals(m_owner) || PropertyAgents.Count == 0) return;

			PropertyCheck(changedProps);
		}

		/// <summary>
		/// Determine if changed property contains, properties that should be tracked.
		/// </summary>
		/// <param name="changedProps">only changed properties</param>
		private void PropertyCheck(Hashtable changedProps)
		{
			foreach (var prop in changedProps)
			{
				var key = (string) prop.Key;

				foreach (var propertyAgent in PropertyAgents)
				{
					if (key != propertyAgent.Key) continue;

					OnPropertyFound(propertyAgent, prop);
				}
			}
		}

		/// <summary>
		/// Invoke value change event with changed property.
		/// </summary>
		/// <param name="propertyAgent">Target Agent</param>
		/// <param name="prop">changed prop</param>
		private void OnPropertyFound(PropertyAgent propertyAgent, DictionaryEntry prop)
		{
			propertyAgent.OnValueChange?.Invoke(new PropertyData(prop.Value));
		}

		//Test Method
		public void OnPlayerPropertyChanged(PropertyData property)
		{
			Debug.Log($" Received changes : {property.Data.GetType()} : {property.Data}");
		}
	}
}