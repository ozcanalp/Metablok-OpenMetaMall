/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
* You may not use this file except in compliance with an authorized license
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
* See the License for the specific language governing permissions and limitations under the License.
* Written by Itseez3D, Inc. <support@avatarsdk.com>, November 2020
*/

using GLTF;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityGLTF;
using static GLTF.CoroutineGLTFSceneImporter;

namespace ItSeez3D.AvatarSdk.Core
{
	/// <summary>
	/// Class to load an fullbody avatar model in GLTF format and to add it to the unity scene.
	/// </summary>
	public class FullbodyAvatarLoader
	{
		/// <summary>
		/// Parent GameObject for the loaded avatar
		/// </summary>
		public GameObject AvatarGameObject { get; set; }

		/// <summary>
		/// Render body with PBR textures (metallness/rougness maps, normal map)
		/// </summary>
		public bool UseBodyPBRTextures = true;

		/// <summary>
		/// Render outfits with PBR textures (metallness/rougness maps, normal map)
		/// </summary>
		public bool UseOutfitsPBRTextures = true;

		/// <summary>
		/// Haircuts and outfits textures take a large number of graphics memory.
		/// The memory usage can be reduced by unloading textures and materials for inactive objects.
		/// </summary>
		public bool KeepLowGraphicsMemoryUsage = true;

		/// <summary>
		/// Set "updateWhenOffscreen" property for the created SkinnedMeshRenderers
		/// </summary>
		public bool UpdateWhenOffscreen = false;

		/// <summary>
		/// Recolor haircuts textures, if required
		/// </summary>
		public Color haircutColor = Color.clear;

		public BodyAppearanceController bodyAppearanceController = null;
		public Dictionary<string, HaircutAppearanceController> haircuts = new Dictionary<string, HaircutAppearanceController>();
		public Dictionary<string, OutfitAppearanceController> outfits = new Dictionary<string, OutfitAppearanceController>();

		private string currentAvatarCode = string.Empty;

		private IFullbodyAvatarProvider fullbodyAvatarProvider = null;
		private IFullbodyPersistentStorage fullbodyPersistentStorage = null;

		public FullbodyAvatarLoader(IFullbodyAvatarProvider fullbodyAvatarProvider)
		{
			this.fullbodyAvatarProvider = fullbodyAvatarProvider;
			fullbodyPersistentStorage = AvatarSdkMgr.FullbodyStorage();
		}

		/// <summary>
		/// Load the model on the scene
		/// </summary>
		/// <param name="avatarCode">Full body avatar code</param>
		/// <param name="avatarGameObject">Parent GameObject for the avatar</param>
		public IEnumerator LoadAvatarAsync(string avatarCode)
		{
			MeasureTime avatarLoadingMeasure = new MeasureTime("Avatar loading");
			currentAvatarCode = avatarCode;

			if (AvatarGameObject == null)
				AvatarGameObject = new GameObject("Fullbody Avatar");

			string meshFilename = fullbodyPersistentStorage.GetAvatarFile(currentAvatarCode, FullbodyAvatarFileType.Model);
			string meshDirectory = Path.GetDirectoryName(meshFilename);
			List<SkinnedMeshRenderer> loadedMeshes = new List<SkinnedMeshRenderer>();
			yield return LoadGltfModelAsync(meshFilename, loadedMeshes);

			SkinnedMeshRenderer bodyMeshRenderer = loadedMeshes.FirstOrDefault(r => r.gameObject.name == "mesh");
			if (bodyMeshRenderer == null)
				throw new Exception("Avatar mesh object isn't found in GLTF");
			bodyAppearanceController = bodyMeshRenderer.gameObject.AddComponent<BodyAppearanceController>();
			bodyAppearanceController.usePBRTextures = UseOutfitsPBRTextures;
			yield return bodyAppearanceController.Setup(currentAvatarCode, bodyMeshRenderer);

			foreach (SkinnedMeshRenderer renderer in loadedMeshes)
			{
				if (renderer.gameObject.name.StartsWith("haircut"))
				{
					string haircutName = ExtractHaircutName(renderer.gameObject.name);
					yield return ConfigureHaircutRenderer(meshDirectory, haircutName, renderer);
				}
				else if (renderer.gameObject.name.StartsWith("outfit"))
				{
					string outfitName = renderer.gameObject.name;
					yield return ConfigureOutfitRenderer(meshDirectory, outfitName, renderer);
				}
			}

			UpdateAvailableHaircutsAndOutfits();
			avatarLoadingMeasure.Stop();
		}

