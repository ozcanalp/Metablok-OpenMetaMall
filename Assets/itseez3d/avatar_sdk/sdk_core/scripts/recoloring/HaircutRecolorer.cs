/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
* You may not use this file except in compliance with an authorized license
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
* See the License for the specific language governing permissions and limitations under the License.
* Written by Itseez3D, Inc. <support@avatarsdk.com>, April 2017
*/

using UnityEngine;

namespace ItSeez3D.AvatarSdk.Core
{
	public class HaircutRecolorer : TextureRecolorer
	{
		protected Color averageColor = Color.black;

		public HaircutRecolorer()
		{
			disableRecoloringWhenResetting = false;
			defaultMaterialPath = "recoloring_materials/HaircutRecoloringMaterial";
		}

		public void SetAverageColor(Color avgColor)
		{
			averageColor = avgColor;
			SetDefaultColor(defaultColor);
		}

		public override void SetDefaultColor(Color color)
		{
			defaultColor = color;
			if (recoloringByMaterial != null)
			{
				Vector4 tint = CoreTools.CalculateTint(defaultColor, averageColor);
				recoloringByMaterial.SetVector("_ColorTarget", defaultColor);
				recoloringByMaterial.SetVector("_ColorTint", tint);
				recoloringByMaterial.SetFloat("_TintCoeff", 0.8f);
			}
		}

		public override void DetectDefaultColor(string avatarCode)
		{
			this.avatarCode = avatarCode;

			if (textureToRecolor != null)
			{
				Texture2D tex = textureToRecolor as Texture2D;
				if (tex != null)
					averageColor = CoreTools.CalculateAverageColor(tex);
				Color predictedHairColor = CoreTools.GetAvatarPredictedHairColor(avatarCode);
				defaultColor = predictedHairColor == Color.clear ? averageColor : predictedHairColor;
				SetDefaultColor(defaultColor);
			}
		}

		protected override void SetMaterialColor(Color color)
		{
			if (recoloringByMaterial != null)
			{
				Vector4 tint = CoreTools.CalculateTint(color, averageColor);
				recoloringByMaterial.SetVector("_ColorTarget", color);
				recoloringByMaterial.SetVector("_ColorTint", tint);
			}
		}

		protected override Material FindMaterialToRecolor()
		{
			Renderer meshRenderer = GetComponentInChildren<Renderer>();
			if (meshRenderer != null)
			{
				foreach(Material m in meshRenderer.sharedMaterials)
				{
					if (m.name.Contains("avatar_sdk"))
						return m;
				}
				return meshRenderer.sharedMaterial;
			}
			return null;
		}
	}

#if UNITY_EDITOR
	[UnityEditor.CustomEditor(typeof(HaircutRecolorer))]
	public class HaircutRecoloringEditor : TextureRecoloringEditor
	{
	}
#endif
}
