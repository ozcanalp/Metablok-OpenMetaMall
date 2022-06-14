/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
* You may not use this file except in compliance with an authorized license
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
* See the License for the specific language governing permissions and limitations under the License.
* Written by Itseez3D, Inc. <support@avatarsdk.com>, April 2017
*/

using System.Collections;
using System.Collections.Generic;
using ItSeez3D.AvatarSdk.Core;
using ItSeez3D.AvatarSdkSamples.SamplePipelineTraits;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ItSeez3D.AvatarSdkSamples.Core
{
	public class ParametersSample : GettingStartedSample
	{
		public ParametersConfigurationPanel parametersPanel;

		protected override void OnPipelineTypeToggleChanged(PipelineType pipelineType)
		{
			base.OnPipelineTypeToggleChanged(pipelineType);

			if (avatarProvider != null && avatarProvider.IsInitialized)
				StartCoroutine(UpdateAvatarParameters());
		}

		/// <summary>
		/// Initializes avatar provider and requests available parameters
		/// </summary>
		protected override IEnumerator Initialize()
		{
			avatarProvider = AvatarSdkMgr.GetAvatarProvider();
			if (!avatarProvider.IsInitialized)
			{
				var initializeRequest = avatarProvider.InitializeAsync();
				yield return Await(initializeRequest);
				if (initializeRequest.IsError)
				{
					Debug.LogError("Avatar provider was not initialized!");
					yield break;
				}
			}

			yield return UpdateAvatarParameters();
		}

		protected override IEnumerator ConfigureComputationParameters(PipelineType pipelineType, ComputationParameters computationParameters)
		{
			parametersPanel.ConfigureComputationParameters(computationParameters);
			yield break;
		}

		/// <summary>
		/// Generates avatar with the selected set of parameters and displayed it in the AvatarViewer scene
		/// </summary>
		protected override IEnumerator GenerateAndDisplayHead(byte[] photoBytes, PipelineType pipeline)
		{
			ComputationParameters computationParameters = ComputationParameters.Empty;
			yield return ConfigureComputationParameters(pipeline, computationParameters);

			var initializeRequest = avatarProvider.InitializeAvatarAsync(photoBytes, "name", null, pipeline, computationParameters);
			yield return Await(initializeRequest);
			string avatarCode = initializeRequest.Result;

			StartCoroutine(SampleUtils.DisplayPhotoPreview(avatarCode, photoPreview));

			var calculateRequest = avatarProvider.StartAndAwaitAvatarCalculationAsync(avatarCode);
			yield return Await(calculateRequest);

			//Download avatar mesh, blendshapes and additional textures if it is Cloud version
			if (sdkType == SdkType.Cloud)
			{
				var downloadDataRequest = avatarProvider.MoveAvatarModelToLocalStorageAsync(avatarCode, false, computationParameters.blendshapes.Values.Count != 0);
				yield return Await(downloadDataRequest);

				if (computationParameters.additionalTextures.Values.Count > 0)
				{
					List<AsyncRequest> downloadTexturesRequests = new List<AsyncRequest>();
					foreach(var texture in computationParameters.additionalTextures.Values)
					{
						var request = avatarProvider.GetTextureAsync(avatarCode, texture.Name);
						downloadTexturesRequests.Add(request);
					}
					yield return Await(downloadTexturesRequests.ToArray());
				}
			}

			AvatarViewer.SetSceneParams(new AvatarViewer.SceneParams()
			{
				avatarCode = avatarCode,
				showSettings = true,
				sceneToReturn = SceneManager.GetActiveScene().name,
				avatarProvider = avatarProvider,
				useAnimations = false
			});
			SceneManager.LoadScene(PluginStructure.GetScenePath(SampleScene.AVATAR_VIEWER));
		}

		protected override void SetControlsInteractable(bool interactable)
		{
			base.SetControlsInteractable(interactable);
			parametersPanel.SetControlsInteractable(interactable);
		}

		protected IEnumerator UpdateAvatarParameters()
		{
			if (avatarProvider == null)
				yield break;

			SetControlsInteractable(false);

			// Get all available parameters
			var allParametersRequest = avatarProvider.GetParametersAsync(ComputationParametersSubset.ALL, selectedPipelineType);
			// Get default parameters
			var defaultParametersRequest = avatarProvider.GetParametersAsync(ComputationParametersSubset.DEFAULT, selectedPipelineType);
			yield return Await(allParametersRequest, defaultParametersRequest);

			if (allParametersRequest.IsError || defaultParametersRequest.IsError)
			{
				Debug.LogError("Unable to get parameters list");
				parametersPanel.UpdateParameters(null, null);
			}
			else
			{
				parametersPanel.UpdateParameters(allParametersRequest.Result, defaultParametersRequest.Result);
			}

			SetControlsInteractable(true);
		}
	}
}
