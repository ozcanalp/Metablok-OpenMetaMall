/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
* You may not use this file except in compliance with an authorized license
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
* See the License for the specific language governing permissions and limitations under the License.
* Written by Itseez3D, Inc. <support@avatarsdk.com>, April 2017
*/

#if UNITY_EDITOR
using System;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace ItSeez3D.AvatarSdk.Core.Editor
{
	/// <summary>
	/// Get the latest version number from the server. Show dialog if plugin needs to be updated.
	/// </summary>
	public class UpdateChecker
	{
		private string lastUpdatePrefKey;
		private string versionUrl;
		private System.Version currentVersion;
		private string pluginName;
		private Action showUpdateWindow;

		public UpdateChecker (string preferenceKey, string url, System.Version version, string pluginName, Action showWarning)
		{
			lastUpdatePrefKey = preferenceKey;
			versionUrl = url;
			currentVersion = version;
			this.pluginName = pluginName;
			showUpdateWindow = showWarning;
		}

		public void CheckOnStartup ()
		{
			bool shouldCheck = true;
			if (EditorPrefs.HasKey(lastUpdatePrefKey))
			{
				var lastCheckStr = EditorPrefs.GetString(lastUpdatePrefKey);
				DateTime lastCheck;
				if (DateTime.TryParse(lastCheckStr, out lastCheck))
				{
					var timeSinceLastCheck = DateTime.Now - lastCheck;
					if (timeSinceLastCheck.TotalHours < 72)
						shouldCheck = false;
				}
			}
			if (shouldCheck)
				CheckForUpdates(automaticCheck: true);
		}

		public void CheckForUpdates (bool automaticCheck)
		{
			var r = UnityWebRequest.Get (versionUrl);
			r.SendWebRequest();

			EditorAsync.ProcessTask (new EditorAsync.EditorAsyncTask (
				isDone: () => r.isDone,
				onCompleted: () => OnVersionKnown(r.downloadHandler.text, automaticCheck)
			));
		}

		private void OnVersionKnown (string version, bool automaticCheck)
		{
			EditorPrefs.SetString (lastUpdatePrefKey, DateTime.Now.ToString ());

			var latestVersion = new System.Version (version);
			Debug.LogFormat ("{0} latest version is: {1}, current version is {2}", pluginName, version, currentVersion);

			if (currentVersion >= latestVersion) {
				if (!automaticCheck)
					EditorUtility.DisplayDialog ("Update check", string.Format("{0} plugin is up to date!", pluginName), "Ok");
			} else {
				Debug.LogFormat (string.Format("{0} version is obsolete. Update recommended.", pluginName));
				showUpdateWindow();
			}
		}
	}
}
#endif
