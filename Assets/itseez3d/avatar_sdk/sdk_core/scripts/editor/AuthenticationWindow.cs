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
using ItSeez3D.AvatarSdk.Core.Communication;
using System.Collections;
using UnityEditor;
using UnityEngine;

namespace ItSeez3D.AvatarSdk.Core.Editor
{
	[InitializeOnLoad]
	public class AuthenticationWindow : EditorWindow
	{
		#region Data

		string clientId, clientSecret;

		bool isRequestInProgress = false;

		string notificationMessage = string.Empty;
		bool showNotification = false;

		/// <summary>
		/// If window is shown right after the plugin import some weird GUI errors may occur. This is a safety mechanism.
		/// </summary>
		static int numUpdatesToWait = 4;

		public static volatile bool justUpdatedCredentials = false;

		#endregion

		#region UI

		GUIStyle titleStyle, textStyle, richTextStyle;
		readonly string linkColor = "#3366BB";

		#endregion

		/// <summary>
		/// Static constructor.
		/// </summary>
		static AuthenticationWindow ()
		{
			EditorApplication.update += InitializeOnce;
		}

		/// <summary>
		/// Show auth window if credentials file is empty.
		/// </summary>
		private static void InitializeOnce ()
		{
			--numUpdatesToWait;
			if (numUpdatesToWait > 0)
				return;
			EditorApplication.update -= InitializeOnce;

			if (!DeveloperSettings.ProvideCredentialsAtRuntime)
			{
				var credentials = AuthUtils.LoadCredentials();
				if (credentials != null && !string.IsNullOrEmpty(credentials.clientId) && !string.IsNullOrEmpty(credentials.clientSecret))
					return;

				Debug.LogFormat("Credentials not provided. Opening auth window...");
				Init();
			}
		}

		/// <summary>
		/// Menu item.
		/// </summary>
		[MenuItem ("Window/Avatar SDK/Authentication")]
		static void Init ()
		{
			var window = (AuthenticationWindow)EditorWindow.GetWindow (typeof(AuthenticationWindow));
			window.InitUI ();
			window.titleContent.text = "Avatar SDK";
			window.minSize = new Vector2 (450, 550);

			var credentials = AuthUtils.LoadCredentialsFromResources();
			if (credentials != null) {
				window.clientId = credentials.clientId;
				window.clientSecret = credentials.clientSecret;
			}
			window.Show ();
		}

		void InitUI ()
		{
			titleStyle = new GUIStyle (EditorStyles.boldLabel);
			titleStyle.alignment = TextAnchor.MiddleCenter;

			textStyle = new GUIStyle (EditorStyles.label);
			textStyle.wordWrap = true;

			richTextStyle = new GUIStyle (EditorStyles.label);
			richTextStyle.richText = true;
			richTextStyle.wordWrap = true;
		}

