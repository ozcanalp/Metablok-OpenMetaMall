/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
* You may not use this file except in compliance with an authorized license
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
* See the License for the specific language governing permissions and limitations under the License.
* Written by Itseez3D, Inc. <support@avatarsdk.com>, May 2019
*/

using System.Collections;
using System.Linq;
using ItSeez3D.AvatarSdk.Core;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;
using ItSeez3D.AvatarSdkSamples.Core;
using System;
using System.IO;

namespace ItSeez3D.AvatarSdkSamples.Cloud
{
	public class CartoonishAvatarSample : GettingStartedSample
	{
		public GameObject cartoonishLevel = null;

		public Button exportToObjButton = null;
		public Button exportToFbxButton = null;
		public Button createPrefabButton = null;

		public Text currentHaircutNameText;

		public Text currentTextureNameText;

		public GameObject haircutControls;

		public GameObject texturesControls;

		public HaircutsSelectingView haircutsSelectingView;

		public ItemsSelectingView texturesSelectingView;

		private float cartoonishValue = 0.5f;

		private int currentHaircutIndex = 0;
		private const string BALD_HAIRCUT_NAME = "bald";
		private const string GENERATED_HAIRCUT_NAME = "generated";
		private List<string> availableHaircuts = new List<string>();

		private List<string> availableTextures = new List<string>();
		private int currentTextureIndex = 0;
		private const string DEFAULT_TEXTURE_NAME = "default";

		private ModelExporter modelExporter = null;

		#region base overrided methods
		protected override IEnumerator ConfigureComputationParameters(PipelineType pipelineType, ComputationParameters computationParameters)
		{
			var parametersRequest = avatarProvider.GetParametersAsync(ComputationParametersSubset.ALL, pipelineType);
			yield return Await(parametersRequest);
			if (parametersRequest.IsError)
				yield break;

			// Generate all available blendshapes sets
			computationParameters.blendshapes = parametersRequest.Result.blendshapes;
			// Generate all available haircuts
			computationParameters.haircuts = parametersRequest.Result.haircuts;
			
			// Set available shape modifications. Values will be set further 
			computationParameters.shapeModifications = parametersRequest.Result.shapeModifications;

			switch (pipelineType)
			{
				case PipelineType.STYLED_FACE:
					ConfigureParametersForStyledFace(computationParameters);
					break;

				case PipelineType.HEAD_1_2:
					ConfigureParametersForHead12(computationParameters);
					break;

				case PipelineType.HEAD_2_0_HEAD_MOBILE:
					ConfigureParametersForHead20_HeadMobile(computationParameters);
					break;

				case PipelineType.HEAD_2_0_BUST_MOBILE:
					ConfigureParametersForHead20_BustMobile(computationParameters);
					break;
			}
		}

