using System.Collections.Generic;
using NaughtyAttributes;
using Network.Extensions;
using UnityEngine;

namespace Network
{
	public class SpawnNode : MonoBehaviour
	{
		/// <summary>SpawnPoints</summary>
		[System.Serializable]
		public class Node
		{
			public Transform Transform;
			public bool Used = false;

			public Node(Transform transform)
			{
				Transform = transform;
			}
		}

		/// <summary>Stored SpawnPoints for a Team</summary>
		[System.Serializable]
		public class SpawnInfo
		{
			[HideInInspector] public string Name;
			public Team Team;
			public Transform NodeContainer;
			public List<Node> Nodes;
		}

		[SerializeField] private List<SpawnInfo> Nodes = new List<SpawnInfo>();

		private void Awake()
		{
			SpawnEvents.SyncSpawnNodeEvent += GetNodeBasedOnTeam;
			SpawnEvents.TeamBasedRespawnEvent += GetNodeBasedOnTeam;
			SpawnEvents.RespawnRandomEvent += RandomSpawnPoint;
		}

		private void OnDisable()
		{
			//remove from register
			SpawnEvents.SyncSpawnNodeEvent -= GetNodeBasedOnTeam;
			SpawnEvents.TeamBasedRespawnEvent -= GetNodeBasedOnTeam;
			SpawnEvents.RespawnRandomEvent -= RandomSpawnPoint;
		}

#if UNITY_EDITOR
		[Button]
		public void GetNodes()
		{
			Debug.LogWarning("Try to find Spawn Nodes.");
			foreach (var node in Nodes)
			{
				node.Nodes.Clear();
				node.Name = node.Team.ToString();
				Transform[] nodes = node.NodeContainer.transform.GetComponentsInChildren<Transform>();
				foreach (var t in nodes)
				{
					if (t != node.NodeContainer)
						node.Nodes.Add(new Node(t));
				}
			}
		}
#endif

		/// <summary>Return a unused SpawnNode based on Team</summary>
		private Transform GetNodeBasedOnTeam(Team team)
		{
			var teamSpawnInfo = GetTeamSpawnInfo(team);
			var notUsedNode = teamSpawnInfo.Nodes.Find(x => x.Used == false);

			if (notUsedNode == null)
			{
				foreach (var node in teamSpawnInfo.Nodes)
				{
					node.Used = false;
				}

				notUsedNode = teamSpawnInfo.Nodes[Random.Range(0, teamSpawnInfo.Nodes.Count - 1)];
			}

			notUsedNode.Used = true;
			return notUsedNode.Transform;
		}

		/// <summary>Return a Random SpawnNode from all Lists</summary>
		private Transform RandomSpawnPoint()
		{
			var teamSpawnInfo = Nodes[Random.Range(0, Nodes.Count)];
			var notUsedNode = teamSpawnInfo.Nodes.Find(x => x.Used == false);

			if (notUsedNode == null)
			{
				foreach (var node in teamSpawnInfo.Nodes)
				{
					node.Used = false;
				}

				notUsedNode = teamSpawnInfo.Nodes[Random.Range(0, teamSpawnInfo.Nodes.Count - 1)];
			}

			if (notUsedNode.Transform == null)
			{
				Debug.LogErrorFormat("{0} is returning an null Transform, please check SpawnNode Prefab.", this);
			}

			notUsedNode.Used = true;
			return notUsedNode.Transform;
		}

		private SpawnInfo GetTeamSpawnInfo(Team team)
		{
			return Nodes.Find(x => x.Team == team);
		}
	}
}