/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
* You may not use this file except in compliance with an authorized license
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
* See the License for the specific language governing permissions and limitations under the License.
* Written by Itseez3D, Inc. <support@avatarsdk.com>, April 2017
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ItSeez3D.AvatarSdk.Core
{
	public enum AvatarShaderType
	{
		UnlitShader,
		LitShader
	}

	public enum MaterialTemplateType
	{
		HeadStandard,
		HeadUnlit,
		Body,
		Outfit,
		HaircutOpaque,
		HaircutAlphaSamplingUnlit,
		HaircutTransparentUnlit,
		HaircutAlphaSamplingLit,
		HaircutTransparentLit
	}

	public static class MaterialUtils
	{
		class BestMatchMaterials
		{
			public MaterialTemplateType unlitMaterial;
			public MaterialTemplateType litMaterial;

			public BestMatchMaterials(MaterialTemplateType unlitMaterial, MaterialTemplateType litMaterial)
			{
				this.unlitMaterial = unlitMaterial;
				this.litMaterial = litMaterial;
			}
		}


		private static Dictionary<MaterialTemplateType, string> materialTemplateNames = null;

		private static Dictionary<string, BestMatchMaterials> haircustBestMatchMaterials = null;

		//These haircuts should be rendered with enabled culling in lit shader. 
		//Because some polygons are inverted 
		private static List<string> haircutsWithCulling = new List<string>()
		{
			"female_NewSea_J096f",
			"female_NewSea_J086f",
			"female_NewSea_J123f",
			"male_NewSea_J082m",
			"male_NewSea_J003m",
			"ponytail_with_bangs",
			"long_disheveled",
			"long_wavy",
			"short_disheveled",
			"mid_length_straight"
		};

		static MaterialUtils()
		{
			string materialSuffix = "";
			string materialDirectory = "";
			string separator = "";

			if (RenderingPipeline.GetCurrent() == RenderingPipelineType.URP)
			{
				materialSuffix = "_urp";
				separator = "/";
				materialDirectory = "urp";
			}

			materialTemplateNames = new Dictionary<MaterialTemplateType, string>()
			{
				{ MaterialTemplateType.HeadStandard, $"avatar_materials/{materialDirectory}{separator}avatar_sdk_template_head{materialSuffix}" },
				{ MaterialTemplateType.HeadUnlit, $"avatar_materials/{materialDirectory}{separator}avatar_sdk_template_head_unlit{materialSuffix}" },
				{ MaterialTemplateType.Body, $"avatar_materials/{materialDirectory}{separator}avatar_sdk_template_body{materialSuffix}" },
				{ MaterialTemplateType.Outfit, $"avatar_materials/{materialDirectory}{separator}avatar_sdk_template_outfit{materialSuffix}" },
				{ MaterialTemplateType.HaircutOpaque, $"avatar_materials/{materialDirectory}{separator}avatar_sdk_template_haircut_opaque{materialSuffix}" },
				{ MaterialTemplateType.HaircutAlphaSamplingUnlit, $"avatar_materials/{materialDirectory}{separator}avatar_sdk_template_haircut_unlit_alpha_sampling{materialSuffix}" },
				{ MaterialTemplateType.HaircutTransparentUnlit, $"avatar_materials/{materialDirectory}{separator}avatar_sdk_template_haircut_unlit_transparent{materialSuffix}" },
				{ MaterialTemplateType.HaircutAlphaSamplingLit, $"avatar_materials/{materialDirectory}{separator}avatar_sdk_template_haircut_lit_alpha_sampling{materialSuffix}" },
				{ MaterialTemplateType.HaircutTransparentLit, $"avatar_materials/{materialDirectory}{separator}avatar_sdk_template_haircut_lit_transparent{materialSuffix}" }
			};

			haircustBestMatchMaterials = new Dictionary<string, BestMatchMaterials>()
			{
				{ "balding", new BestMatchMaterials(MaterialTemplateType.HaircutTransparentUnlit, MaterialTemplateType.HaircutTransparentLit) },
				{ "bob_parted", new BestMatchMaterials(MaterialTemplateType.HaircutTransparentUnlit, MaterialTemplateType.HaircutTransparentLit) },
				{ "facegen_balding", new BestMatchMaterials(MaterialTemplateType.HaircutTransparentUnlit, MaterialTemplateType.HaircutTransparentLit) },
				{ "facegen_bob_parted", new BestMatchMaterials(MaterialTemplateType.HaircutTransparentUnlit, MaterialTemplateType.HaircutTransparentLit) },
				{ "generated", new BestMatchMaterials(MaterialTemplateType.HaircutTransparentUnlit, MaterialTemplateType.HaircutTransparentLit) },

				{ "corkscrew_curls", new BestMatchMaterials(MaterialTemplateType.HaircutAlphaSamplingUnlit, MaterialTemplateType.HaircutAlphaSamplingLit) },
				{ "facegen_mid_length_messy", new BestMatchMaterials(MaterialTemplateType.HaircutAlphaSamplingUnlit, MaterialTemplateType.HaircutAlphaSamplingLit)},
				{ "facegen_pigtails", new BestMatchMaterials(MaterialTemplateType.HaircutAlphaSamplingUnlit, MaterialTemplateType.HaircutAlphaSamplingLit)},
				{ "facegen_short", new BestMatchMaterials(MaterialTemplateType.HaircutAlphaSamplingUnlit, MaterialTemplateType.HaircutAlphaSamplingLit)},
				{ "facegen_short_curls", new BestMatchMaterials(MaterialTemplateType.HaircutAlphaSamplingUnlit, MaterialTemplateType.HaircutAlphaSamplingLit)},
				{ "facegen_short_parted", new BestMatchMaterials(MaterialTemplateType.HaircutAlphaSamplingUnlit, MaterialTemplateType.HaircutAlphaSamplingLit)},
				{ "facegen_very_long", new BestMatchMaterials(MaterialTemplateType.HaircutAlphaSamplingUnlit, MaterialTemplateType.HaircutAlphaSamplingLit)},
				{ "female_NewSea_J086f", new BestMatchMaterials(MaterialTemplateType.HaircutAlphaSamplingUnlit, MaterialTemplateType.HaircutAlphaSamplingLit)},
				{ "female_NewSea_J096f", new BestMatchMaterials(MaterialTemplateType.HaircutAlphaSamplingUnlit, MaterialTemplateType.HaircutAlphaSamplingLit)},
				{ "female_NewSea_J123f", new BestMatchMaterials(MaterialTemplateType.HaircutAlphaSamplingUnlit, MaterialTemplateType.HaircutAlphaSamplingLit)},
				{ "long_crimped", new BestMatchMaterials(MaterialTemplateType.HaircutAlphaSamplingUnlit, MaterialTemplateType.HaircutAlphaSamplingLit) },
				{ "long_disheveled", new BestMatchMaterials(MaterialTemplateType.HaircutAlphaSamplingUnlit, MaterialTemplateType.HaircutAlphaSamplingLit) },
				{ "long_wavy", new BestMatchMaterials(MaterialTemplateType.HaircutAlphaSamplingUnlit, MaterialTemplateType.HaircutAlphaSamplingLit) },
				{ "male_makehuman_short02", new BestMatchMaterials(MaterialTemplateType.HaircutAlphaSamplingUnlit, MaterialTemplateType.HaircutAlphaSamplingLit) },
				{ "male_NewSea_J003m", new BestMatchMaterials(MaterialTemplateType.HaircutAlphaSamplingUnlit, MaterialTemplateType.HaircutAlphaSamplingLit) },
				{ "male_NewSea_J082m", new BestMatchMaterials(MaterialTemplateType.HaircutAlphaSamplingUnlit, MaterialTemplateType.HaircutAlphaSamplingLit)},
				{ "mid_length_ruffled", new BestMatchMaterials(MaterialTemplateType.HaircutAlphaSamplingUnlit, MaterialTemplateType.HaircutAlphaSamplingLit) },
				{ "mid_length_straight", new BestMatchMaterials(MaterialTemplateType.HaircutAlphaSamplingUnlit, MaterialTemplateType.HaircutAlphaSamplingLit) },
				{ "mid_length_straight2", new BestMatchMaterials(MaterialTemplateType.HaircutAlphaSamplingUnlit, MaterialTemplateType.HaircutAlphaSamplingLit) },
				{ "mid_length_wispy", new BestMatchMaterials(MaterialTemplateType.HaircutAlphaSamplingUnlit, MaterialTemplateType.HaircutAlphaSamplingLit) },
				{ "ponytail_with_bangs", new BestMatchMaterials(MaterialTemplateType.HaircutAlphaSamplingUnlit, MaterialTemplateType.HaircutAlphaSamplingLit) },
				{ "roman", new BestMatchMaterials(MaterialTemplateType.HaircutAlphaSamplingUnlit, MaterialTemplateType.HaircutAlphaSamplingLit) },
				{ "rasta", new BestMatchMaterials(MaterialTemplateType.HaircutAlphaSamplingUnlit, MaterialTemplateType.HaircutAlphaSamplingLit) },
				{ "short_curls", new BestMatchMaterials(MaterialTemplateType.HaircutAlphaSamplingUnlit, MaterialTemplateType.HaircutAlphaSamplingLit) },
				{ "short_disheveled", new BestMatchMaterials(MaterialTemplateType.HaircutAlphaSamplingUnlit, MaterialTemplateType.HaircutAlphaSamplingLit) },
				{ "short_parted", new BestMatchMaterials(MaterialTemplateType.HaircutAlphaSamplingUnlit, MaterialTemplateType.HaircutAlphaSamplingLit) },
				{ "short_simple", new BestMatchMaterials(MaterialTemplateType.HaircutAlphaSamplingUnlit, MaterialTemplateType.HaircutAlphaSamplingLit) },
				{ "short_slick", new BestMatchMaterials(MaterialTemplateType.HaircutAlphaSamplingUnlit, MaterialTemplateType.HaircutAlphaSamplingLit) },
				{ "shoulder_length", new BestMatchMaterials(MaterialTemplateType.HaircutAlphaSamplingUnlit, MaterialTemplateType.HaircutAlphaSamplingLit) },
				{ "very_long", new BestMatchMaterials(MaterialTemplateType.HaircutAlphaSamplingUnlit, MaterialTemplateType.HaircutAlphaSamplingLit) },
				{ "wavy_bob", new BestMatchMaterials(MaterialTemplateType.HaircutAlphaSamplingUnlit, MaterialTemplateType.HaircutAlphaSamplingLit) },
			};

			if (RenderingPipeline.GetCurrent() == RenderingPipelineType.BuiltIn)
			{
				// We need to use a custom shader because the Standard doesn't support culling
				haircustBestMatchMaterials.Add("facegen_long_bob", new BestMatchMaterials(MaterialTemplateType.HaircutAlphaSamplingUnlit, MaterialTemplateType.HaircutAlphaSamplingLit));
				haircustBestMatchMaterials.Add("facegen_long_hair", new BestMatchMaterials(MaterialTemplateType.HaircutAlphaSamplingUnlit, MaterialTemplateType.HaircutAlphaSamplingLit));
				haircustBestMatchMaterials.Add("facegen_long_hair2", new BestMatchMaterials(MaterialTemplateType.HaircutAlphaSamplingUnlit, MaterialTemplateType.HaircutAlphaSamplingLit));
				haircustBestMatchMaterials.Add("facegen_mid_length_ruffled", new BestMatchMaterials(MaterialTemplateType.HaircutAlphaSamplingUnlit, MaterialTemplateType.HaircutAlphaSamplingLit));
				haircustBestMatchMaterials.Add("facegen_straight_bob_bangs", new BestMatchMaterials(MaterialTemplateType.HaircutAlphaSamplingUnlit, MaterialTemplateType.HaircutAlphaSamplingLit));
			}
		}

		public static string GetTemplateMaterialName(MaterialTemplateType materialTemplate)
		{
			if (materialTemplateNames == null)
			{
				throw new Exception("Incorrect initialization of material templates names: Template names is null");
			}
			else if (!materialTemplateNames.ContainsKey(materialTemplate))
			{
				throw new Exception("Incorrect initialization of material templates names: Key not found");
			}
			return materialTemplateNames[materialTemplate];
		}

		public static string GetHeadTemplateMaterialName(AvatarShaderType shaderType)
		{
			return GetTemplateMaterialName(shaderType == AvatarShaderType.UnlitShader ? MaterialTemplateType.HeadUnlit : MaterialTemplateType.HeadStandard);
		}

		public static string GetBestMatchHaircutTemplateMaterialName(string haircutName, AvatarShaderType shaderType)
		{
			MaterialTemplateType haircutMaterialTemplate = GetHaircutMaterialType(haircutName, shaderType);
			return GetTemplateMaterialName(haircutMaterialTemplate);
		}

		public static bool IsCullingRequiredForHaircut(string haircutName)
		{
			string shortHaircutName = CoreTools.GetShortHaircutId(haircutName);
			return haircutsWithCulling.Contains(shortHaircutName);
		}

		private static MaterialTemplateType GetHaircutMaterialType(string haircutName, AvatarShaderType shaderType)
		{
			string shortHaircutName = CoreTools.GetShortHaircutId(haircutName);

			if (haircustBestMatchMaterials.ContainsKey(shortHaircutName))
			{
				BestMatchMaterials haircutBestMatch = haircustBestMatchMaterials[shortHaircutName];
				return shaderType == AvatarShaderType.UnlitShader ? haircutBestMatch.unlitMaterial : haircutBestMatch.litMaterial;
			}

			if (shaderType == AvatarShaderType.UnlitShader)
				return MaterialTemplateType.HaircutAlphaSamplingUnlit;
			else
				return MaterialTemplateType.HaircutOpaque;
		}
	}
}
