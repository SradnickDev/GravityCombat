using Photon.Pun;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Note:
// From an older Project, not entirely written for Gravity Combat.
namespace UI.Flex
{
	public abstract class FlexScreen : MonoBehaviourPunCallbacks, IFlexScreen
	{
		#region Variables

		public Selectable StartSelectable;

		#endregion

		#region UnityEvents

		public UnityEvent onOpen;
		public UnityEvent onClose;

		#endregion

		private void Start()
		{
			if (StartSelectable)
			{
				EventSystem.current.SetSelectedGameObject(StartSelectable.gameObject);
			}
		}

		public virtual void Open()
		{
			if (onOpen != null)
			{
				onOpen.Invoke();
			}

			gameObject.SetActive(true);
		}

		public virtual void Close()
		{
			if (onClose != null)
			{
				onClose.Invoke();
			}


			gameObject.SetActive(false);
		}
	}
}