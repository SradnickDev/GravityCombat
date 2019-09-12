namespace Network.WaitingRoom
{
	public enum WaitingRoomState
	{
		None = (byte) 1,
		WaitingForPlayer,
		Full,
		TimerIsOver,
		StartMatch
	}
}