/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
* You may not use this file except in compliance with an authorized license
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
* See the License for the specific language governing permissions and limitations under the License.
* Written by Itseez3D, Inc. <support@avatarsdk.com>, May 2020
*/

using ItSeez3D.AvatarSdk.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ItSeez3D.AvatarSdkSamples.Core
{
	public class FullbodyExportSample : GettingStartedSample
	{
		public FullbodyParametersConfigurationPanel parametersPanel = null;

		private GameObject avatarObject = null;

		private IFullbodyAvatarProvider fullbodyAvatarProvider = null;

		private bool isParametersPanelActive = false;
		#region public methods
		public FullbodyExportSample()
		{
			selectedPipelineType = PipelineType.FIT_PERSON;
		}

		public void OnParametersButtonClick()
		{
			SwitchParametersPanelState();
		}
		#endregion public methods

		#region base overrided methods

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
			yield return CheckAvailablePipeline();
		}

		protected override IEnumerator GenerateAndDisplayHead(byte[] photoBytes, PipelineType pipeline)
		{
			if (avatarObject != null)
				DestroyImmediate(avatarObject);

			if (isParametersPanelActive)
				OnParametersButtonClick();

			FullbodyAvatarComputationParameters computationParameters = new FullbodyAvatarComputationParameters();
			parametersPanel.ConfigureComputationParameters(computationParameters);

			// generate avatar from the photo and get its code in the Result of request
			var initializeRequest = fullbodyAvatarProvider.InitializeFullbodyAvatarAsync(photoBytes, computationParameters, selectedPipelineType);
			yield return Await(initializeRequest);
			currentAvatarCode = initializeRequest.Result;

			StartCoroutine(SampleUtils.DisplayPhotoPreview(currentAvatarCode, photoPreview));

			var calculateRequest = fullbodyAvatarProvider.StartAndAwaitAvatarCalculationAsync(currentAvatarCode);
			yield return Await(calculateRequest);

			var downloadRequest = fullbodyAvatarProvider.RetrieveAllAvatarDataFromCloudAsync(currentAvatarCode);
			yield return Await(downloadRequest);

			string avatarDirectory = AvatarSdkMgr.FullbodyStorage().GetAvatarDirectory(currentAvatarCode);
			progressText.text = string.Format("The generated avatar can be found in the directory:\n{0}", avatarDirectory);
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
			try
			{
				System.Diagnostics.Process.Start(avatarDirectory);
			}
			catch (Exception exc)
			{
				Debug.LogErrorFormat("Unable to show output folder. Exception: {0}", exc);
			}
#endif
		}

		protected override void SetControlsInteractable(bool interactable)
		{
			base.SetControlsInteractable(interactable);
			parametersPanel.SetControlsInteractable(interactable);
		}

		protected override void OnPipelineTypeToggleChanged(PipelineType pipelineType)
		{
			base.OnPipelineTypeToggleChanged(pipelineType);

			if (avatarProvider != null && avatarProvider.IsInitialized)
				StartCoroutine(UpdateAvatarParameters());
		}
		#endregion

		#region private methods
		private IEnumerator CheckAvailablePipeline()
		{
			// Fullbody avatars are available on the Pro plan. Need to verify it.
			SetControlsInteractable(false);
			var pipelineAvailabilityRequest = avatarProvider.IsPipelineSupportedAsync(selectedPipelineType);
			yield return Await(pipelineAvailabilityRequest);
			if (pipelineAvailabilityRequest.IsError)
				yield break;

			if (pipelineAvailabilityRequest.Result == true)
			{
				yield return UpdateAvatarParameters();
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

		private IEnumerator UpdateAvatarParameters()
		{
			bool hideParametersPanel = isParametersPanelActive;
			if (hideParametersPanel)
				SwitchParametersPanelState();

			SetControlsInteractable(false);

			var parametersRequest = fullbodyAvatarProvider.GetAvailableComputationParametersAsync(selectedPipelineType);
			yield return Await(parametersRequest);
			if (parametersRequest.IsError)
			{
				Debug.LogError("Unable to get available computation parameters");
			}
			else
			{
				FullbodyAvatarComputationParameters availableParameters = parametersRequest.Result;
				parametersPanel.UpdateParameters(availableParameters);
				SetControlsInteractable(true);
				if (hideParametersPanel)
					SwitchParametersPanelState();
			}
		}

		private void SwitchParametersPanelState()
		{
			isParametersPanelActive = !isParametersPanelActive;
			parametersPanel.SwitchActiveState(isParametersPanelActive);

			progressText.gameObject.SetActive(!isParametersPanelActive);
		}
		#endregion private methods
	}
}
