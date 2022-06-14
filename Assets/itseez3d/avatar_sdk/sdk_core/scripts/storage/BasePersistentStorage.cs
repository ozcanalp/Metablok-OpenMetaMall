/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
* You may not use this file except in compliance with an authorized license
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
* See the License for the specific language governing permissions and limitations under the License.
* Written by Itseez3D, Inc. <support@avatarsdk.com>, April 2017
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ItSeez3D.AvatarSdk.Core
{
	public class BasePersistentStorage
	{
		protected string dataRoot = string.Empty;

		public string EnsureDirectoryExists(string d)
		{
			if (!Directory.Exists(d))
				Directory.CreateDirectory(d);
			return d;
		}

		/// <summary>
		/// Native plugins do not currently support non-ASCII file paths. Therefore we must choose
		/// location that only contains ASCII characters in its path and is read-write accessible.
		/// This function will try different options before giving up.
		/// </summary>
		public string GetDataDirectory()
		{
			if (string.IsNullOrEmpty(dataRoot))
			{
				var options = new string[] {
					Application.persistentDataPath,
					#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
					IOUtils.CombinePaths (Environment.GetFolderPath (Environment.SpecialFolder.CommonApplicationData), "avatar_sdk"),
					IOUtils.CombinePaths ("C:\\", "avatar_sdk_data"),
					#endif
					IOUtils.CombinePaths (Application.dataPath, "..", "avatar_sdk"),
				};

				for (int i = 0; i < options.Length; ++i)
				{
					Debug.LogFormat("Trying {0} as data root...", options[i]);
					if (Utils.HasNonAscii(options[i]))
					{
						Debug.LogWarningFormat("Data path \"{0}\" contains non-ASCII characters, trying next option...", options[i]);
						continue;
					}

					try
					{
						// make sanity checks to make sure we actually have read-write access to the directory
						EnsureDirectoryExists(options[i]);
						var testFilePath = Path.Combine(options[i], "test.file");
						File.WriteAllText(testFilePath, "test");
						File.ReadAllText(testFilePath);
						File.Delete(testFilePath);
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						Debug.LogWarningFormat("Could not access {0}, trying next option...", options[i]);
						continue;
					}

					dataRoot = options[i];
					break;
				}
			}

			if (string.IsNullOrEmpty(dataRoot))
				throw new Exception("Could not find directory for persistent data! See log for details.");

			return EnsureDirectoryExists(dataRoot);
		}
	}
}