		/// <summary>
		/// Returns GameObject of the body mesh
		/// </summary>
		public GameObject GetBodyMeshObject()
		{
			return bodyAppearanceController.bodyRenderer.gameObject;
		}

		#region haircuts handling
		/// <summary>
		/// Returns the list of available haircuts
		/// </summary>
		public List<string> GetHaircuts()
		{
			return haircuts.Keys.ToList();
		}

		/// <summary>
		/// Returns name of the displayed haircut 
		/// </summary>
		public string GetCurrentHaircutName()
		{
			return bodyAppearanceController.ActiveHaircutName;
		}

		/// <summary>
		/// Loads the haircut mesh from the disk and adds it to the scene
		/// </summary>
		public AsyncRequest LoadHaircutAsync(string haircutName)
		{
			var request = new AsyncRequest(AvatarSdkMgr.Str(Strings.LoadingHaircut));
			AvatarSdkMgr.SpawnCoroutine(LoadHaircutFunc(haircutName, request));
			return request;
		}

		/// <summary>
		/// Show haircut on the scene
		/// </summary>
		public AsyncRequest ShowHaircutAsync(string haircutName)
		{
			var request = new AsyncRequest(AvatarSdkMgr.Str(Strings.ShowingHaircut));
			AvatarSdkMgr.SpawnCoroutine(ShowHaircutFunc(haircutName, request));
			return request;
		}

		/// <summary>
		/// Makes all haircuts objects inactive
		/// </summary>
		public void HideAllHaircuts()
		{
			foreach (var haircut in haircuts)
			{
				if (haircut.Value != null)
					haircut.Value.gameObject.SetActive(false);
			}
		}

		/// <summary>
		/// Updates the lists of available haircuts and outfits
		/// </summary>
		public void UpdateAvailableHaircutsAndOutfits()
		{
			List<string> haircutsInDiscreteFiles = fullbodyAvatarProvider.GetHaircutsInDiscreteFiles(currentAvatarCode);
			foreach (string haircutName in haircutsInDiscreteFiles)
				if (!haircuts.ContainsKey(haircutName))
					haircuts.Add(haircutName, null);

			List<string> outfitsInDiscreteFiles = fullbodyAvatarProvider.GetOutfitsInDiscreteFiles(currentAvatarCode);
			foreach (string outfitName in outfitsInDiscreteFiles)
				if (!outfits.ContainsKey(outfitName))
					outfits.Add(outfitName, null);
		}

		/// <summary>
		/// Makes the haircut object active.
		/// It loads the haircut mesh from the disk if the SkinnedMeshRenderer doesn't exist yet.
		/// </summary>
		private IEnumerator ShowHaircutFunc(string haircutName, AsyncRequest request)
		{
			MeasureTime haircutShowingMeasure = new MeasureTime(string.Format("{0} showing", haircutName));
			if (!string.IsNullOrEmpty(haircutName) && haircuts.ContainsKey(haircutName))
			{
				if (haircuts[haircutName] == null)
				{
					var loadHaircutRequest = LoadHaircutAsync(haircutName);
					yield return request.AwaitSubrequest(loadHaircutRequest, 0.99f);
					if (request.IsError)
						yield break;
				}

				SkinnedMeshRenderer haircutRenderer = haircuts[haircutName].haircutMeshRenderer;
				if (haircutRenderer != null)
				{
					yield return haircuts[haircutName].PrepareMaterial();
					haircutRenderer.gameObject.SetActive(true);
				}
			}

			request.Progress = 1.0f;
			request.IsDone = true;
			haircutShowingMeasure.Stop(true);
		}

		private string ExtractHaircutName(string haircutObjectName)
		{
			string prefixPattern = "haircut_";
			int pos = haircutObjectName.IndexOf(prefixPattern);
			return haircutObjectName.Substring(pos + prefixPattern.Length);
		}

