using System.Linq;
using Network.Extensions;
using Network.Gamemode;
using Photon.Pun;
using UnityEngine;

namespace PlayerBehaviour.General
{
	public class PlayerColor : MonoBehaviour
	{
		public PhotonView PhotonView = null;
		[SerializeField] private SkinnedMeshRenderer MeshRenderer = null;
		[SerializeField] private Texture[] Textures = new Texture[6];
		[SerializeField] private Texture King = null;
		[SerializeField] private Texture Friendly = null;
		[SerializeField] private Texture Enemy = null;
		private readonly int m_albedo = Shader.PropertyToID("_Albedo");

		private void Start()
		{
			if (PhotonView == null) return;
			ChangeTexture();
		}

		/// <summary>
		/// Change Texture based on Game Mode;
		/// </summary>
		private void ChangeTexture()
		{
			var gameMode = PhotonNetwork.CurrentRoom.GetGameMode();

			switch (gameMode)
			{
				case LastManStanding _:
				case Deathmatch _:
					SetRandomTexture();
					break;

				case LastTeamStanding _:
				case TeamDeathmatch _:
				case KillTheKing _:

					if (PhotonView.Owner.IsKing())
					{
						SetTexture(King);
						break;
					}

					SetTextureBasedOnTeam();
					break;
			}
		}

		/// <summary>
		/// Blue for friendly, red for enemies.
		/// </summary>
		private void SetTextureBasedOnTeam()
		{
			var texture = PhotonView.Owner.IsFriendly() ? Friendly : Enemy;
			SetTexture(texture);
		}

		/// <summary>
		/// Texture based on index in Player list.
		/// </summary>
		private void SetRandomTexture()
		{
			var idx = PhotonNetwork.PlayerList.ToList().FindIndex(x => x.ActorNumber == PhotonView.Owner.ActorNumber);
			if (idx == -1) return;
			SetTexture(Textures[idx]);
		}

		/// <summary>
		/// Sets texture.
		/// </summary>
		/// <param name="txt"></param>
		private void SetTexture(Texture txt)
		{
			MeshRenderer.material.SetTexture(m_albedo, txt);
		}
	}
}