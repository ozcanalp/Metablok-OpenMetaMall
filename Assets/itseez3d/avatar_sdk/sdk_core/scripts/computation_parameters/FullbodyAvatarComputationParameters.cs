/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
* You may not use this file except in compliance with an authorized license
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
* See the License for the specific language governing permissions and limitations under the License.
* Written by Itseez3D, Inc. <support@avatarsdk.com>, November 2020
*/

using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ItSeez3D.AvatarSdk.Core
{
	/// <summary>
	/// Computation parameters for fullbody model
	/// </summary>
	public class FullbodyAvatarComputationParameters
	{
		protected const string BLENDSHAPES_KEY = "blendshapes";
		protected const string HAIRCUTS_KEY = "haircuts";
		protected const string OUTFITS_KEY = "outfits";
		protected const string AVATAR_MODIFICATIONS_KEY = "avatar_modifications";
		protected const string MODEL_INFO_KEY = "model_info";
		protected const string ADDITIONAL_TEXTURES_KEY = "additional_textures";
		protected const string BODY_SHAPE_KEY = "body_shape";

		public MeshFormat meshFormat = MeshFormat.GLTF;

		public ExportTemplate template = ExportTemplate.FULLBODY;

		public int lod = 0;

		public bool embedTextures = false;

		public TextureSize textureSize = null;

		public HaircutsParameters haircuts = new HaircutsParameters();

		public BlendshapesParameters blendshapes = new BlendshapesParameters();

		public OutfitsParameters outfits = new OutfitsParameters();

		public AdditionalTexturesParameters additionalTextures = new AdditionalTexturesParameters();

		public ModelInfoGroup modelInfo = new ModelInfoGroup();

		public AvatarModificationsGroup avatarModifications = new AvatarModificationsGroup();

		public BodyShapeGroup bodyShape = new BodyShapeGroup();

		public string GetExportJson()
		{
			JSONObject json = new JSONObject();
			json["format"] = meshFormat.MeshFormatToStr();
			json["embed_textures"] = embedTextures;

			if (template == ExportTemplate.HEAD)
				json["template"] = "head";

			if (lod > 0)
				json["lod"] = lod.ToString();

			if (textureSize != null)
				json["texture_size"] = textureSize.ToJson();

			if (haircuts != null && haircuts.names.Count > 0)
				json[HAIRCUTS_KEY] = haircuts.ToJson();

			if (blendshapes != null && blendshapes.names.Count > 0)
				json[BLENDSHAPES_KEY] = blendshapes.ToJson();

			if (outfits != null && outfits.names.Count > 0)
				json[OUTFITS_KEY] = outfits.ToJson();

			if (additionalTextures != null && additionalTextures.names.Count > 0)
				json[ADDITIONAL_TEXTURES_KEY] = additionalTextures.ToJson();

			return json.ToString();
		}

		public string GetParametersJson()
		{
			JSONObject json = new JSONObject();

			if (!modelInfo.IsEmpty())
				json[MODEL_INFO_KEY] = modelInfo.ToJson(false);

			if (!avatarModifications.IsEmpty())
				json[AVATAR_MODIFICATIONS_KEY] = avatarModifications.ToJson(false);

			if (!bodyShape.IsEmpty())
				json[BODY_SHAPE_KEY] = bodyShape.ToJson(false);

			return json.ToString();
		}

		public void ParseExportJson(string exportJson)
		{
			var rootNode = JSON.Parse(exportJson);
			if (rootNode != null)
			{
				var blendshapesRootNode = JsonUtils.FindNodeByName(rootNode, BLENDSHAPES_KEY);
				if (blendshapesRootNode != null)
				{
					foreach (var p in blendshapesRootNode)
						blendshapes.names.Add(p.Value);
				}

				var haircutsRootNode = JsonUtils.FindNodeByName(rootNode, HAIRCUTS_KEY);
				if (haircutsRootNode != null)
				{
					foreach (var p in haircutsRootNode)
						haircuts.names.Add(p.Value);
				}

				var outfitsRootNode = JsonUtils.FindNodeByName(rootNode, OUTFITS_KEY);
				if (outfitsRootNode != null)
				{
					foreach (var p in outfitsRootNode)
						outfits.names.Add(p.Value);
				}
			}
		}

		public void ParseParametersJson(string parametersJson)
		{
			var rootNode = JSON.Parse(parametersJson);
			if (rootNode != null)
			{
				avatarModifications.SetPropertiesToUnavailableState();
				var avatarModificationsNode = JsonUtils.FindNodeByName(rootNode, AVATAR_MODIFICATIONS_KEY);
				if (avatarModificationsNode != null)
					avatarModifications.FromJson(avatarModificationsNode);

				modelInfo.SetPropertiesToUnavailableState();
				var modelInfoNode = JsonUtils.FindNodeByName(rootNode, MODEL_INFO_KEY);
				if (modelInfoNode != null)
					modelInfo.FromJson(modelInfoNode);

				bodyShape.SetPropertiesToUnavailableState();
				var bodyShapeNode = JsonUtils.FindNodeByName(rootNode, BODY_SHAPE_KEY);
				if (bodyShapeNode != null)
					bodyShape.FromJson(bodyShapeNode);
			}
		}
	}
}
