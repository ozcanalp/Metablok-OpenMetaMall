/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
* You may not use this file except in compliance with an authorized license
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
* See the License for the specific language governing permissions and limitations under the License.
* Written by Itseez3D, Inc. <support@avatarsdk.com>, April 2017
*/

using ItSeez3D.AvatarSdk.Core;
using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ItSeez3D.AvatarSdk.Cloud
{
	/// <summary>
	/// Resource manager for Cloud SDK
	/// </summary>
	public class CloudComputationParametersController
	{
		protected const string PIPELINE_KEY = "pipeline";
		protected const string PIPELINE_SUBTYPE_KEY = "pipeline_subtype";
		protected const string BLENDSHAPES_KEY = "blendshapes";
		protected const string HAIRCUTS_KEY = "haircuts";
		protected const string AVATAR_MODIFICATIONS = "avatar_modifications";
		protected const string MODEL_INFO = "model_info";
		protected const string SHAPE_MODIFICATIONS = "shape_modifications";
		protected const string ADDITIONAL_TEXTURES = "additional_textures";
		protected const string BODY_SHAPE = "body_shape";
		protected const string OUTFITS = "outfits";


		/// <summary>
		/// Converts ComputationParameters to the json format for cloud computations
		/// </summary>
		public string GetCalculationParametersJson(PipelineType pipelineType, ComputationParameters computationParams)
		{
			JSONObject resourcesJson = new JSONObject();

			if (computationParams != null)
			{
				if (!IsListNullOrEmpty(computationParams.blendshapes.Values))
					resourcesJson[BLENDSHAPES_KEY] = computationParams.blendshapes.ToJson();

				if (!IsListNullOrEmpty(computationParams.haircuts.Values))
					resourcesJson[HAIRCUTS_KEY] = computationParams.haircuts.ToJson();

				if (!IsListNullOrEmpty(computationParams.additionalTextures.Values))
					resourcesJson[ADDITIONAL_TEXTURES] = computationParams.additionalTextures.ToJson();

				if (!computationParams.modelInfo.IsEmpty())
					resourcesJson[MODEL_INFO] = computationParams.modelInfo.ToJson();

				if (!computationParams.avatarModifications.IsEmpty())
				{
					if (pipelineType == PipelineType.HEAD_2_0_HEAD_MOBILE)
						computationParams.avatarModifications.allowModifyNeck.GroupName = "base";
					resourcesJson[AVATAR_MODIFICATIONS] = computationParams.avatarModifications.ToJson();
				}

				if (!computationParams.shapeModifications.IsEmpty())
					resourcesJson[SHAPE_MODIFICATIONS] = computationParams.shapeModifications.ToJson();

				if (!computationParams.bodyShape.IsEmpty())
					resourcesJson[BODY_SHAPE] = computationParams.bodyShape.ToJson();

				if (!IsListNullOrEmpty(computationParams.outfits.Values))
					resourcesJson[OUTFITS] = computationParams.outfits.ToJson();
			}

			return resourcesJson.ToString(4);
		}

		/// <summary>
		/// Parses JSON to AvatarParameters
		/// </summary>
		public static ComputationParameters GetParametersFromJson(string json)
		{
			ComputationParameters computationParams = ComputationParameters.Empty;
			var rootNode = JSON.Parse(json);
			if (rootNode != null)
			{
				var blendshapesRootNode = JsonUtils.FindNodeByName(rootNode, BLENDSHAPES_KEY);
				if (blendshapesRootNode != null)
					computationParams.blendshapes = new ComputationList(blendshapesRootNode);

				var haircutsRootNode = JsonUtils.FindNodeByName(rootNode, HAIRCUTS_KEY);
				if (haircutsRootNode != null)
					computationParams.haircuts = new ComputationList(haircutsRootNode);

				computationParams.avatarModifications.SetPropertiesToUnavailableState();
				var avatarModificationsNode = JsonUtils.FindNodeByName(rootNode, AVATAR_MODIFICATIONS);
				if (avatarModificationsNode != null)
					computationParams.avatarModifications.FromJson(avatarModificationsNode);

				computationParams.modelInfo.SetPropertiesToUnavailableState();
				var modelInfoNode = JsonUtils.FindNodeByName(rootNode, MODEL_INFO);
				if (modelInfoNode != null)
					computationParams.modelInfo.FromJson(modelInfoNode);

				computationParams.shapeModifications.SetPropertiesToUnavailableState();
				var shapeModificationsNode = JsonUtils.FindNodeByName(rootNode, SHAPE_MODIFICATIONS);
				if (shapeModificationsNode != null)
					computationParams.shapeModifications.FromJson(shapeModificationsNode);

				var additionalTexturesNode = JsonUtils.FindNodeByName(rootNode, ADDITIONAL_TEXTURES);
				if (additionalTexturesNode != null)
					computationParams.additionalTextures = new ComputationList(additionalTexturesNode);

				computationParams.bodyShape.SetPropertiesToUnavailableState();
				var bodyShape = JsonUtils.FindNodeByName(rootNode, BODY_SHAPE);
				if (bodyShape != null)
					computationParams.bodyShape.FromJson(bodyShape);

				var outfitsNode = JsonUtils.FindNodeByName(rootNode, OUTFITS);
				if (outfitsNode != null)
					computationParams.outfits = new ComputationList(outfitsNode);
			}
			return computationParams;
		}

		/// <summary>
		/// Checks if list is null or empty
		/// </summary>
		protected static bool IsListNullOrEmpty<T>(List<T> list)
		{
			return list == null || list.Count == 0;
		}
	}
}
