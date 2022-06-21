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
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using ItSeez3D.AvatarSdk.Cloud;
using ItSeez3D.AvatarSdk.Core;
using ItSeez3D.AvatarSdkSamples.Core;
using System.Runtime.InteropServices;
using System.Text;
using ItSeez3D.AvatarSdkSamples.SamplePipelineTraits;

namespace ItSeez3D.AvatarSdkSamples.Cloud
{
	/// <summary>
	/// Avatar states for a simple "state machine" implemented within WebglSample class.
	/// </summary>
	public enum WebglAvatarState
	{
		DEFAULT,
		// avatar image is being uploaded to the server
		UPLOADING,
		// avatar is being calculated on the server (all server states like Queued and Computing go here)
		CALCULATING_IN_CLOUD,
		// downloading results from server, avatar should be in "Completed" state on the server
		DOWNLOADING,
		// finished downloading the results, avatar is ready to be displayed in the scene
		FINISHED,
		// Calculations failed on the server, cannot download results and display avatar.
		// Make sure your photo is decent quality and resolution and contains a human face. Works best on selfies.
		FAILED,
	}

	public class WebglSample : MonoBehaviour
	{
		public GameObject mainScreen;
		public GameObject sampleImagesPanel;
		public FileBrowser fileBrowser;
		public Image image;
		public Text statusText, progressText, urlUploadingStatusText;
		public Button uploadButton, showButton, browseButton, urlButton, selfieButton;
		public GameObject browsePanel;
		public InputField urlInput;

		public QrSelfieHandler qrSelfieHandler;

		private bool controlsEnabled = true;

		private Connection connection = null;

		private PipelineType pipelineType = PipelineType.HEAD_1_2;

		// these variables should be static to maintain the previous state of the scene
		// in case the scene was reloaded
		private static byte[] selectedImageBytes = null;
		private static string selectedImageFalbackUrl = null;
		private static CloudAvatarProvider avatarProvider = null;
		private static AvatarData createdAvatar = null;
		private static WebglAvatarState avatarState;


#if UNITY_WEBGL && !UNITY_EDITOR
		private readonly string urlProxy = "https://accounts.avatarsdk.com/imgp/";

		[DllImport("__Internal")]
		private static extern void showPrompt(string message, string objectName, string callbackFuncName);
#endif

		void Start()
		{
#if !UNITY_EDITOR
#if UNITY_WEBGL
			urlInput.gameObject.SetActive(false);
			urlButton.gameObject.SetActive(true);
			HorizontalLayoutGroup browsePanelLayout = browsePanel.GetComponentInChildren<HorizontalLayoutGroup>();
			browsePanelLayout.childControlWidth = true;
#else
			browsePanel.SetActive(false);
#endif
#endif

			fileBrowser.fileHandler = HandleUploadedImage;
			StartCoroutine(Initialize());
		}

		public void OnEnterURLEnded(string value)
		{
			Debug.LogFormat("Entered: {0}", value);
			if (string.IsNullOrEmpty(value))
				return;

			StartCoroutine(UploadImageByUrl(value));
		}

		public void OnUploadButtonClick()
		{
			StartCoroutine(CreateNewAvatar());
		}

		public void OnSelfieButtonClick()
		{
			mainScreen.SetActive(false);
			qrSelfieHandler.Show(connection, new Action<bool>(isCreated =>
			{
				mainScreen.SetActive(true);

				if (!string.IsNullOrEmpty(qrSelfieHandler.SelfieCode))
				{
					selectedImageBytes = qrSelfieHandler.Selfie;
					UpdateSelectedImage(selectedImageBytes);
					image.gameObject.SetActive(true);
					StartCoroutine(CreateNewAvatar(true));
				}
			}));
		}

		public void OnShowButtonClick()
		{
			AvatarViewer.SetSceneParams(new AvatarViewer.SceneParams()
				{
					avatarCode = createdAvatar.code,
					showSettings = false,
					sceneToReturn = SceneManager.GetActiveScene().name,
					avatarProvider = WebglSample.avatarProvider,
				}
			);
			SceneManager.LoadScene(PluginStructure.GetScenePath(SampleScene.AVATAR_VIEWER));
		}

		public void OnEnterUrlClick()
		{
#if UNITY_WEBGL && !UNITY_EDITOR
			showPrompt("Enter URL", gameObject.name, "OnEnterURLEnded");
#endif
		}
 
		private void SelectDefaultImage()
		{
			var imageItems = sampleImagesPanel.GetComponentsInChildren<ImageItem>();
			int initialSampleImageIndex = 6;
			imageItems[initialSampleImageIndex].OnPointerClick(null);
		}

