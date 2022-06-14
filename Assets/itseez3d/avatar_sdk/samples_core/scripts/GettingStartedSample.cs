/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
* You may not use this file except in compliance with an authorized license
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
* See the License for the specific language governing permissions and limitations under the License.
* Written by Itseez3D, Inc. <support@avatarsdk.com>, April 2017
*/

using ItSeez3D.AvatarSdk.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using ItSeez3D.AvatarSdkSamples.SamplePipelineTraits;
using ItSeez3D.AvatarSdkSamples.Core.WebCamera;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ItSeez3D.AvatarSdkSamples.Core
{
	public class GettingStartedSample : MonoBehaviour
	{
		public SdkType sdkType;

		// Test data
		public SamplePhotoSupplier photoSupplier;

		#region UI
		public GameObject sampleDescriptionPanel;
		public Text progressText;
		public Selectable[] controls;
		public Button generateHaircutButton;
		public Image photoPreview;
		public GameObject pipelineSelection;
		#endregion

		// Control to capture photo from the web camera
		public WebCameraCapturer webCameraCapturer = null;

		protected FileBrowser fileBrowser = null;

		// Instance of IAvatarProvider. Do not forget to call Dispose upon MonoBehaviour destruction.
		protected IAvatarProvider avatarProvider = null;

		// Pipeline type that will be used to generate avatar
		protected PipelineType selectedPipelineType;

		// ID of the current avatar
		protected string currentAvatarCode = string.Empty;

		// ID of the current haircut
		protected string currentHaircutId = string.Empty;

		// Pipeline type of the generated avatar
		protected PipelineType generatedAvatarPipeline;

		protected readonly string AVATAR_OBJECT_NAME = "ItSeez3D Avatar";

		protected readonly string HEAD_OBJECT_NAME = "HeadObject";

		protected readonly string HAIRCUT_OBJECT_NAME = "HaircutObject";

		protected void OnEnable()
		{
			if (sampleDescriptionPanel != null)
				sampleDescriptionPanel.SetActive(false);

			// first of all, initialize the SDK
			if (!AvatarSdkMgr.IsInitialized)
			{
				AvatarSdkMgr.Init(sdkType: sdkType);
			}

			// Provide credentials at runtime if the corresponding option is set.
			if (DeveloperSettings.ProvideCredentialsAtRuntime)
				AuthUtils.SetCredentials("", "");

		}

		protected virtual void Start()
		{
			SamplesRenderingSettings.Setup();

			var ui = controls.Select(b => b.gameObject).ToArray();
			if (!SampleUtils.CheckIfSupported(progressText, ui, sdkType))
				return;

			if (pipelineSelection != null)
			{
				var selector = pipelineSelection.GetComponent<PipelineSelector>();
				selector.PipelineTypeChanged += OnPipelineTypeToggleChanged;
				selectedPipelineType = selector.PipelineType;
			}

			StartCoroutine(Initialize());

			foreach (var b in controls)
			{
				if (b is Button)
				{
#if UNITY_EDITOR || UNITY_WEBGL || UNITY_ANDROID || UNITY_IOS
					if (b.name.Contains("UserPhoto"))
					{
						b.gameObject.SetActive(true);
						fileBrowser = b.GetComponentInChildren<FileBrowser>();
						if (fileBrowser != null)
						{
							fileBrowser.fileHandler = GenerateAvatarFunc;
						}
					}
#endif
				}
			}

			if (webCameraCapturer != null)
				webCameraCapturer.OnPhotoMade += bytes => StartCoroutine(GenerateAvatarFunc(bytes));
		}

		protected virtual void OnPipelineTypeToggleChanged(PipelineType newType)
		{
			selectedPipelineType = newType;
		}


		protected virtual IEnumerator Initialize()
		{
			//Create and intialize avatar provider
			avatarProvider = AvatarSdkMgr.GetAvatarProvider();
			yield return Await(avatarProvider.InitializeAsync());
		}

		/// <summary>
		/// Button click handler.
		/// Loads one of the predefined photos from resources.
		/// </summary>
		public virtual void GenerateRandomAvatar()
		{
			// Load random sample photo from the assets. Here you may replace it with your own photo.
			StartCoroutine(GenerateAvatarFunc(photoSupplier.GetRandomPhoto()));
		}

		/// <summary>
		/// Button click handler.
		/// Starts coroutine to generate avatar from camera's photo.
		/// </summary>
		public void GenerateAvatarFromCameraPhoto()
		{
			StartCoroutine(GenerateAvatarFromCameraPhotoAsync());
		}

		/// <summary>
		/// Launches camera application on mobile platforms, takes photo and generates avatar from it.
		/// </summary>
		private IEnumerator GenerateAvatarFromCameraPhotoAsync()
		{
			string photoPath = string.Empty;
#if UNITY_ANDROID
			AndroidImageSupplier imageSupplier = new AndroidImageSupplier();
			yield return imageSupplier.CaptureImageFromCameraAsync();
			photoPath = imageSupplier.FilePath;
#elif UNITY_IOS
			IOSImageSupplier imageSupplier = IOSImageSupplier.Create();
			yield return imageSupplier.CaptureImageFromCameraAsync();
			photoPath = imageSupplier.FilePath;
#else
			if (webCameraCapturer != null)
				webCameraCapturer.gameObject.SetActive(true);
#endif
			if (string.IsNullOrEmpty(photoPath))
				yield break;
			byte[] bytes = File.ReadAllBytes(photoPath);
			yield return GenerateAvatarFunc(bytes);
		}

		/// <summary>
		/// Destroy the existing avatar in the scene. Disable the buttons.
		/// Wait until coroutine finishes and then enable buttons again.
		/// </summary>
		protected virtual IEnumerator GenerateAvatarFunc(byte[] photoBytes)
		{
			var avatarObject = GameObject.Find(AVATAR_OBJECT_NAME);
			Destroy(avatarObject);
			SetControlsInteractable(false);
			photoPreview.gameObject.SetActive(false);
			yield return StartCoroutine(GenerateAndDisplayHead(photoBytes, selectedPipelineType));
			SetControlsInteractable(true);
			if (generateHaircutButton != null)
				generateHaircutButton.gameObject.SetActive(selectedPipelineType == PipelineType.HEAD_2_0_HEAD_MOBILE);
		}

		/// <summary>
		/// Helper function that allows to yield on multiple async requests in a coroutine.
		/// It also tracks progress on the current request(s) and updates it in UI.
		/// </summary>
		protected IEnumerator Await(params AsyncRequest[] requests)
		{
			foreach (var r in requests)
				while (!r.IsDone)
				{
					// yield null to wait until next frame (to avoid blocking the main thread)
					yield return null;

					// This function will throw on any error. Such primitive error handling only provided as
					// an example, the production app probably should be more clever about it.
					if (r.IsError)
					{
						Debug.LogError(r.ErrorMessage);
						progressText.text = r.ErrorMessage;
						SetControlsInteractable(true);
						throw new Exception(r.ErrorMessage);
					}

					// Each requests may or may not contain "subrequests" - the asynchronous subtasks needed to
					// complete the request. The progress for the requests can be tracked overall, as well as for
					// every subtask. The code below shows how to recursively iterate over current subtasks
					// to display progress for them.
					var progress = new List<string>();
					AsyncRequest request = r;
					while (request != null)
					{
						progress.Add(string.Format("{0}: {1}%", request.State, request.ProgressPercent.ToString("0.0")));
						request = request.CurrentSubrequest;
					}
					progressText.text = string.Join("\n", progress.ToArray());
				}

			progressText.text = string.Empty;
		}

		protected virtual IEnumerator ConfigureComputationParameters(PipelineType pipelineType, ComputationParameters computationParameters)
		{
			// Choose default set of parameters
			var parametersRequest = avatarProvider.GetParametersAsync(ComputationParametersSubset.DEFAULT, pipelineType);
			yield return Await(parametersRequest);
			computationParameters.haircuts = parametersRequest.Result.haircuts;
			computationParameters.blendshapes = parametersRequest.Result.blendshapes;
		}

		/// <summary>
		/// To make Getting Started sample as simple as possible all code required for creating and
		/// displaying an avatar is placed here in a single function. This function is also a good example of how to
		/// chain asynchronous requests, just like in traditional sequential code.
		/// </summary>
		protected virtual IEnumerator GenerateAndDisplayHead(byte[] photoBytes, PipelineType pipeline)
		{
			generatedAvatarPipeline = pipeline;

			ComputationParameters computationParameters = ComputationParameters.Empty;
			yield return ConfigureComputationParameters(pipeline, computationParameters);

			// generate avatar from the photo and get its code in the Result of request
			var initializeRequest = avatarProvider.InitializeAvatarAsync(photoBytes, "name", "description", pipeline, computationParameters);
			yield return Await(initializeRequest);
			currentAvatarCode = initializeRequest.Result;

			StartCoroutine(SampleUtils.DisplayPhotoPreview(currentAvatarCode, photoPreview));

			var calculateRequest = avatarProvider.StartAndAwaitAvatarCalculationAsync(currentAvatarCode);
			yield return Await(calculateRequest);

			// with known avatar code we can get TexturedMesh for head in order to show it further
			var avatarHeadRequest = avatarProvider.GetHeadMeshAsync(currentAvatarCode, false);
			yield return Await(avatarHeadRequest);
			TexturedMesh headTexturedMesh = avatarHeadRequest.Result;

			TexturedMesh haircutTexturedMesh = null;
			// get identities of all haircuts available for the generated avatar
			var haircutsIdRequest = avatarProvider.GetHaircutsIdAsync(currentAvatarCode);
			yield return Await(haircutsIdRequest);

			if (haircutsIdRequest.Result != null && haircutsIdRequest.Result.Length > 0)
			{
				var generatedHaircuts = haircutsIdRequest.Result.ToList();

				// show default haircut if it exists
				var defaultHaircut = PipelineSampleTraitsFactory.Instance.GetTraitsFromAvatarCode(currentAvatarCode).GetDefaultAvatarHaircut(currentAvatarCode);
				int haircutIdx = generatedHaircuts.FindIndex(h => h.Contains(defaultHaircut));

				// select random haircut if default doesn't exist
				if (haircutIdx < 0)
					haircutIdx = UnityEngine.Random.Range(0, generatedHaircuts.Count);

				currentHaircutId = generatedHaircuts[haircutIdx];

				// load TexturedMesh for the chosen haircut 
				var haircutRequest = avatarProvider.GetHaircutMeshAsync(currentAvatarCode, currentHaircutId);
				yield return Await(haircutRequest);
				haircutTexturedMesh = haircutRequest.Result;
			}

			DisplayHead(headTexturedMesh, haircutTexturedMesh);
		}

		/// <summary>
		/// Displays head mesh and harcut on the scene
		/// </summary>
		protected virtual void DisplayHead(TexturedMesh headMesh, TexturedMesh haircutMesh)
		{
			// create parent avatar object in a scene, attach a script to it to allow rotation by mouse
			var avatarObject = new GameObject(AVATAR_OBJECT_NAME);
			avatarObject.AddComponent<RotateByMouse>();

			// create head object in the scene
			Debug.LogFormat("Generating Unity mesh object for head...");
			var headObject = new GameObject(HEAD_OBJECT_NAME);
			var headMeshRenderer = headObject.AddComponent<SkinnedMeshRenderer>();
			headMeshRenderer.sharedMesh = headMesh.mesh;
			headMeshRenderer.material = MaterialAdjuster.GetHeadMaterial(currentAvatarCode, headMesh.texture, AvatarShaderType.UnlitShader);
			headObject.transform.SetParent(avatarObject.transform);
			if (haircutMesh != null)
			{
				// create haircut object in the scene
				var haircutObject = new GameObject(HAIRCUT_OBJECT_NAME);
				var haircutMeshRenderer = haircutObject.AddComponent<SkinnedMeshRenderer>();
				haircutMeshRenderer.sharedMesh = haircutMesh.mesh;
				haircutMeshRenderer.material = MaterialAdjuster.GetHaircutMaterial(haircutMesh.texture, currentHaircutId, AvatarShaderType.UnlitShader);
				haircutObject.transform.SetParent(avatarObject.transform);
			}
			avatarObject.transform.localScale = selectedPipelineType.SampleTraits().ViewerDisplayScale;
			avatarObject.transform.localPosition = selectedPipelineType.SampleTraits().ViewerLocalPosition;
		}

		/// <summary>
		/// Update haircut on the scene 
		/// </summary>
		protected void UpdateHaircut(TexturedMesh haircutMesh)
		{
			var avatarObject = GameObject.Find(AVATAR_OBJECT_NAME);

			var haircutObject = GameObject.Find(HAIRCUT_OBJECT_NAME);
			if (haircutObject != null)
				Destroy(haircutObject);

			if (haircutMesh != null)
			{
				haircutObject = new GameObject(HAIRCUT_OBJECT_NAME);
				var haircutMeshRenderer = haircutObject.AddComponent<SkinnedMeshRenderer>();
				haircutMeshRenderer.sharedMesh = haircutMesh.mesh;
				haircutMeshRenderer.material = MaterialAdjuster.GetHaircutMaterial(haircutMesh.texture, currentHaircutId, AvatarShaderType.UnlitShader); ;
				haircutObject.transform.SetParent(avatarObject.transform);
				haircutObject.transform.localRotation = Quaternion.identity;
				haircutObject.transform.localScale = Vector3.one;
				haircutObject.transform.localPosition = Vector3.zero;
			}
		}

		/// <summary>
		/// Allows to change controls interactability.
		/// </summary>
		protected virtual void SetControlsInteractable(bool interactable)
		{
			foreach (var c in controls)
				c.interactable = interactable;

			if(pipelineSelection!= null)
			{
				pipelineSelection.GetComponent<PipelineSelector>().Interactable = interactable;
			}
		}
	}
}
