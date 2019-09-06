using System;
using System.Collections.Generic;
using System.Linq;
using Network.Extensions;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Network.Gamemode
{
	/// <summary>
	/// Base class for GameModes.
	/// </summary>
	public abstract class GameModeBase
	{
		#region Events

		public Action OnConditionReached;

		#endregion

		public string Name => this.ToString();
		protected readonly Teams Teams = new Teams();
		protected string[] Stats = new string[4];
		private bool m_ready = false;

		protected GameModeBase() { }

		/// <summary>Used to start a GameMode.</summary>
		public virtual void Start()
		{
			m_ready = true;
		}

		/// <summary>
		/// All Game Modes that where created.
		/// </summary>
		public static List<string> GetAll()
		{
			var targetType = typeof(GameModeBase);
			var assembly = System.Reflection.Assembly.GetExecutingAssembly();
			var types = assembly.GetTypes();
			var subclasses = types.Where(t => t.BaseType == targetType);

			return subclasses.Select(type => type.Name).ToList();
		}

		/// <summary>
		/// Should be overriden for new GameModes to add a individual Team management.
		/// </summary>
		/// <param name="player"></param>
		protected abstract void Assign(Player player);

		public virtual void HandlePlayer()
		{
			if (PhotonNetwork.IsMasterClient)
			{
				foreach (var player in PhotonNetwork.PlayerList)
				{
					Assign(player);
				}
			}
		}

		/// <summary>
		/// Can be used to create an individual spawn mechanic for a game mode.
		/// </summary>
		/// <param name="player"></param>
		public abstract void SpawnPlayer(Player player);

		/// <summary>
		/// To allow respawning.
		/// </summary>
		/// <returns></returns>
		public abstract bool AllowRespawn();

		/// <summary>
		/// Should be used to Update specific GameMode win conditions.
		/// </summary>
		public void UpdateWinCondition()
		{
			VerifyWinCondition();
		}

		/// <summary>
		/// Can be implemented for specific condition when a gameMode usually ends.
		/// </summary>
		/// <returns></returns>
		protected virtual bool WinCondition()
		{
			return false;
		}

		/// <summary>
		/// Can be implemented to add a specific Condition to end an Match. OnConditionReached should be called if implemented condition is true.
		/// Will be called from OnRoomPropertiesUpdate and OnPlayerPropertiesUpdate
		/// </summary>
		private void VerifyWinCondition()
		{
			if (!m_ready) return;
			if (WinCondition())
			{
				Debug.Log("GameMode Verified Win.");
				OnConditionReached?.Invoke();
			}
		}

		/// <summary>
		/// Should return the leading Player, Team or specific GameMode stuff.
		/// </summary>
		/// <returns></returns>
		public abstract string GetLeading();

		public string[] GetStats()
		{
			return ConfigStats();
		}

		/// <summary>
		/// Implementation should be in a fixed order.
		/// 0 Friendly
		/// 1 Opponent
		/// 2 Counter
		/// 3 Bar
		/// </summary>
		/// <returns></returns>
		protected abstract string[] ConfigStats();
	}
}