		private IEnumerator Initialize()
		{
			if (!AvatarSdkMgr.IsInitialized)
				AvatarSdkMgr.Init(stringMgr: new DefaultStringManager(), storage: new DefaultPersistentStorage(), sdkType: SdkType.Cloud);

			if (avatarProvider == null)
				avatarProvider = new CloudAvatarProvider();
			connection = avatarProvider.Connection;

			var imageItems = sampleImagesPanel.GetComponentsInChildren<ImageItem>();
			foreach (ImageItem item in imageItems)
			{
				item.imageSelectedHandler = HandleSelectedImage;
			}

            if (createdAvatar != null)
            {
                UpdateSelectedImage(selectedImageBytes);
				UpdateAvatarState(avatarState, PipelineType.HEAD_1_2);
            }
            else
            {
				SelectDefaultImage();
            }

            image.gameObject.SetActive(true);
            // initialize provider
            if (!avatarProvider.Connection.IsAuthorized)
			{
				yield return avatarProvider.InitializeAsync();
				if (!avatarProvider.Connection.IsAuthorized)
				{
					Debug.LogError("Authentication failed!");
					yield break;
				}
			}
		}
 
		private void UpdateSelectedImage(byte[] bytes)
		{
			ResetControlsToDefaultState();

			selectedImageBytes = bytes;
			selectedImageFalbackUrl = null;

			Texture2D jpgTexture = new Texture2D(1, 1);
			jpgTexture.LoadImage(selectedImageBytes);

			if (jpgTexture.width > 1024 || jpgTexture.height > 1024)
				jpgTexture = SampleUtils.RescaleTexture(jpgTexture, 512);

			Texture2D previewTexture = new Texture2D(jpgTexture.width, jpgTexture.height, jpgTexture.format, false);
			previewTexture.SetPixels(jpgTexture.GetPixels(0, 0, jpgTexture.width, jpgTexture.height));
			previewTexture.Apply();

			previewTexture = ImageUtils.FixExifRotation(previewTexture, bytes);

			Destroy(jpgTexture);
			jpgTexture = null;

			var color = image.color;
			color.a = 1;
			image.color = color;

			image.preserveAspect = true;
			image.sprite = Sprite.Create(previewTexture, new Rect(0, 0, previewTexture.width, previewTexture.height), Vector2.zero);
		}

		private void HandleSelectedImage(byte[] imageBytes, string fallbackImageUrl, AvatarGender gender)
		{
			if (controlsEnabled)
			{
				UpdateSelectedImage(imageBytes);
				selectedImageFalbackUrl = fallbackImageUrl;
			}
		}

		private IEnumerator HandleUploadedImage(byte[] imageBytes)
		{
			UpdateSelectedImage(imageBytes);
			yield return new WaitForEndOfFrame();
		}

		private IEnumerator UploadImageByUrl(string url, bool useProxy = true, bool changeControlsState = true)
		{
			if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
			{
				string msg = "Invalid URL";
				Debug.LogError(msg);
				ShowImageUploadingStatus(msg);
				yield break;
			}

			if (changeControlsState)
				ChangeControlsState(false);

#if !UNITY_EDITOR && UNITY_WEBGL
			// In webgl we need to use proxy due to security restrictions
			string encodedUrl = Base64.Encode(Encoding.UTF8.GetBytes(url), true);
			if(useProxy) 
			{
				url = urlProxy + encodedUrl;
			}
#endif

			UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(url);
			webRequest.SendWebRequest();

			do
			{
				yield return null;
				string statusMessage = string.Format("Downloading image: {0}%", (webRequest.downloadProgress * 100).ToString("0.0"));
				ShowImageUploadingStatus(statusMessage);
			} while (!webRequest.isDone);

			if (changeControlsState)
				ChangeControlsState(true);

			urlInput.text = string.Empty;

			if (NetworkUtils.IsWebRequestFailed(webRequest))
			{
				Debug.LogErrorFormat("Unable to upload image: {0}, Code: {1}!", webRequest.error, webRequest.responseCode);
				ShowImageUploadingStatus("Unable to get image by URL!");
				yield break;
			}

			// www.texture contains red question-mark (8x8) if no image was loaded
			// we need do detect such case and handle it as error 
			DownloadHandlerTexture texturehandler = ((DownloadHandlerTexture)webRequest.downloadHandler);
			if (texturehandler.texture == null || (texturehandler.texture.width == 8 && texturehandler.texture.height == 8))
			{
				Debug.LogErrorFormat("Unable to upload image2: {0}, Code: {1}!", webRequest.error, webRequest.responseCode);
				ShowImageUploadingStatus("Unable to get image by URL!");
				yield break;
			}

			HideImageUploadingStatus();
			if (useProxy)
				UpdateSelectedImage(webRequest.downloadHandler.data);
		}

