﻿/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
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
	public class OutfitsParameters
	{
		public List<string> names = new List<string>();

		public MeshFormat? meshFormat = null;

		public bool embed = true;

		public bool embedTextures = false;

		public TextureSize textureSize = null;

		public List<string> additionalTextures = new List<string>();

		private readonly List<string> availableTexturesNames = new List<string>()
		{
			"normal_map",
			"roughness_map",
			"metallic_map",
			"body_visibility_mask"
		};

		public OutfitsParameters()
		{
			IncludeAllAvailableTextures();
		}

		public void IncludeAllAvailableTextures()
		{
			foreach (string textureName in availableTexturesNames)
			{
				if (!additionalTextures.Contains(textureName))
					additionalTextures.Add(textureName);
			}
		}

		public JSONNode ToJson()
		{
			JSONNode json = new JSONObject();
			JSONArray list = new JSONArray();
			foreach (string name in names)
				list.Add(name);
			json["list"] = list;
			json["embed"] = embed;
			json["embed_textures"] = embedTextures;
			if (meshFormat != null)
				json["format"] = meshFormat.Value.MeshFormatToStr();
			if (textureSize != null)
				json["texture_size"] = textureSize.ToJson();
			if (additionalTextures.Count > 0)
			{
				JSONArray texturesLst = new JSONArray();
				foreach (string textureName in additionalTextures)
					texturesLst.Add(textureName);
				json["additional_textures"] = texturesLst;
			}
			return json;
		}
	}
}
