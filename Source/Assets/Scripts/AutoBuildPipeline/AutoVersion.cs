#if UNITY_EDITOR

using UnityEditor;

namespace SradnickDev.BuildPipe
{
	public static class AutoVersion
	{
		public static void HandleVersion(bool increaseVersion, bool useSuffix)
		{
			//split version string at .
			string[] bundleVersionSplit = PlayerSettings.bundleVersion.Split('.');

			//convert to default convetion
			bundleVersionSplit = bundleVersionSplit.ToDefault();

			//last char is suffix
			var suffix = bundleVersionSplit[1];
			//default suffix
			var letter = 'a';

			//delete last char if suffix has 3 chars
			if (suffix.Length > 2)
			{
				bundleVersionSplit[1] = bundleVersionSplit[1].Remove(bundleVersionSplit[1].Length - 1);
			}

			//version numbers
			var n1 = int.Parse(bundleVersionSplit[0]);
			var n2 = int.Parse(bundleVersionSplit[1]);

			if (useSuffix)
			{
				//suffix has more then 2 chars
				//get the last char of it
				if (suffix.Length > 2)
				{
					//get suffix and increase it
					letter = suffix[suffix.Length - 1];
					letter = IncreaseWithSuffix(letter);
				}
			}

			n2 = IncreaseVersion(increaseVersion, n2, ref n1);

			var buildVersion = n1 + "." + n2.ToString("D2");

			if (useSuffix)
			{
				buildVersion += letter;
			}

			//If just a build is created don't change build number
			if (!useSuffix && !increaseVersion)
			{
				buildVersion = PlayerSettings.bundleVersion;
			}

			SetBuildVersion(buildVersion);
		}

		private static void SetBuildVersion(string buildVersion)
		{
			var buildNumber = int.Parse(PlayerSettings.macOS.buildNumber) + 1;

			PlayerSettings.bundleVersion = buildVersion;
			PlayerSettings.macOS.buildNumber = buildNumber.ToString();
		}

		private static int IncreaseVersion(bool increaseVersion, int n2, ref int n1)
		{
			if (!increaseVersion) return n2;

			if (n2 == 99)
			{
				n1 += 1;
				n2 = 0;
			}

			n2++;

			return n2;
		}

		private static string[] ToDefault(this string[] bundleVersionSplit)
		{
			//default version if size is to small
			if (bundleVersionSplit.Length == 1)
			{
				bundleVersionSplit = new[] {"0", "0"};
			}

			return bundleVersionSplit;
		}

		private static char IncreaseWithSuffix(char s)
		{
			const string alp = "abcdefghijklmnopqrstuvwxyz";
			var t = alp.IndexOf(s);

			if (s == alp[alp.Length - 1])
			{
				return s;
			}

			return alp[t + 1];
		}
	}
}
#endif