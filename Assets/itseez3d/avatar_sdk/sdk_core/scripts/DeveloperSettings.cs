/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
* You may not use this file except in compliance with an authorized license
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
* See the License for the specific language governing permissions and limitations under the License.
* Written by Itseez3D, Inc. <support@avatarsdk.com>, July 2021
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ItSeez3D.AvatarSdk.Core
{
	public static class DeveloperSettings
	{
		[Serializable]
		class DeveloperSettingsValues
		{
			public bool runtime_credentials = false;
		}

		private const string developerSettingsFilename = "developer_settings";

		private static DeveloperSettingsValues settingsValues = null;

		public static bool ProvideCredentialsAtRuntime
		{
			get
			{
				if (settingsValues == null)
					settingsValues = LoadValues();
				return settingsValues.runtime_credentials;
			}

			set
			{
				if (settingsValues == null)
					settingsValues = LoadValues();
				settingsValues.runtime_credentials = value;
				SaveValues(settingsValues);
			}
		}

		private static DeveloperSettingsValues LoadValues()
		{
			try
			{
				var asset = Resources.Load(developerSettingsFilename) as TextAsset;
				if (asset == null)
					return new DeveloperSettingsValues();

				return JsonUtility.FromJson<DeveloperSettingsValues>(asset.text);
			}
			catch (Exception ex)
			{
				Debug.LogErrorFormat("Exception during loading developer settings: {0}", ex);
			}
			return null;
		}

		private static void SaveValues(DeveloperSettingsValues values)
		{
#if UNITY_EDITOR
			string miscSettingsPath = PluginStructure.GetPluginDirectoryPath(PluginStructure.MISC_SETTINGS_RESOURCES_DIR, PathOriginOptions.FullPath);
			PluginStructure.CreatePluginDirectory(miscSettingsPath);
			var path = Path.Combine(miscSettingsPath, developerSettingsFilename + ".json");
			File.WriteAllText(path, JsonUtility.ToJson(values));
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
#else
			Debug.LogWarning("Developer Settings can be saved only in the Editor mode.");
#endif
		}
	}
}