		private IEnumerator CreateNewAvatar(bool createFromSelfie = false)
		{
			ChangeControlsState(false);
			createdAvatar = null;

			if(Uri.IsWellFormedUriString(selectedImageFalbackUrl, UriKind.Absolute))
			{
				yield return UploadImageByUrl(selectedImageFalbackUrl, false, false);
			}

			AsyncRequest<AvatarData> avatarGenerationRequest;
			if (createFromSelfie)
				avatarGenerationRequest = GenerateAvatarAsync(qrSelfieHandler.SelfieCode, pipelineType);
			else
				avatarGenerationRequest = GenerateAvatarAsync(selectedImageBytes, pipelineType);

			yield return avatarGenerationRequest;

			ChangeControlsState(true);

			if (!avatarGenerationRequest.IsError && avatarState == WebglAvatarState.FINISHED)
			{
				createdAvatar = avatarGenerationRequest.Result;
				OnShowButtonClick();
			}
		}

		private AsyncRequest<AvatarData> GenerateAvatarAsync(string selfieCode, PipelineType pipelineType)
		{
			var request = new AsyncRequest<AvatarData>();
			AvatarSdkMgr.SpawnCoroutine(GenerateAvatarFunc(null, pipelineType, request, selfieCode));
			return request;
		}

		private AsyncRequest<AvatarData> GenerateAvatarAsync(byte[] selectedImageBytes, PipelineType pipelineType)
		{
			var request = new AsyncRequest<AvatarData>();
			AvatarSdkMgr.SpawnCoroutine(GenerateAvatarFunc(selectedImageBytes, pipelineType, request));
			return request;
		}

		private AsyncRequest DownloadAvatarAsync(AvatarData avatar, PipelineType pipelineType)
		{
			var request = new AsyncRequest();
			AvatarSdkMgr.SpawnCoroutine(DownloadAvatarFunc(avatar, pipelineType, request));
			return request;
		}

		private IEnumerator GenerateAvatarFunc(byte[] selectedImageBytes, PipelineType pipelineType, AsyncRequest<AvatarData> request, string selfieCode = "")
		{
			UpdateAvatarState(WebglAvatarState.UPLOADING, pipelineType);

			var defaultParametersRequest = avatarProvider.GetParametersAsync(ComputationParametersSubset.DEFAULT, pipelineType);
			yield return Await(defaultParametersRequest, pipelineType);

			// Generate all haircuts and default blendshapes to play animations
			var allParametersRequest = avatarProvider.GetParametersAsync(ComputationParametersSubset.ALL, pipelineType);
			yield return Await(allParametersRequest, pipelineType);
			if (defaultParametersRequest.IsError || allParametersRequest.IsError)
			{
				string msg = "Unable to get parameters list";
				Debug.LogError(msg);
				UpdateAvatarState(WebglAvatarState.FAILED, pipelineType);
				request.SetError(msg);
				yield break;
			}
			ComputationParameters computationParameters = ComputationParameters.Empty;
			computationParameters.haircuts = allParametersRequest.Result.haircuts;
			computationParameters.blendshapes = defaultParametersRequest.Result.blendshapes;
			computationParameters.avatarModifications = allParametersRequest.Result.avatarModifications;
			computationParameters.avatarModifications.removeSmile.Value = true;
			computationParameters.avatarModifications.removeGlasses.Value = true;
			computationParameters.avatarModifications.enhanceLighting.Value = true;
			AsyncWebRequest<AvatarData> createAvatar;
			if (string.IsNullOrEmpty(selfieCode))
			{
				createAvatar = connection.CreateAvatarWithPhotoAsync("test_avatar", null, selectedImageBytes, pipelineType, computationParameters);
			}
			else
			{
				createAvatar = connection.CreateAvatarWithSelfieAsync("test_avatar", null, selfieCode, pipelineType, computationParameters);
			}

			yield return Await(createAvatar, pipelineType);
			if (createAvatar.IsError)
			{
				Debug.LogError(createAvatar.ErrorMessage);
				UpdateAvatarState(WebglAvatarState.FAILED, pipelineType);
				request.SetError(createAvatar.ErrorMessage);
				yield break;
			}

			var avatar = createAvatar.Result;
			AsyncRequest<string> savePhoto;
			if (string.IsNullOrEmpty(selfieCode))
			{
				savePhoto = CoreTools.SaveAvatarFileAsync(selectedImageBytes, avatar.code, AvatarFile.PHOTO);
			}
			else
			{
				savePhoto = CoreTools.SaveAvatarFileAsync(qrSelfieHandler.Selfie, avatar.code, AvatarFile.PHOTO);
			}

			yield return savePhoto;

			CoreTools.SavePipelineType(pipelineType, avatar.code);

			if (savePhoto.IsError)
			{
				Debug.LogError(savePhoto.ErrorMessage);
				UpdateAvatarState(WebglAvatarState.FAILED, pipelineType);
				request.SetError(savePhoto.ErrorMessage);
				yield break;
			}

			UpdateAvatarState(WebglAvatarState.CALCULATING_IN_CLOUD, pipelineType);

			var awaitCalculations = connection.AwaitAvatarCalculationsAsync(avatar);
			yield return Await(awaitCalculations, pipelineType);

			if (awaitCalculations.IsError)
			{
				Debug.LogError(awaitCalculations.ErrorMessage);
				UpdateAvatarState(WebglAvatarState.FAILED, pipelineType);
				request.SetError(awaitCalculations.ErrorMessage);
				yield break;
			}

			AvatarData avatarData = awaitCalculations.Result;
			UpdateAvatarState(WebglAvatarState.DOWNLOADING, pipelineType);
			var downloadRequest = DownloadAvatarAsync(avatarData, pipelineType);
			yield return downloadRequest;

			if (downloadRequest.IsError)
			{
				Debug.LogError(downloadRequest.ErrorMessage);
				UpdateAvatarState(WebglAvatarState.FAILED, pipelineType);
				request.SetError(downloadRequest.ErrorMessage);
				yield break;
			}

			UpdateAvatarState(WebglAvatarState.FINISHED, pipelineType);
			request.Result = avatarData;
			request.IsDone = true;
		}