		private IEnumerator LoadHaircutFunc(string haircutName, AsyncRequest request)
		{
			var gettingHaircutFilesRequest = fullbodyAvatarProvider.RetrieveHaircutModelFromCloudAsync(currentAvatarCode, haircutName);
			yield return request.AwaitSubrequest(gettingHaircutFilesRequest, 0.9f);
			if (request.IsError)
				yield break;

			request.State = "Reading model file";
			string haircutModelFile = fullbodyPersistentStorage.GetHaircutFile(currentAvatarCode, haircutName, FullbodyHaircutFileType.Model);
			List<SkinnedMeshRenderer> loadedMeshes = new List<SkinnedMeshRenderer>();
			yield return LoadGltfModelAsync(haircutModelFile, loadedMeshes);

			if (loadedMeshes.Count != 1)
			{
				Debug.LogWarningFormat("Unexpected number of meshes in haircut gltf: {0}", loadedMeshes.Count);
			}
			else
			{
				SkinnedMeshRenderer haircutMeshRenderer = loadedMeshes[0];
				string haircutObjectName = string.Format("haircut_{0}", haircutName);
				if (haircutMeshRenderer.gameObject.name != haircutObjectName)
					haircutMeshRenderer.gameObject.name = haircutObjectName;

				yield return ConfigureHaircutRenderer(Path.GetDirectoryName(haircutModelFile), haircutName, haircutMeshRenderer);
			}

			request.Progress = 1.0f;
			request.IsDone = true;
		}

		private IEnumerator ConfigureHaircutRenderer(string haircutDir, string haircutName, SkinnedMeshRenderer renderer)
		{
			renderer.gameObject.SetActive(false);
			HaircutAppearanceController haircutApperanceController = renderer.gameObject.AddComponent<HaircutAppearanceController>();
			haircutApperanceController.haircutColor = haircutColor;
			yield return haircutApperanceController.Setup(haircutDir, haircutName, renderer, KeepLowGraphicsMemoryUsage, !KeepLowGraphicsMemoryUsage);
			bodyAppearanceController.ObserveHaircutVisibilityState(haircutApperanceController);
			haircuts[haircutName] = haircutApperanceController;
		}
		#endregion haircuts handling

		#region outfits handling
		/// <summary>
		/// Returns the list of available outfits
		/// </summary>
		public List<string> GetOutfits()
		{
			return outfits.Keys.ToList();
		}

		/// <summary>
		/// Returns name of the displayed outfit 
		/// </summary>
		public string GetCurrentOutfitName()
		{
			return bodyAppearanceController.ActiveOutfitName;
		}

		/// <summary>
		/// Loads the outfit mesh from the disk and adds it to the scene
		/// </summary>
		public AsyncRequest LoadOutfitAsync(string outfitName)
		{
			var request = new AsyncRequest(AvatarSdkMgr.Str(Strings.LoadingOutfit));
			AvatarSdkMgr.SpawnCoroutine(LoadOutfitFunc(outfitName, request));
			return request;
		}

		/// <summary>
		/// Show outfit on the scene
		/// </summary>
		public AsyncRequest ShowOutfitAsync(string outfitName)
		{
			var request = new AsyncRequest(AvatarSdkMgr.Str(Strings.ShowingOutfit));
			AvatarSdkMgr.SpawnCoroutine(ShowOutfitFunc(outfitName, request));
			return request;
		}

		/// <summary>
		/// Makes all outfits objects inactive
		/// </summary>
		public void HideAllOutfits()
		{
			foreach (var outfit in outfits)
			{
				if (outfit.Value != null)
					outfit.Value.gameObject.SetActive(false);
			}
		}

		/// <summary>
		/// Makes the outfit object active.
		/// It loads the outfit mesh from the disk if the SkinnedMeshRenderer doesn't exist yet.
		/// </summary>
		private IEnumerator ShowOutfitFunc(string outfitName, AsyncRequest request)
		{
			MeasureTime outfitShowingMeasure = new MeasureTime(string.Format("{0} showing", outfitName));
			if (!string.IsNullOrEmpty(outfitName) && outfits.ContainsKey(outfitName))
			{
				if (outfits[outfitName] == null)
				{
					var loadOutfitRequest = LoadOutfitAsync(outfitName);
					yield return request.AwaitSubrequest(loadOutfitRequest, 0.99f);
					if (request.IsError)
						yield break;
				}

				SkinnedMeshRenderer outfitRenderer = outfits[outfitName].outfitMeshRenderer;
				if (outfitRenderer != null)
				{
					yield return outfits[outfitName].PrepareMaterial();
					outfitRenderer.gameObject.SetActive(true);
				}
			}

			request.Progress = 1.0f;
			request.IsDone = true;
			outfitShowingMeasure.Stop(true);
		}

