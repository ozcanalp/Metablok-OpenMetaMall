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
	public class HaircutAppearanceController : MonoBehaviour
	{
		public SkinnedMeshRenderer haircutMeshRenderer = null;

		public string haircutName = null;

		[HideInInspector]
		public Color haircutColor = Color.clear; 

		private string haircutDirectory = null;

		private FullbodyMaterialAdjuster materialAdjuster = new FullbodyMaterialAdjuster();

		private bool clearMaterialWhenInactive = true;

		private bool isMaterialPrepared = false;

		public event Action<HaircutAppearanceController, bool> onHaircutShown;

		public IEnumerator Setup(string haircutDirectory, string haircutName, SkinnedMeshRenderer haircutMeshRenderer, bool clearMaterialWhenInactive, bool prepareMaterialBeforehand)
		{
			this.haircutDirectory = haircutDirectory;
			this.haircutName = haircutName;
			this.haircutMeshRenderer = haircutMeshRenderer;
			this.clearMaterialWhenInactive = clearMaterialWhenInactive;

			if (prepareMaterialBeforehand)
				yield return PrepareMaterial();
		}

		public IEnumerator PrepareMaterial()
		{
			CoroutineResult<Material> coroutineResult = new CoroutineResult<Material>();
			yield return materialAdjuster.PrepareHairMaterial(coroutineResult, haircutDirectory, haircutName);
			haircutMeshRenderer.sharedMaterial = coroutineResult.result;

			if (haircutColor != Color.clear)
				ImageUtils.RecolorTexture(haircutMeshRenderer.sharedMaterial.mainTexture as Texture2D, haircutColor);

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
				haircutMeshRenderer.materials = new Material[] { };
			}
			onHaircutShown?.Invoke(this, false);
			Resources.UnloadUnusedAssets();
		}

		private IEnumerator OnEnableRoutine()
		{
			if (!isMaterialPrepared)
				yield return PrepareMaterial();
			onHaircutShown?.Invoke(this, true);
		}
	}
}
