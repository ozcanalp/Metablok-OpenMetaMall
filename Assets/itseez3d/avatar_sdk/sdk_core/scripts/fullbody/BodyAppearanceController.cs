/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
* You may not use this file except in compliance with an authorized license
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
* See the License for the specific language governing permissions and limitations under the License.
* Written by Itseez3D, Inc. <support@avatarsdk.com>, December 2020
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GLTF.CoroutineGLTFSceneImporter;

namespace ItSeez3D.AvatarSdk.Core
{
	public class BodyAppearanceController : MonoBehaviour
	{
		public SkinnedMeshRenderer bodyRenderer = null;

		public Texture2D mainOpaqueTexture = null;

		public bool usePBRTextures = true;

		private HaircutAppearanceController activeHaircutController = null;

		private OutfitAppearanceController activeOutfitController = null;

		private FullbodyMaterialAdjuster materialAdjuster = new FullbodyMaterialAdjuster();

		private string avatarCode = null;

		private bool isCurrentMaterialWithPBR = true;

		public event Action<string> activeHaircutChanged;

		public event Action<string> activeOutfitChanged;

		public IEnumerator Setup(string avatarCode, SkinnedMeshRenderer bodyRenderer)
		{
			this.bodyRenderer = bodyRenderer;
			this.avatarCode = avatarCode;

			yield return PrepareMaterial();
			mainOpaqueTexture = bodyRenderer.material.mainTexture as Texture2D;
		}

		public void SetBodyTexture(Texture2D bodyTexture)
		{
			materialAdjuster.UpdateBodyMainTexture(bodyRenderer.sharedMaterial, bodyTexture);
			Resources.UnloadUnusedAssets();
		}

		public void ObserveHaircutVisibilityState(HaircutAppearanceController haircutController)
		{
			haircutController.onHaircutShown += VisibleHaircutChanged;
		}

		public void ObserveOutfitVisibilityState(OutfitAppearanceController outfitController)
		{
			outfitController.onOutfitShown += VisibleOutfitChanged;
		}

		public string ActiveHaircutName
		{
			get { return activeHaircutController == null ? string.Empty : activeHaircutController.haircutName; }
		}

		public string ActiveOutfitName
		{
			get { return activeOutfitController == null ? string.Empty : activeOutfitController.outfitName; }
		}

		private IEnumerator PrepareMaterial()
		{
			isCurrentMaterialWithPBR = usePBRTextures;
			CoroutineResult<Material> coroutineResult = new CoroutineResult<Material>();
			yield return materialAdjuster.PrepareBodyMaterial(coroutineResult, avatarCode, usePBRTextures);
			bodyRenderer.sharedMaterial = coroutineResult.result;
		}

		private IEnumerator UpdateMaterial()
		{
			Texture2D currentMainTexture = bodyRenderer.sharedMaterial.mainTexture as Texture2D;
			yield return PrepareMaterial();
			SetBodyTexture(currentMainTexture);
			Resources.UnloadUnusedAssets();
		}

		private void VisibleHaircutChanged(HaircutAppearanceController haircutController, bool isActive)
		{
			if (isActive)
			{
				if (activeHaircutController != haircutController)
				{
					if (activeHaircutController != null)
						activeHaircutController.gameObject.SetActive(false);
					activeHaircutController = haircutController;
				}
			}
			else
			{
				if (activeHaircutController == haircutController)
					activeHaircutController = null;
			}

			activeHaircutChanged?.Invoke(ActiveHaircutName);
		}

		private void VisibleOutfitChanged(OutfitAppearanceController outfitController, bool isActive)
		{
			if (isActive)
			{
				if (activeOutfitController != outfitController)
				{
					if (activeOutfitController != null)
						activeOutfitController.gameObject.SetActive(false);
					activeOutfitController = outfitController;

					bodyRenderer.sharedMaterial.mainTexture = activeOutfitController.bodyTransparentTexture != null ? activeOutfitController.bodyTransparentTexture : mainOpaqueTexture;
				}
			}
			else
			{
				if (activeOutfitController == outfitController)
				{
					activeOutfitController = null;
					bodyRenderer.sharedMaterial.mainTexture = mainOpaqueTexture;
				}
			}
			activeOutfitChanged?.Invoke(ActiveOutfitName);
		}

		private void Update()
		{
			if (isCurrentMaterialWithPBR != usePBRTextures)
				StartCoroutine(UpdateMaterial());
		}
	}
}
