using UnityEngine;

namespace UI.Room
{
	/// <summary>
	/// Settings for specific Room.
	/// Contains only data.
	/// Rounds, Time, SpawnTime,Playercount 
	/// </summary>
	public class RoomSettings : MonoBehaviour
	{
		#region Settings

		private byte m_playerCount = 0;
		private float m_matchTime = 0.0f;
		private int m_rounds = 0;
		private int m_weaponSpawnTime = 0;

		public byte PlayerCount
		{
			get { return m_playerCount; }
		}

		public float MatchTime
		{
			get { return m_matchTime; }
		}

		public int Rounds
		{
			get { return m_rounds; }
		}

		public int WeaponSpawnTime
		{
			get { return m_weaponSpawnTime; }
		}

		#endregion

		//Used with UnityEvents

		#region Setter

		public void SetPlayerCount(float count)
		{
			m_playerCount = (byte) count;
		}

		public void SetMatchTime(float time)
		{
			m_matchTime = (int) time * 60;
		}

		public void SetRounds(float rounds)
		{
			m_rounds = (int) rounds;
		}

		public void SetSpawnTime(float value)
		{
			m_weaponSpawnTime = (int) value;
		}

		#endregion
	}
}