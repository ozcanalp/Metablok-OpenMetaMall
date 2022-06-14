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
	public class AdditionalTexturesParameters
	{
		private readonly List<string> availableTexturesNames = new List<string>()
		{
			"normal_map",
			"roughness_map",
			"metallic_map"
		};

		public List<string> names = new List<string>();

		public AdditionalTexturesParameters()
		{
			IncludeAllAvailableTextures();
		}

		public void IncludeAllAvailableTextures()
		{
			foreach (string textureName in availableTexturesNames)
			{
				if (!names.Contains(textureName))
					names.Add(textureName);
			}
		}

		public JSONNode ToJson()
		{
			JSONArray list = new JSONArray();
			foreach (string name in names)
				list.Add(name);
			return list;
		}
	}
}
