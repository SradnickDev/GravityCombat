using Network;
using Photon.Pun;
using UnityEngine;

namespace PlayerBehaviour.Model
{
	public class PlayerSyncModel : PlayerBaseModel, IPunObservable
	{
		[SerializeField] private float InterpolationBackTime = 0.12f; //Default 0.1, one tenth of a second
		[SerializeField] private float ExtrapolationLimit = 0.5f;

		private struct State
		{
			public double Timestamp;
			public Vector3 Position;
			public Quaternion Rotation;

			public State(double timestamp, Vector3 position, Quaternion rotation)
			{
				Timestamp = timestamp;
				Position = position;
				Rotation = rotation;
			}
		}

		private State[] m_stateBuffer = new State[30];
		private int m_stateCount = 0;

		private void FixedUpdate()
		{
			if (PhotonView.IsMine || m_stateCount == 0) return;

			var currentTime = PhotonNetwork.Time;
			var interpolationTime = currentTime - InterpolationBackTime;

			if (m_stateBuffer[0].Timestamp > interpolationTime)
			{
				Interpolation(interpolationTime);
			}
			else
			{
				Extrapolation(interpolationTime);
			}
		}

		private void Interpolation(double interpolationTime)
		{
			for (var i = 0; i < m_stateCount; i++)
			{
				//closest state that matches network Time or use oldest state
				if (m_stateBuffer[i].Timestamp <= interpolationTime || i == m_stateCount - 1)
				{
					//closest to Network
					var lhs = m_stateBuffer[i];
					//one newer
					var rhs = m_stateBuffer[Mathf.Max(i - 1, 0)];
					//time between
					var length = rhs.Timestamp - lhs.Timestamp;

					var t = 0.0f;
					if (length > 0.0001f)
					{
						t = (float) ((interpolationTime - lhs.Timestamp) / length);
					}

					Rigidbody.position = Vector3.Lerp(lhs.Position, rhs.Position, t);
					Rigidbody.rotation = Quaternion.Slerp(lhs.Rotation, rhs.Rotation, t);
					break;
				}
			}
		}

		private void Extrapolation(double interpolationTime)
		{
			var latestState = m_stateBuffer[0];

			var extrapolationLength = (float) (interpolationTime - latestState.Timestamp);
			if (extrapolationLength < ExtrapolationLimit)
			{
				Rigidbody.position = latestState.Position;
				Rigidbody.rotation = latestState.Rotation;
			}
		}

		public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
		{
			if (stream.IsWriting)
			{
				stream.SendNext(Compression.PackVector(Rigidbody.position));
				stream.SendNext(Compression.Quaternion(Rigidbody.rotation));
			}
			else
			{
				var pos = Compression.UnpackVector((int) stream.ReceiveNext());
				var rot = Compression.DecompressQuaternion((float) stream.ReceiveNext());

				var newState = new State(info.SentServerTime, pos, rot);

				AddState(newState);

				for (var i = 0; i < m_stateCount - 1; i++)
				{
					if (m_stateBuffer[i].Timestamp < m_stateBuffer[i + 1].Timestamp)
					{
						Debug.Log("State inconsistent");
					}
				}
			}
		}

		/// <summary>
		/// Shift the states to the right.Storing newest at 0.
		/// </summary>
		/// <param name="state"></param>
		private void AddState(State state)
		{
			for (var i = m_stateBuffer.Length - 1; i > 0; i--)
			{
				m_stateBuffer[i] = m_stateBuffer[i - 1];
			}

			//Newest State
			m_stateBuffer[0] = state;

			m_stateCount = Mathf.Min(m_stateCount + 1, m_stateBuffer.Length);
		}
	}
}