		protected override IEnumerator GenerateAndDisplayHead(byte[] photoBytes, PipelineType pipeline)
		{
			haircutControls.SetActive(false);
			texturesControls.SetActive(false);

			var pipelineAvailabilityRequest = avatarProvider.IsPipelineSupportedAsync(pipeline);
			yield return Await(pipelineAvailabilityRequest);
			if (!pipelineAvailabilityRequest.Result)
			{
				progressText.text = string.Format("{0} pipeline isn't available on your subscription plan!", pipeline.Traits().DisplayName);
				yield break;
			}

			ComputationParameters computationParameters = ComputationParameters.Empty;
			yield return ConfigureComputationParameters(pipeline, computationParameters);

			// generate avatar from the photo and get its code in the Result of request
			var initializeRequest = avatarProvider.InitializeAvatarAsync(photoBytes, "name", "description", pipeline, computationParameters);
			yield return Await(initializeRequest);
			currentAvatarCode = initializeRequest.Result;

			StartCoroutine(SampleUtils.DisplayPhotoPreview(currentAvatarCode, photoPreview));

			var calculateRequest = avatarProvider.StartAndAwaitAvatarCalculationAsync(currentAvatarCode);
			yield return Await(calculateRequest);

			// get identities of all haircuts available for the generated avatar
			var haircutsIdRequest = avatarProvider.GetHaircutsIdAsync(currentAvatarCode);
			yield return Await(haircutsIdRequest);

			TexturedMesh haircutMesh = null;
			if (haircutsIdRequest.Result != null)
			{
				availableHaircuts = haircutsIdRequest.Result.ToList();
				availableHaircuts.Insert(0, BALD_HAIRCUT_NAME);
				int generatedHaircutIdx = availableHaircuts.FindIndex(h => h.Contains(GENERATED_HAIRCUT_NAME));
				if (generatedHaircutIdx > 0)
				{
					currentHaircutNameText.text = GENERATED_HAIRCUT_NAME;
					currentHaircutIndex = generatedHaircutIdx;

					var haircutRequest = avatarProvider.GetHaircutMeshAsync(currentAvatarCode, availableHaircuts[currentHaircutIndex]);
					yield return Await(haircutRequest);
					haircutMesh = haircutRequest.Result;
				}
				else
				{
					currentHaircutNameText.text = BALD_HAIRCUT_NAME;
					currentHaircutIndex = 0;
				}
				haircutsSelectingView.InitItems(currentAvatarCode, availableHaircuts, avatarProvider);
			}
			else
				availableHaircuts.Clear();

			UpdateAvailableTextures(computationParameters.additionalTextures);
			currentTextureIndex = availableTextures.Count - 1;
			currentTextureNameText.text = availableTextures[currentTextureIndex];
			texturesSelectingView.InitItems(availableTextures);

			var avatarHeadRequest = avatarProvider.GetHeadMeshAsync(currentAvatarCode, false, 0, MeshFormat.PLY, GetAdditionalTextureName());
			yield return Await(avatarHeadRequest);
			TexturedMesh headTexturedMesh = avatarHeadRequest.Result;

			DisplayHead(headTexturedMesh, haircutMesh);

			var avatarObject = GameObject.Find(AVATAR_OBJECT_NAME);
			modelExporter = avatarObject.AddComponent<ModelExporter>();

			if (MeshConverter.IsExportAvailable)
				exportToObjButton.gameObject.SetActive(true);

			if (MeshConverter.IsFbxFormatSupported)
				exportToFbxButton.gameObject.SetActive(true);

#if UNITY_EDITOR_WIN
			if (MeshConverter.IsFbxFormatSupported)
				createPrefabButton.gameObject.SetActive(true);
#endif

			haircutControls.SetActive(availableHaircuts.Count > 0);
			texturesControls.SetActive(availableTextures.Count > 1);
		}

		protected override void OnPipelineTypeToggleChanged(PipelineType newType)
		{
			selectedPipelineType = newType;
			cartoonishLevel.SetActive(selectedPipelineType == PipelineType.STYLED_FACE || 
									  selectedPipelineType == PipelineType.HEAD_1_2 || 
									  selectedPipelineType == PipelineType.HEAD_2_0_HEAD_MOBILE);
		}
		#endregion

		#region UI handling
		public void OnCartoonishSliderChanged(float val)
		{
			cartoonishValue = val;
		}

		public void OnNextHaircutClick()
		{
			currentHaircutIndex = currentHaircutIndex == availableHaircuts.Count - 1 ? 0 : currentHaircutIndex + 1;
			StartCoroutine(ChangeHaircut());
		}

		public void OnPrevHaircutClick()
		{
			currentHaircutIndex = currentHaircutIndex == 0 ? availableHaircuts.Count - 1 : currentHaircutIndex - 1;
			StartCoroutine(ChangeHaircut());
		}

		public void OnNextTextureClick()
		{
			currentTextureIndex = currentTextureIndex == availableTextures.Count - 1 ? 0 : currentTextureIndex + 1;
			StartCoroutine(ChangeTexture());
		}

		public void OnPrevTextureClick()
		{
			currentTextureIndex = currentTextureIndex == 0 ? availableTextures.Count - 1 : currentTextureIndex - 1;
			StartCoroutine(ChangeTexture());
		}

