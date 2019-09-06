namespace UI.Room
{
	/// <summary>
	/// Contains Information about the room while in Waiting Room
	/// </summary>
	public class RoomStats
	{
		private int m_playerCount = 0;
		private readonly int m_maxPlayerCount = 0;
		private readonly SceneContainer.SceneContainer m_sceneContainer = null;

		public int CurrentPlayerCount
		{
			get { return m_playerCount; }
			set { m_playerCount = value; }
		}

		public int MaxPlayerCount
		{
			get { return m_maxPlayerCount; }
		}

		public SceneContainer.SceneContainer Map
		{
			get { return m_sceneContainer; }
		}

		public bool FullRoom
		{
			get { return m_playerCount >= m_maxPlayerCount; }
		}

		public RoomStats(int maxPlayerCount, int playerCount, SceneContainer.SceneContainer sceneContainer)
		{
			m_maxPlayerCount = maxPlayerCount;
			m_playerCount = playerCount;
			m_sceneContainer = sceneContainer;
		}
	}
}