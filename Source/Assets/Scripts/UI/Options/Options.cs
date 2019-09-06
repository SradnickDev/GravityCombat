using UI.Flex;
using UnityEngine;

namespace UI.Options
{
	public class Options : FlexScreen
	{
		#region PlayerPrefsExtension

		public static void SetBool(string key, bool value)
		{
			var boolValue = value ? 1 : 0;
			PlayerPrefs.SetInt(key, boolValue);
		}

		public static bool GetBool(string key, bool fallback)
		{
			if (!PlayerPrefs.HasKey(key))
			{
				return fallback;
			}

			var boolValue = PlayerPrefs.GetInt(key);

			switch (boolValue)
			{
				case 0:
					return false;
				case 1:
					return true;
			}

			return false;
		}

		#endregion
	}
}