		public void OnHaircutListButtonClick()
		{
			SetControlsInteractable(false);
			haircutsSelectingView.Show(new List<string>() { availableHaircuts[currentHaircutIndex] }, (list, isSelected) =>
			{
				// Find index of the selected haircut.
				currentHaircutIndex = availableHaircuts.IndexOf(list[0]);
				StartCoroutine(ChangeHaircut());
			});
		}

		public void OnTexturesListButtonClick()
		{
			SetControlsInteractable(false);
			texturesSelectingView.Show(new List<string>() { availableTextures[currentTextureIndex] }, (list, isSelected) => 
			{
				currentTextureIndex = availableTextures.IndexOf(list[0]);
				StartCoroutine(ChangeTexture());
			});
		}

		public void ExportAvatarAsObj()
		{
			var outputDir = AvatarSdkMgr.Storage().GetAvatarSubdirectory(currentAvatarCode, AvatarSubdirectory.OBJ_EXPORT);
			modelExporter.meshFormat = MeshFileFormat.OBJ;
			modelExporter.ExportModel(outputDir);

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
			try
			{
				System.Diagnostics.Process.Start(outputDir);
			}
			catch (Exception exc)
			{
				Debug.LogErrorFormat("Unable to show output folder. Exception: {0}", exc);
			}
#endif
			progressText.text = "OBJ model was saved in avatar's directory.";
			Debug.LogFormat("OBJ model was saved in {0}", outputDir);
		}

		public void ExportAvatarAsFbx()
		{
			var outputFbxDir = AvatarSdkMgr.Storage().GetAvatarSubdirectory(currentAvatarCode, AvatarSubdirectory.FBX_EXPORT);
			modelExporter.meshFormat = MeshFileFormat.FBX;
			modelExporter.ExportModel(outputFbxDir);

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
			try
			{
				System.Diagnostics.Process.Start(outputFbxDir);
			}
			catch (Exception exc)
			{
				Debug.LogErrorFormat("Unable to show output folder. Exception: {0}", exc);
			}
#endif
			progressText.text = "FBX model was saved in avatar's directory.";
			Debug.LogFormat("FBX model was saved in {0}", outputFbxDir);
		}

		public void CreateAvatarPrefab()
		{
#if UNITY_EDITOR
			string prefabDir = Path.Combine(PluginStructure.GetPluginDirectoryPath(PluginStructure.PREFABS_DIR, PathOriginOptions.RelativeToAssetsFolder), currentAvatarCode);
			AvatarPrefabBuilder.CreateAvatarPrefab(prefabDir, GameObject.Find(AVATAR_OBJECT_NAME));
#endif
		}
		#endregion UI handling

		#region private methods
		// Get the human-readable haircut name to be displayed in the text field 
		private string GetCurrentHaircutDisplayName()
		{
			string haircutName = availableHaircuts[currentHaircutIndex];
			if (haircutName != BALD_HAIRCUT_NAME)
			{
				ComputationListValue haircutProperty = new ComputationListValue(haircutName);
				haircutName = haircutProperty.Name;
			}
			return haircutName;
		}

		// Get the fully qualified name that is used to retirieve this haircut
		private string GetCurrentHaircutName()
		{
			if (availableHaircuts.Count > 0)
			{
				string haircutName = availableHaircuts[currentHaircutIndex];
				return haircutName == BALD_HAIRCUT_NAME ? null : haircutName;
			}
			return null;
		}

		private string GetAdditionalTextureName()
		{
			return availableTextures[currentTextureIndex] == DEFAULT_TEXTURE_NAME ? null : availableTextures[currentTextureIndex];
		}

		private IEnumerator ChangeHaircut()
		{
			SetControlsInteractable(false);

			currentHaircutNameText.text = GetCurrentHaircutDisplayName();
			TexturedMesh haircutMesh = null;
			string haircutName = GetCurrentHaircutName();
			if (!string.IsNullOrEmpty(haircutName))
			{
				var haircutRequest = avatarProvider.GetHaircutMeshAsync(currentAvatarCode, haircutName);
				yield return Await(haircutRequest);
				haircutMesh = haircutRequest.Result;
			}
			UpdateHaircut(haircutMesh);

			SetControlsInteractable(true);
		}

