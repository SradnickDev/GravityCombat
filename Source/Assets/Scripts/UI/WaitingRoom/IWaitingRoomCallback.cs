using Photon.Realtime;

namespace Network.WaitingRoom
{
	/// <summary>
	/// Wraps basic mechanics for a Waiting Room. Tracks Player count, Timer and sync the current state to all clients.
	/// StateChanged Event can be used to implement specific behaviour, like scene change.
	/// WaitingRoomPart has to be Initialize e.g on Start(), has to be updated ing Update and Shutdown has to be called when not needed to avoid errors with Photon Raise Event System.
	/// </summary>
	public interface IWaitingRoomCallback
	{
		void PlayerJoined(Player player);
		void PlayerLeft(Player player);
		void PlayerIsReady(Player player);
		void MasterClientLeft();
		void MatchStart();
	}
}