		private AsyncRequest<T> RetryUntilDoneAsync<T>(Func<AsyncRequest<T>> requestFactory, PipelineType pipelineType, int maxRetries = 3)
		{
			AsyncRequest<T> request = new AsyncRequest<T>();
			AvatarSdkMgr.SpawnCoroutine(RetryUntilDone(request, requestFactory, pipelineType));
			return request;
		}

		private AsyncRequest RetryUntilDoneAsync(Func<AsyncRequest> requestFactory, PipelineType pipelineType, int maxRetries = 3)
		{
			AsyncRequest request = new AsyncRequest();
			AvatarSdkMgr.SpawnCoroutine(RetryUntilDone(request, requestFactory, pipelineType));
			return request;
		}

		private IEnumerator RetryUntilDone<T>(AsyncRequest<T> parentRequest, Func<AsyncRequest<T>> requestFactory, PipelineType pipelineType, int maxRetries = 3)
		{
			int retryNumber = 0;
			while (true)
			{
				AsyncRequest<T> request = requestFactory();
				yield return Await(request, pipelineType);
				if (!request.IsError)
				{
					parentRequest.Result = request.Result;
					parentRequest.IsDone = true;
					yield break;
				}
				retryNumber++;
				if (retryNumber == maxRetries)
				{
					parentRequest.SetError(request.ErrorMessage);
					yield break;
				}
			}
		}

		private IEnumerator RetryUntilDone(AsyncRequest parentRequest, Func<AsyncRequest> requestFactory, PipelineType pipelineType, int maxRetries = 3)
		{
			int retryNumber = 0;
			while (true)
			{
				AsyncRequest request = requestFactory();
				yield return Await(request, pipelineType);
				if (!request.IsError)
				{
					parentRequest.IsDone = true;
					yield break;
				}
				retryNumber++;
				if (retryNumber == maxRetries)
				{
					parentRequest.SetError(request.ErrorMessage);
					yield break;
				}
			}
		}