		private IEnumerator ChangeTexture()
		{
			SetControlsInteractable(false);
			string currentTexture = availableTextures[currentTextureIndex];
			currentTextureNameText.text = currentTexture;

			var textureRequest = avatarProvider.GetTextureAsync(currentAvatarCode, currentTexture == DEFAULT_TEXTURE_NAME ? null : currentTexture);
			yield return Await(textureRequest);


			var head = GameObject.Find(HEAD_OBJECT_NAME);
			SkinnedMeshRenderer meshRenderer = head.GetComponent<SkinnedMeshRenderer>();
			meshRenderer.material.mainTexture = textureRequest.Result;

			SetControlsInteractable(true);
		}

		private void UpdateAvailableTextures(ComputationList texturesList)
		{
			availableTextures.Clear();
			availableTextures.Add(DEFAULT_TEXTURE_NAME);
			texturesList.Values.ForEach(t => 
			{
				if (t.Name.Contains("cartoonish"))
					availableTextures.Add(t.Name);
			});
		}

		private void ConfigureParametersForStyledFace(ComputationParameters computationParameters)
		{
			computationParameters.avatarModifications = new AvatarModificationsGroup();
			computationParameters.avatarModifications.parametricEyesTexture.Value = true;
			computationParameters.avatarModifications.allowModifyNeck.Value = true;
			computationParameters.avatarModifications.addEyelidShadow.Value = true;
			computationParameters.avatarModifications.addGlare.Value = true;

			computationParameters.shapeModifications.cartoonishV03.Value = cartoonishValue;

			computationParameters.additionalTextures.AddValue("base/slightly_cartoonish_texture");
			computationParameters.additionalTextures.AddValue("plus/cartoonish_texture");
		}

		private void ConfigureParametersForHead20_HeadMobile(ComputationParameters computationParameters)
		{
			computationParameters.avatarModifications = new AvatarModificationsGroup();
			computationParameters.avatarModifications.slightlyCartoonishTexture.Value = true;
			computationParameters.avatarModifications.parametricEyesTextureV2.Value = true;
			computationParameters.avatarModifications.allowModifyNeck.Value = true;
			computationParameters.avatarModifications.enhanceLighting.Value = true;
			computationParameters.avatarModifications.removeGlasses.Value = true;
			computationParameters.avatarModifications.removeSmile.Value = true;

			computationParameters.shapeModifications.cartoonishV1.Value = cartoonishValue;
		}

		private void ConfigureParametersForHead20_BustMobile(ComputationParameters computationParameters)
		{
			computationParameters.avatarModifications = new AvatarModificationsGroup();
			computationParameters.avatarModifications.slightlyCartoonishTexture.Value = true;
			computationParameters.avatarModifications.parametricEyesTextureV2.Value = true;
			computationParameters.avatarModifications.enhanceLighting.Value = true;
			computationParameters.avatarModifications.removeGlasses.Value = true;
			computationParameters.avatarModifications.removeSmile.Value = true;
		}

		private void ConfigureParametersForHead12(ComputationParameters computationParameters)
		{
			computationParameters.avatarModifications = new AvatarModificationsGroup();
			computationParameters.avatarModifications.slightlyCartoonishTexture.Value = true;
			computationParameters.avatarModifications.parametricEyesTexture.Value = true;
			computationParameters.avatarModifications.addEyelidShadow.Value = true;
			computationParameters.avatarModifications.addGlare.Value = true;
			computationParameters.avatarModifications.enhanceLighting.Value = true;
			computationParameters.avatarModifications.removeGlasses.Value = true;
			computationParameters.avatarModifications.removeSmile.Value = true;

			computationParameters.shapeModifications.cartoonishV1.Value = cartoonishValue;
		}
		#endregion
	}
}