		/// <summary>
		/// Draw auth window interface.
		/// </summary>
		void OnGUI ()
		{
			GUILayout.Label ("Authorization", titleStyle);

			GUILayout.Label (
				"The plugin needs auth credentials to access the cloud API and to obtain a valid Local Compute SDK license. " +
				"Please follow the steps below to authorize the plugin.",
				textStyle
			);

			GUILayout.Label ("Step #1", titleStyle);
			GUILayout.Label (
				"Make sure you have Avatar SDK developer account. If you don't please register one on the sign up page:",
				textStyle
			);
			if (GUILayout.Button (string.Format ("<size=12><color={0}>Register developer account</color></size>", linkColor), richTextStyle))
				Application.OpenURL ("https://accounts.avatarsdk.com/developer/signup/");

			GUILayout.Label ("Step #2", titleStyle);
			GUILayout.Label (
				"Create an 'app' in the 'Developer' section of your Avatara SDK account:",
				richTextStyle
			);
			if (GUILayout.Button (string.Format ("<size=12><color={0}>Go to developer settings</color></size>", linkColor), richTextStyle))
				Application.OpenURL ("https://accounts.avatarsdk.com/developer/#web-api");

			GUILayout.Label ("Step #3", titleStyle);
			GUILayout.Label (
				"<color=red>Make sure that in the properties of your app the Authorization Grant " +
				"for 'Client Access' is set to 'Client credentials'!</color>",
				richTextStyle
			);

			GUILayout.Label ("Step #4", titleStyle);
			bool useRuntimeCredentials = DeveloperSettings.ProvideCredentialsAtRuntime;
			if (!useRuntimeCredentials)
			{
				GUILayout.Label(
					"Copy Client ID and Client Secret from 'Client Access' section and enter them below. " +
					"Make sure that you use 'Client Access', and not 'Developer Access'! " +
					"Press Save button below to store encrypted credentials in your app Resources.",
					textStyle
				);
				GUI.enabled = !isRequestInProgress;
				clientId = EditorGUILayout.TextField("Client Id", clientId);
				clientSecret = EditorGUILayout.TextField("Secret Key", clientSecret);
				GUILayout.Space(4);
				GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				if (GUILayout.Button("Save credentials", GUILayout.Width(150), GUILayout.Height(25)))
				{
					AuthUtils.StoreCredentials(new AccessCredentials(clientId, clientSecret));
					var credentials = AuthUtils.LoadCredentials();
					if (credentials == null || credentials.clientId != clientId || credentials.clientSecret != clientSecret)
					{
						ShowNotification(new GUIContent("Could not store credentials! See logs for details."));
					}
					else
					{
						ShowNotification(new GUIContent("Successfully saved credentials"));
						justUpdatedCredentials = true;
					}
				}
				GUI.enabled = !isRequestInProgress && !string.IsNullOrEmpty(clientId) && !string.IsNullOrEmpty(clientSecret);
				if (GUILayout.Button("Test connection", GUILayout.Width(150), GUILayout.Height(25)))
				{
					Coroutines.EditorRunner.instance.Run(TestConnection());
				}
				GUI.enabled = true;
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
			}
			bool useRuntimeCredentialsToggleValue = GUILayout.Toggle(useRuntimeCredentials, "I will provide credentials at runtime");
			if (useRuntimeCredentialsToggleValue != useRuntimeCredentials)
			{
				DeveloperSettings.ProvideCredentialsAtRuntime = useRuntimeCredentialsToggleValue;
				if (useRuntimeCredentialsToggleValue)
				{
					clientId = string.Empty;
					clientSecret = string.Empty;
					AuthUtils.StoreCredentials(new AccessCredentials(clientId, clientSecret));
				}
			}
			if (useRuntimeCredentialsToggleValue)
			{
				GUILayout.Label("You will need to set credentials in a code before initializing an AvatarProvider:", textStyle);
				EditorGUILayout.BeginVertical("Box");
				GUILayout.Label("AuthUtils.SetCredentials(\"client_id_value\", \"client_secret_value\");", textStyle);
				EditorGUILayout.EndVertical();
			}

			GUILayout.Label ("Step #5", titleStyle);
			GUILayout.Label (
				"If you entered your credentials correctly, the plugin should be authorized to " +
				"use the API. Please run any of the samples to test if the authentication works.",
				textStyle
			);
			GUILayout.Label (
				string.Format (
					"If you have any questions, problems or suggestions please write to " +
					"<size=12><color={0}>support@avatarsdk.com</color></size> " +
					"We will be glad to help you as soon as we can.",
					linkColor
				),
				richTextStyle
			);

			if (showNotification)
			{
				ShowNotification(new GUIContent(notificationMessage));
				showNotification = false;
			}
		}

		private IEnumerator TestConnection()
		{
			Debug.Log("Sending request to the server to test credentials.");
			isRequestInProgress = true;
			ConnectionBase connection = new ConnectionBase();

			var credentials = new AccessCredentials(clientId, clientSecret);
			var request = connection.GenerateAuthRequest(credentials);

			request.SendWebRequest();

			while (!request.isDone)
				yield return null;

			Debug.LogFormat("Server response: {0}", request.downloadHandler.text);
			AccessData accessData = JsonUtility.FromJson<AccessData>(request.downloadHandler.text);

			bool isError = NetworkUtils.IsWebRequestFailed(request);
			if (isError || string.IsNullOrEmpty(accessData.access_token)) {
				Debug.LogErrorFormat("Connection error: {0}", request.error);
				notificationMessage = "Unable to get access to the cloud API";
			} else if (string.IsNullOrEmpty(accessData.access_token)) {
				Debug.LogErrorFormat("Credentials are invalid: {0}", request.downloadHandler.text);
				notificationMessage = "Credentials are invalid!";
			} else {
				Debug.Log("Successful authentication!");
				notificationMessage = "Successful authentication!";
			}
			showNotification = true;
			isRequestInProgress = false;
			Repaint();
		}
	}
}
#endif