		private IEnumerator LoadOutfitFunc(string outfitName, AsyncRequest request)
		{
			var gettingOutfitRequest = fullbodyAvatarProvider.RetrieveOutfitModelFromCloudAsync(currentAvatarCode, outfitName);
			yield return request.AwaitSubrequest(gettingOutfitRequest, 0.9f);
			if (request.IsError)
				yield break;

			request.State = "Reading model file";
			string outfitModelFile = fullbodyPersistentStorage.GetOutfitFile(currentAvatarCode, outfitName, OutfitFileType.Model);
			List<SkinnedMeshRenderer> loadedMeshes = new List<SkinnedMeshRenderer>();
			yield return LoadGltfModelAsync(outfitModelFile, loadedMeshes);

			if (loadedMeshes.Count != 1)
			{
				Debug.LogWarningFormat("Unexpected number of meshes in outfit gltf: {0}", loadedMeshes.Count);
			}
			else
			{
				SkinnedMeshRenderer outfitMeshRenderer = loadedMeshes[0];
				string outfitObjectName = string.Format("{0}", outfitName);
				if (outfitMeshRenderer.gameObject.name != outfitObjectName)
					outfitMeshRenderer.gameObject.name = outfitObjectName;

				yield return ConfigureOutfitRenderer(Path.GetDirectoryName(outfitModelFile), outfitName, outfitMeshRenderer);
			}

			request.Progress = 1.0f;
			request.IsDone = true;
		}

		private IEnumerator ConfigureOutfitRenderer(string outfitDir, string outfitName, SkinnedMeshRenderer renderer)
		{
			renderer.gameObject.SetActive(false);
			OutfitAppearanceController outfitApperanceController = renderer.gameObject.AddComponent<OutfitAppearanceController>();
			outfitApperanceController.usePBRTextures = UseOutfitsPBRTextures;
			yield return outfitApperanceController.Setup(outfitDir, outfitName, renderer, bodyAppearanceController.mainOpaqueTexture, 
				KeepLowGraphicsMemoryUsage, !KeepLowGraphicsMemoryUsage);
			bodyAppearanceController.ObserveOutfitVisibilityState(outfitApperanceController);
			outfits[outfitName] = outfitApperanceController;
		}
		#endregion outfits handling

		/// <summary>
		/// Returns a list of available blendshapes
		/// </summary>
		public List<string> GetBlendshapes()
		{
			List<string> blendshapes = new List<string>();
			for (int i = 0; i < bodyAppearanceController.bodyRenderer.sharedMesh.blendShapeCount; i++)
				blendshapes.Add(bodyAppearanceController.bodyRenderer.sharedMesh.GetBlendShapeName(i));
			return blendshapes;
		}

		/// <summary>
		/// Sets all blendshapes weights to zero
		/// </summary>
		public void ClearBlendshapesWeights()
		{
			for (int i = 0; i < bodyAppearanceController.bodyRenderer.sharedMesh.blendShapeCount; i++)
				bodyAppearanceController.bodyRenderer.SetBlendShapeWeight(i, 0.0f);
		}

		/// <summary>
		/// Sets the weight for the blendshapes with the provided index
		/// </summary>
		public void SetBlendshapeWeight(int blendshapeIdx, float weight)
		{
			bodyAppearanceController.bodyRenderer.SetBlendShapeWeight(blendshapeIdx, weight);
		}

		private IEnumerator LoadGltfModelAsync(string gltfModelFilename, List<SkinnedMeshRenderer> loadedMeshes)
		{
			var importOptions = new ImportOptions();
			importOptions.DataLoader = new UnityGLTF.Loader.FileLoader(URIHelper.GetDirectoryName(gltfModelFilename));

			CoroutineGLTFSceneImporter sceneImporter = null;
			try
			{
				sceneImporter = new CoroutineGLTFSceneImporter(Path.GetFileName(gltfModelFilename), importOptions);
				sceneImporter.SkipTexturesLoading = KeepLowGraphicsMemoryUsage;
				sceneImporter.UpdateWhenOffscreenMeshRenderer = UpdateWhenOffscreen;

				yield return sceneImporter.LoadSceneAsync(AvatarGameObject);
				foreach (SkinnedMeshRenderer meshRenderer in sceneImporter.lastLoadedMeshes)
					loadedMeshes.Add(meshRenderer);
			}
			finally
			{
				if (importOptions.DataLoader != null)
				{
					sceneImporter?.Dispose();
					sceneImporter = null;
					importOptions.DataLoader = null;
				}
			}
		}
	}
}