		private IEnumerator DownloadAvatarFunc(AvatarData avatar, PipelineType pipelineType, AsyncRequest request)
		{
			var getAvatarRequest = RetryUntilDoneAsync(() => { return connection.GetAvatarAsync(avatar.code); }, pipelineType);
			yield return getAvatarRequest;
			if (getAvatarRequest.IsError)
			{
				request.SetError(getAvatarRequest.ErrorMessage);
				yield break;
			}

			var downloadAvatarRequest = RetryUntilDoneAsync(() => { return avatarProvider.DownloadAndSaveAvatarModelAsync(avatar, false, true); }, pipelineType);
			yield return downloadAvatarRequest;
			if (downloadAvatarRequest.IsError)
			{
				request.SetError(downloadAvatarRequest.ErrorMessage);
				yield break;
			}

			var defaultHaircut = PipelineSampleTraitsFactory.Instance.GetTraitsFromAvatarCode(avatar.code).GetDefaultAvatarHaircut(avatar.code);
			if (!string.IsNullOrEmpty(defaultHaircut))
			{
				var haircutsRequest = RetryUntilDoneAsync(() => { return avatarProvider.GetHaircutsIdAsync(avatar.code); }, pipelineType);
				yield return haircutsRequest;
				if (haircutsRequest.IsError)
				{
					request.SetError(haircutsRequest.ErrorMessage);
					yield break;
				}

				if (haircutsRequest.Result.FirstOrDefault(h => h.Contains(defaultHaircut)) != null)
				{
					var downloadHaircutRequest = RetryUntilDoneAsync(() => { return avatarProvider.GetHaircutMeshAsync(avatar.code, defaultHaircut); }, pipelineType);
					yield return downloadHaircutRequest;
					if (downloadHaircutRequest.IsError)
					{
						request.SetError(downloadHaircutRequest.ErrorMessage);
						yield break;
					}
				}
			}

			request.IsDone = true;
		}

		private static string StatePretty(WebglAvatarState state)
		{
			switch (state)
			{
				case WebglAvatarState.UPLOADING:
					return "Uploading photo to the server";
				case WebglAvatarState.CALCULATING_IN_CLOUD:
					return "Generating model";
				case WebglAvatarState.DOWNLOADING:
					return "Downloading avatar files";
				case WebglAvatarState.FINISHED:
					return "done";
				case WebglAvatarState.FAILED:
					return "Calculations failed, please try a different photo";
			}

			return "Unknown state";
		}

		private void UpdateAvatarState(WebglAvatarState state, PipelineType pipelineType)
		{
			Debug.LogFormat("Pipeline: {0}, state: {1}", pipelineType, state);

			if (pipelineType == PipelineType.HEAD_2_0_HEAD_MOBILE)
			{
				// Don't display avatar status of animated face pipeline
				// It is being calculated in background
				return;
			}

			avatarState = state;
			statusText.text = StatePretty(state);

			uploadButton.gameObject.SetActive(false);
			showButton.gameObject.SetActive(false);
			statusText.gameObject.SetActive(true);
			progressText.gameObject.SetActive(true);

			if (state == WebglAvatarState.FAILED)
				return;

			switch (state)
			{
				case WebglAvatarState.FINISHED:
					showButton.gameObject.SetActive(true);
					statusText.gameObject.SetActive(false);
					progressText.gameObject.SetActive(false);
					break;
				case WebglAvatarState.DEFAULT:
					statusText.gameObject.SetActive(false);
					progressText.gameObject.SetActive(false);
					uploadButton.gameObject.SetActive(true);
					break;
				default:
					break;
			}
		}

		private IEnumerator Await(AsyncRequest r, PipelineType pipelineType)
		{
			while (!r.IsDone)
			{
				yield return null;

				if (r.IsError)
				{
					Debug.LogError(r.ErrorMessage);
					if (pipelineType == PipelineType.HEAD_1_2)
						progressText.text = r.ErrorMessage;
					yield break;
				}

				if (pipelineType == PipelineType.HEAD_1_2)
				{
					int subrequestLevel = 0;
					var progress = new List<string>();
					AsyncRequest request = r;
					while (request != null && subrequestLevel < 2)
					{
						progress.Add(string.Format("{0}: {1}%", request.State, request.ProgressPercent.ToString("0.0")));
						request = request.CurrentSubrequest;
						subrequestLevel++;
					}

					progressText.text = string.Join("\n", progress.ToArray());
				}
			}

			if (pipelineType == PipelineType.HEAD_1_2)
				progressText.text = string.Empty;
		}

		private void ChangeControlsState(bool isEnabled)
		{
			controlsEnabled = isEnabled;
			browseButton.interactable = isEnabled;
			urlInput.interactable = isEnabled;
			uploadButton.interactable = isEnabled;
			showButton.interactable = isEnabled;
			urlButton.interactable = isEnabled;
			selfieButton.interactable = isEnabled;
		}

		private void ResetControlsToDefaultState()
		{
			urlUploadingStatusText.gameObject.SetActive(false);
			uploadButton.gameObject.SetActive(true);
			showButton.gameObject.SetActive(false);
			statusText.text = string.Empty;
			progressText.text = string.Empty;			
		}

		private void ShowImageUploadingStatus(string message)
		{
			urlUploadingStatusText.text = message;
			urlUploadingStatusText.gameObject.SetActive(true);
		}

		private void HideImageUploadingStatus()
		{
			urlUploadingStatusText.gameObject.SetActive(false);
		}
	}
}
