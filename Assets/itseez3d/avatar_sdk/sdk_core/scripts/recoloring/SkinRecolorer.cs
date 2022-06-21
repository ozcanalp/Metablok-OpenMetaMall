/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
* You may not use this file except in compliance with an authorized license
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
* See the License for the specific language governing permissions and limitations under the License.
* Written by Itseez3D, Inc. <support@avatarsdk.com>, September 2021
*/

using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace ItSeez3D.AvatarSdk.Core
{
	public class SkinRecolorer : TextureRecolorer
	{
		private readonly string skinMaskTexturePath = "textures/mask_skin_head_mobile";

		public SkinRecolorer()
		{
			defaultMaterialPath = "recoloring_materials/SkinRecoloringMaterial";
		}

		public override void SetDefaultColor(Color color)
		{
			defaultColor = color;
			if (recoloringByMaterial != null)
			{
				recoloringByMaterial.SetVector("_SkinColor", defaultColor);
				recoloringByMaterial.SetVector("_TargetSkinColor", defaultColor);
			}
		}

		public override void DetectDefaultColor(string avatarCode)
		{
			this.avatarCode = avatarCode;

			if (textureToRecolor != null)
			{
				if (!string.IsNullOrEmpty(avatarCode))
					defaultColor = CoreTools.GetSkinColor(avatarCode);

				if (defaultColor == Color.white)
				{
					Texture2D skinMaskTexture = Resources.Load<Texture2D>(skinMaskTexturePath);
					if (skinMaskTexture != null)
					{
						defaultColor = CoreTools.CalculateAverageColorByMask(textureToRecolor as Texture2D, skinMaskTexture);
						Resources.UnloadAsset(skinMaskTexture);
					}
				}

				SetDefaultColor(defaultColor);
			}
		}

		protected override void SetMaterialColor(Color color)
		{
			if (recoloringByMaterial != null)
				recoloringByMaterial.SetVector("_TargetSkinColor", color);
		}
	}

#if UNITY_EDITOR
	[UnityEditor.CustomEditor(typeof(SkinRecolorer))]
	public class SkinRecoloringEditor : TextureRecoloringEditor
	{
	}
#endif
}
