using System.Collections.Generic;
using UnityEngine;

namespace SCT
{
	public class ScriptableTextDisplay : MonoBehaviour
	{
		#region Singelton

		private static ScriptableTextDisplay m_instance = null;

		public static ScriptableTextDisplay Instance
		{
			get
			{
				if (m_instance == null)
				{
					var target = FindObjectOfType<ScriptableTextDisplay>();
					m_instance = target;
				}

				return m_instance;
			}
		}

		#endregion

		[SerializeField] private Camera m_targetCamera = null;
		[SerializeField] private int m_poolSize = 0;
		[SerializeField] private GameObject m_objectToPool = null;
		[SerializeField] private ScriptableTextTypeList m_textTypeList = null;

		public ScriptableTextTypeList TextTypeList
		{
			get { return m_textTypeList; }
		}

		private Dictionary<string, ScriptableTextComponent> m_stackingText =
			new Dictionary<string, ScriptableTextComponent>();

		//for each pool one specific parent and List
		[System.Serializable]
		public class ObjectPool
		{
			public Transform PoolHolder;
			public List<GameObject> GameObject = new List<GameObject>();
			public List<ScriptableTextComponent> Component = new List<ScriptableTextComponent>();
		}

		private ObjectPool[] m_objectPool;

		private void Awake()
		{
			if (Instance != null && Instance != this)
			{
				Destroy(this.gameObject);
			}
			else
			{
				m_instance = this;
			}
		}

		private void Start()
		{
			InitializePools();
		}

		void CreateParent()
		{
			//Initialize Array with Length of TextTypeList
			m_objectPool = new ObjectPool[m_textTypeList.ListSize];

			for (int i = 0; i < m_textTypeList.ListSize; i++)
			{
				ObjectPool newOP = new ObjectPool
				{
					PoolHolder = new GameObject(m_textTypeList.GetName(i) + "Parent", typeof(RectTransform)).transform
				};

				newOP.PoolHolder.SetParent(this.gameObject.transform);
				newOP.PoolHolder.localScale = new Vector3(1, 1, 1);
				m_objectPool[i] = newOP;
			}
		}

		void InitializePools()
		{
			//Create Parent for each TextType 
			CreateParent();
			//Create x Amount of Objects
			//store each Object in the right pool and list
			//set Pool Parent
			for (int t = 0; t < m_textTypeList.ScriptableTextTyps.Count; t++)
			{
				for (int i = 0; i < m_poolSize; i++)
				{
					GameObject gbj = Instantiate(m_objectToPool);
					gbj.SetActive(false);
					gbj.transform.SetParent(m_objectPool[t].PoolHolder, false);
					m_objectPool[t].GameObject.Add(gbj);
					m_objectPool[t].Component.Add(gbj.GetComponent<ScriptableTextComponent>());
				}
			}
		}

		private int GetIndex(int index)
		{
			//look through all GameObjects in the array
			for (int i = 0; i < m_objectPool[index].GameObject.Count; i++)
			{
				//If one GameObject is not Active, return this GameObject
				if (!m_objectPool[index].GameObject[i].activeInHierarchy)
				{
					//enable the GameObject before return
					m_objectPool[index].GameObject[i].SetActive(true);
					return i;
				}
			}

			//If there is no activ GameObject the for loop just return nothing and this code get called
			//creates an GameObject, parent it,add it to the Pool(object itself and Component), activate it and return it
			GameObject newGameObject = Instantiate(m_objectToPool, m_objectPool[index].PoolHolder, false);
			m_objectPool[index].GameObject.Add(newGameObject);
			m_objectPool[index].Component.Add(newGameObject.GetComponent<ScriptableTextComponent>());
			newGameObject.SetActive(true);
			return m_objectPool[index].GameObject.Count - 1;
		}

		/// <summary>
		/// Initialize a Scritpable Text.
		/// </summary>
		/// <param Name="listPosition">From wich CustomType in your List. eg [0]DamageText</param>
		/// <param Name="pos">Just paste your Position, handle offset and Randomnessin your SCT</param>
		/// <param Name="msg">What you want to output.</param>
		public void InitializeScriptableText(int listPosition, Vector3 pos, string msg)
		{
			//Get the position from an activ and ready to use GameObject
			int poolArrayIndex = GetIndex(listPosition);
			//prepare start Positions
			var sct = m_textTypeList.ScriptableTextTyps[listPosition];
			// call Initialize Mehtod 
			m_objectPool[listPosition].Component[poolArrayIndex].Initialize(sct, pos, msg, m_targetCamera);
		}

		public void InitializeStackingScriptableText(int listPosition, Vector3 pos, string value, string name)
		{
			if (m_stackingText.ContainsKey(name) == true)
			{
				if (!m_stackingText[name].gameObject.activeInHierarchy)
					m_stackingText.Remove(name);
			}

			ScriptableText sct = m_textTypeList.ScriptableTextTyps[listPosition];
			if (m_stackingText.ContainsKey(name) == false)
			{
				//Get the position from an activ and ready to use GameObject
				int poolArrayIndex = GetIndex(listPosition);
				//prepare start Positions

				// call Initialize Mehtod 

				ScriptableTextComponent scriptableTextComponent = m_objectPool[listPosition].Component[poolArrayIndex];
				scriptableTextComponent.Initialize(sct, pos, value, m_targetCamera);
				m_stackingText.Add(name, scriptableTextComponent);
			}
			else
			{
				m_stackingText[name].SetStackValue(sct, value, pos);
			}
		}

		/// <summary>
		/// Disable all ScriptableTextDisplay GameObjects.
		/// </summary>
		public void DisableAll()
		{
			for (int i = 0; i < m_textTypeList.ListSize; i++)
			{
				for (int p = 0; p < m_objectPool[i].GameObject.Count - 1; p++)
				{
					m_objectPool[i].GameObject[p].SetActive(false);
				}
			}
		}

		/// <summary>
		/// /// Disable all ScriptableTextDisplay GameObjects from a specific type
		/// </summary>
		/// <param name="idx">Text Type index</param>
		public void DisableAll(int idx)
		{
			for (int p = 0; p < m_objectPool[idx].GameObject.Count - 1; p++)
			{
				m_objectPool[idx].GameObject[p].SetActive(false);
			}
		}
	}
}