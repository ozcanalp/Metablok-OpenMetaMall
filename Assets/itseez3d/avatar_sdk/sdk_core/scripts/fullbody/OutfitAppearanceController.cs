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
	public class OutfitAppearanceController : MonoBehaviour
	{
		public SkinnedMeshRenderer outfitMeshRenderer = null;

		public Texture2D bodyTransparentTexture = null;

		public bool usePBRTextures = true;

		public string outfitName = null;

		private Texture2D bodyOpaqueTexture = null;

		private string outfitDirectory = null;

		private FullbodyMaterialAdjuster materialAdjuster = new FullbodyMaterialAdjuster();

		private bool clearMaterialWhenInactive = true;

		private bool isCurrentMaterialWithPBR = true;

		private bool isMaterialPrepared = false;

		public event Action<OutfitAppearanceController, bool> onOutfitShown;

		public IEnumerator Setup(string outfitDirectory, string outfitName, 
			SkinnedMeshRenderer outfitMeshRenderer, Texture2D bodyOpaqueTexture, bool clearMaterialWhenInactive, bool prepareMaterialBeforehand)
		{
			this.outfitDirectory = outfitDirectory;
			this.outfitName = outfitName;
			this.outfitMeshRenderer = outfitMeshRenderer;
			this.bodyOpaqueTexture = bodyOpaqueTexture;
			this.clearMaterialWhenInactive = clearMaterialWhenInactive;

			if (prepareMaterialBeforehand)
				yield return PrepareMaterial();
		}

		public IEnumerator PrepareMaterial()
		{
			isCurrentMaterialWithPBR = usePBRTextures;
			CoroutineResult<Material> outfitMaterialResult = new CoroutineResult<Material>();
			yield return materialAdjuster.PrepareOutfitMaterial(outfitMaterialResult, outfitDirectory, outfitName, usePBRTextures);
			outfitMeshRenderer.sharedMaterial = outfitMaterialResult.result;

			if (bodyTransparentTexture == null)
			{
				CoroutineResult<Texture2D> textureResult = new CoroutineResult<Texture2D>();
				yield return materialAdjuster.PrepareTransparentBodyTextureForOutfit(textureResult, bodyOpaqueTexture, outfitDirectory, outfitName);
				bodyTransparentTexture = textureResult.result;
			}

			isMaterialPrepared = true;
		}

		private void OnEnable()
		{
			StartCoroutine(OnEnableRoutine());
		}

		private void OnDisable()
		{
			if (clearMaterialWhenInactive)
			{
				isMaterialPrepared = false;
				outfitMeshRenderer.materials = new Material[] { };
				if (bodyTransparentTexture != null)
				{
					DestroyImmediate(bodyTransparentTexture);
					bodyTransparentTexture = null;
				}
			}
			onOutfitShown?.Invoke(this, false);
			Resources.UnloadUnusedAssets();
		}

		private void Update()
		{
			if (isCurrentMaterialWithPBR != usePBRTextures)
				StartCoroutine(PrepareMaterial());
		}

		private IEnumerator OnEnableRoutine()
		{
			if (!isMaterialPrepared)
				yield return PrepareMaterial();
			onOutfitShown?.Invoke(this, true);
		}
	}
}
