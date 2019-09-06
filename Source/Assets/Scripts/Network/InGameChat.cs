using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using Photon.Pun;
using Photon.Realtime;
using UI;
using UI.Options;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Network
{
	/// <summary>
	/// Put this on a GameObject add all References
	/// add a PhotonView to the GameObject and leave it in your Room/Scene (SceneObject)
	///
	/// Included a Message Limit
	/// this will by default only allow 1 message per second
	/// If you send more,the messages will be queued and send after 1(default) second
	/// also by default it allows only 10 messages in the queue to enforce spamming.
	///
	/// It is to take care of the Photon Message Limit, so spamming wouldn't disconnect/kick you.
	/// </summary>

	// Note
	// This is from an old Project, not entirely made for Gravity Combat. 
	public class InGameChat : MonoBehaviourPunCallbacks
	{
		[SerializeField] private Text MessagePrefab = null;
		[SerializeField] private Transform ChatContent = null;
		[SerializeField] private InputField MessageInput = null;
		[SerializeField] private PhotonView PhotonView = null;
		[SerializeField] private GameObject Chat = null;
		[SerializeField, ReadOnly] private int MessagesInQueue = 0;

		private Queue<string> m_messageQueue = new Queue<string>();
		private Coroutine m_coroutineHide = null;

		private enum ChatState
		{
			Active,
			Inactive
		}

		private ChatState m_chatState = ChatState.Inactive;

		#region Message Limits

		private const double m_sendTimeLimit = 1;
		private const int m_queueLimit = 10;

		private bool m_canSend = true;
		private double m_nextSendingTime;

		#endregion

		private void Start()
		{
			if (!Options.GetBool(GameSettings.ChatPref, true))
			{
				gameObject.SetActive(false);
				return;
			}

			MessageInput.text = string.Empty;
			MessageInput.gameObject.SetActive(false);
			UiSelection.Instance.OnSelectionChanged += OnSelectionChanged;
		}

		private void OnSelectionChanged(GameObject newSelection)
		{
			if (newSelection != MessageInput)
			{
				Close();
			}
		}

		private void Update()
		{
			PlayerInput();
		}

		private void PlayerInput()
		{
			if (Input.GetKeyDown(KeyCode.Return))
			{
				switch (m_chatState)
				{
					case ChatState.Active:
						Close();
						break;
					case ChatState.Inactive:
						Open();
						break;
				}
			}
		}

		private void Open()
		{
			if (m_coroutineHide != null)
			{
				StopCoroutine(m_coroutineHide);
			}

			m_coroutineHide = StartCoroutine(Hide(false));

			UiSelection.Instance.AddSelection(MessageInput.gameObject);
			MessageInput.gameObject.SetActive(true);
			EventSystem.current.SetSelectedGameObject(MessageInput.gameObject);
			m_chatState = ChatState.Active;
		}

		private void Close()
		{
			UiSelection.Instance.RemoveSelection(MessageInput.gameObject);

			if (!string.IsNullOrEmpty(MessageInput.text))
			{
				SendSimpleMessage();
			}

			//clean input Field
			MessageInput.text = string.Empty;
			//deselect the InputField
			EventSystem.current.SetSelectedGameObject(null);
			MessageInput.gameObject.SetActive(false);

			if (m_coroutineHide != null)
			{
				StopCoroutine(m_coroutineHide);
			}

			m_coroutineHide = StartCoroutine(Hide(true));
			m_chatState = ChatState.Inactive;
		}

		private void SendSimpleMessage()
		{
			if (m_canSend)
			{
				//Calculate next Time when the Message/s can be sended
				//if less than 0 nextout is 0
				var nextOut = m_nextSendingTime - Time.time < 0.0 ? 0.0 : m_nextSendingTime - Time.time;
				HandleQueueLimit(MessageInput.text);
				StartCoroutine(HandleMessageLimit(nextOut));
			}
			else
			{
				HandleQueueLimit(MessageInput.text);
				RefreshQueueCount();
			}
		}

		/// <summary>
		/// Enqueue all Messages.
		/// If Message Queue is full stops adding and ignores Messages.
		/// </summary>
		/// <param name="msg">Message to Send.</param>
		private void HandleQueueLimit(string msg)
		{
			if (m_messageQueue.Count < m_queueLimit)
			{
				m_messageQueue.Enqueue(msg);
			}
			else
			{
				Debug.Log("Message Queue is full.Wait a moment");
			}
		}

		/// <summary>
		/// Calculate and set the Time to send a Message.
		/// "iterate" through all Messages in the Queue
		/// and stores them in as one large string.
		/// Send the large string via RPC
		/// </summary>
		/// <param name="delay"></param>
		/// <returns></returns>
		private IEnumerator HandleMessageLimit(double delay)
		{
			m_canSend = false;
			m_nextSendingTime = Time.time + m_sendTimeLimit + delay;
			yield return new WaitForSeconds((float) delay);

			var queuedMessages = string.Empty;

			while (m_messageQueue.Count > 0)
			{
				queuedMessages += m_messageQueue.Dequeue() + "\n";
			}

			RefreshQueueCount();
			this.PhotonView.RPC("SendMessage", RpcTarget.All, queuedMessages);
			m_canSend = true;
		}

		private void RefreshQueueCount()
		{
			MessagesInQueue = m_messageQueue.Count != 0 ? m_messageQueue.Count + 1 : 0;
		}

		[PunRPC]
		private void SendMessage(string text, PhotonMessageInfo info)
		{
			if (m_coroutineHide != null)
			{
				StopCoroutine(m_coroutineHide);
			}

			m_coroutineHide = StartCoroutine(Hide(false));

			CreateMessage(text, info.Sender);
		}

		private void CreateMessage(string text, Player sender)
		{
			var senderName = FormatName(sender.NickName);
			var messageText = Instantiate(MessagePrefab, ChatContent, false);

			messageText.text = senderName + " : " + text;
		}

		private void CreateMessage(string text)
		{
			var messageText = Instantiate(MessagePrefab, ChatContent, false);

			messageText.text = text;
		}

		private IEnumerator Hide(bool hideChat)
		{
			if (hideChat)
			{
				yield return new WaitForSeconds(10);
				Chat.SetActive(false);
			}
			else
			{
				Chat.SetActive(true);
			}
		}

		/// <summary>
		/// Adds some Colors and chars to the Player Name.
		/// </summary>
		/// <param name="nickName"></param>
		/// <returns></returns>
		private string FormatName(string nickName)
		{
			var senderName = nickName;

			if (string.IsNullOrEmpty(senderName))
			{
				senderName = "Someone";
			}

			var name = nickName == PhotonNetwork.LocalPlayer.NickName
				? "<color=#add8e6ff> [" + senderName + "]</color>"
				: "<color=#808080ff>" + senderName + "</color>";
			return name;
		}

		/// <summary>Check if a string is empty or contains only spaces.</summary>
		/// <param name="text">Strint to check.</param>
		/// <returns>True if Empty/spaces, Otherwise false</returns>
		private bool IsEmptyOrAllWhiteSpace(string text)
		{
			return null != text && text.All(x => x.Equals(' '));
		}

		#region PhotonCallback

		public override void OnPlayerEnteredRoom(Player newPlayer)
		{
			CreateMessage($"<color=#008000ff>{newPlayer.NickName} joined the Game. </color>");
		}

		public override void OnPlayerLeftRoom(Player otherPlayer)
		{
			CreateMessage($"{otherPlayer.NickName} left the Game.");
		}

		#endregion

		public override void OnEnable()
		{
			base.OnEnable();
			UiSelection.Instance.OnSelectionChanged -= OnSelectionChanged;
		}
	}
}