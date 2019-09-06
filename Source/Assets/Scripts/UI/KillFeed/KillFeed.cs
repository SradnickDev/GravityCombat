using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using SCT;
using UnityEngine;

namespace UI.KillFeed
{
	/// <summary>
	/// Provides Methods to send a Kill feed to all Player, which can be displayed.
	/// </summary>
	[RequireComponent(typeof(PhotonView))]
	public class KillFeed : MonoBehaviour
	{
		#region Singelton

		private static KillFeed m_instance;

		public static KillFeed Instance
		{
			get { return m_instance; }
		}

		#endregion

		#region  Events

		public Action OnKill;

		#endregion

		[SerializeField] private int FeedViewAmount = 10;
		[SerializeField] private Transform FeedGroupTransform = null;
		[SerializeField] private KillFeedView KillFeedView = null;
		[SerializeField] private ScriptableTextDisplay ScriptableTextDisplay = null;

		private PhotonView m_view = null;
		private Queue<KillFeedView> m_feedViews = new Queue<KillFeedView>();

		private void Awake()
		{
			CreateInstance();
		}

		/// <summary>
		/// Create Singelton Instance
		/// </summary>
		private void CreateInstance()
		{
			if (m_instance != null && m_instance != this)
			{
				Destroy(gameObject);
			}
			else
			{
				m_instance = this;
			}
		}

		private void Start()
		{
			m_view = GetComponent<PhotonView>();
			CreateViews();
		}

		/// <summary>Create Feed Views to use for Kill Feeds.</summary>
		private void CreateViews()
		{
			for (var i = 0; i < FeedViewAmount; i++)
			{
				var newFeed = Instantiate(KillFeedView, FeedGroupTransform);
				m_feedViews.Enqueue(newFeed);
			}
		}

		/// <summary>Sends an RPC to all Clients to create a new Kill Feed message.</summary>
		/// <param name="player">Player who killed sender</param>
		public void AddFeed(Player player)
		{
			m_view.RPC("AddFeedRPC", RpcTarget.AllViaServer, player);
		}

		[PunRPC]
		private void AddFeedRPC(Player player, PhotonMessageInfo messageInfo)
		{
			AddFeed(messageInfo.Sender, player);
		}

		/// <summary>Sorts Queue and Feed Sibling Index, set new Kill feed content.</summary>
		private void AddFeed(Player killedPlayer, Player player)
		{
			if (player.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber && ScriptableTextDisplay != null)
			{
				ScriptableTextDisplay.DisableAll(7);
				ScriptableTextDisplay.InitializeScriptableText(7, new Vector3(0, 0, 0),
																"Killed " + killedPlayer.NickName);
			}

			if (player.IsLocal)
			{
				OnKill?.Invoke();
			}

			var oldFeed = m_feedViews.Dequeue();

			oldFeed.transform.SetAsFirstSibling();
			oldFeed.SetFeed(player, killedPlayer);

			m_feedViews.Enqueue(oldFeed);
		}
	}
}