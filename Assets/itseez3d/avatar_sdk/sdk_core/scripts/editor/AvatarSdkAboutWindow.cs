/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
* You may not use this file except in compliance with an authorized license
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
* See the License for the specific language governing permissions and limitations under the License.
* Written by Itseez3D, Inc. <support@avatarsdk.com>, March 2021
*/

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ItSeez3D.AvatarSdk.Core.Editor
{
	public class AvatarSdkAboutWindow : EditorWindow
	{
		private Texture logoTexture;

		private GUIStyle captionStyle;

		[MenuItem("Window/Avatar SDK/About")]
		static void Init()
		{
			// Get existing open window or if none, make a new one:
			AvatarSdkAboutWindow window = (AvatarSdkAboutWindow)EditorWindow.GetWindow(typeof(AvatarSdkAboutWindow));
			window.titleContent.text = "About Avatar SDK";
			window.minSize = new Vector2(320, 270);
			window.maxSize = new Vector2(320, 300);
			window.Show();
		}

		void OnGUI()
		{
			Initialize();

			Rect pos = new Rect(0,0, 320, 75);
			GUI.DrawTexture(pos, logoTexture);
			GUILayout.Space(80);

			var versionFlavours = CoreTools.DetectFlavour();
			if (versionFlavours.Count <= 0)
			{
				EditorGUILayout.HelpBox("No detected versions of the Avatar SDK", MessageType.Error);
			}
			else
			{
				foreach(Flavour flavour in versionFlavours)
				{
					var flavourTraits = flavour.GetTraits();

					EditorGUILayout.BeginVertical("Box");
					string label = string.Format("Version: {0}", flavourTraits.Version);
					EditorGUILayout.LabelField(flavourTraits.Name, captionStyle);
					EditorGUILayout.LabelField(label);
					if (GUILayout.Button("Documentation"))
					{
						DocumentationHelper.OpenDocumentationInBrowser("index.html", flavour);
					}
					if (GUILayout.Button("Check For Updates"))
					{
						UpdateCheckerUi.CheckForUpdates(flavour);
					}
					GUILayout.EndVertical();
				}
			}
		}

		void Initialize()
		{
			if (logoTexture == null)
			{
				string logoTexturePath = "Assets/itseez3d/avatar_sdk/sdk_core/resources/avatar_sdk_logo.png";
				logoTexture = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture>(logoTexturePath);
			}

			if (captionStyle == null)
			{
				captionStyle = new GUIStyle(EditorStyles.boldLabel)
				{
					fontSize = 16
				};
			}
		}
	}
}
