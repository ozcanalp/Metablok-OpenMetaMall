/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
* You may not use this file except in compliance with an authorized license
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
* See the License for the specific language governing permissions and limitations under the License.
* Written by Itseez3D, Inc. <support@avatarsdk.com>, November 2020
*/

using ItSeez3D.AvatarSdk.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ItSeez3D.AvatarSdkSamples.Core
{
	public class FullbodyGettingStartedSample : GettingStartedSample
	{
		private GameObject avatarObject = null;

		private IFullbodyAvatarProvider fullbodyAvatarProvider = null;

		private readonly string generatedHaircutName = "generated";

		public FullbodyGettingStartedSample()
		{
			selectedPipelineType = PipelineType.FIT_PERSON;
		}

		public override void GenerateRandomAvatar()
		{
			byte[] photoBytes = null;
			if (selectedPipelineType == PipelineType.META_PERSON_MALE)
				photoBytes = photoSupplier.GetMalePhoto();
			else if (selectedPipelineType == PipelineType.META_PERSON_FEMALE)
				photoBytes = photoSupplier.GetFemalePhoto();
			else
				photoBytes = photoSupplier.GetRandomPhoto();

			StartCoroutine(GenerateAvatarFunc(photoBytes));
		}

		protected override IEnumerator Initialize()
		{
			fullbodyAvatarProvider = AvatarSdkMgr.GetFullbodyAvatarProvider();
			avatarProvider = fullbodyAvatarProvider; 
			yield return Await(avatarProvider.InitializeAsync());
			yield return CheckIfFullbodyPipelineAvailable();
		}

		protected override IEnumerator GenerateAndDisplayHead(byte[] photoBytes, PipelineType pipeline)
		{
			if (avatarObject != null)
				DestroyImmediate(avatarObject);

			// Create computation parameters.
			// By default GLTF format is used for fullbody avatars.
			FullbodyAvatarComputationParameters computationParameters = new FullbodyAvatarComputationParameters();
			// Request "generated" haircut to be computed.
			computationParameters.haircuts.names.Add(generatedHaircutName);
			computationParameters.haircuts.embed = false;

			// Generate avatar from the photo and get its code in the Result of request
			var initializeRequest = fullbodyAvatarProvider.InitializeFullbodyAvatarAsync(photoBytes, computationParameters, selectedPipelineType);
			yield return Await(initializeRequest);
			currentAvatarCode = initializeRequest.Result;

			StartCoroutine(SampleUtils.DisplayPhotoPreview(currentAvatarCode, photoPreview));

			// Wait avatar to be calculated
			var calculateRequest = fullbodyAvatarProvider.StartAndAwaitAvatarCalculationAsync(currentAvatarCode);
			yield return Await(calculateRequest);

			// Download all avatar data from the cloud and store on the local drive
			var gettingAvatarModelRequest = fullbodyAvatarProvider.RetrieveAllAvatarDataFromCloudAsync(currentAvatarCode);
			yield return Await(gettingAvatarModelRequest);

			// FullbodyAvatarLoader is used to display fullbody avatars on the scene.
			FullbodyAvatarLoader avatarLoader = new FullbodyAvatarLoader(AvatarSdkMgr.GetFullbodyAvatarProvider());
			yield return avatarLoader.LoadAvatarAsync(currentAvatarCode);

			// Show "generated" haircut
			var showHaircutRequest = avatarLoader.ShowHaircutAsync(generatedHaircutName);
			yield return Await(showHaircutRequest);

			avatarObject = avatarLoader.AvatarGameObject;
			avatarObject.AddComponent<MoveByMouse>();
		}

		private IEnumerator CheckIfFullbodyPipelineAvailable()
		{
			// Fullbody avatars are available on the Pro plan. Need to verify it.
			SetControlsInteractable(false);
			var pipelineAvailabilityRequest = avatarProvider.IsPipelineSupportedAsync(selectedPipelineType);
			yield return Await(pipelineAvailabilityRequest);
			if (pipelineAvailabilityRequest.IsError)
				yield break;

			if (pipelineAvailabilityRequest.Result == true)
			{
				progressText.text = string.Empty;
				SetControlsInteractable(true);
			}
			else
			{
				string errorMsg = "You can't generate fullbody avatars.\nThis option is available on the PRO plan.";
				progressText.text = errorMsg;
				progressText.color = Color.red;
				Debug.LogError(errorMsg);
			}
		}
	}
}
