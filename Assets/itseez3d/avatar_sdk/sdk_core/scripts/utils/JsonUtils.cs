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
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ItSeez3D.AvatarSdk.Core
{
	public static class JsonUtils
	{
		public static JSONNode FindNodeByName(JSONNode rootNode, string name)
		{
			if (rootNode == null)
				return null;

			var node = rootNode[name];
			if (node != null)
				return node;

			foreach (JSONNode childNode in rootNode.Children)
			{
				node = FindNodeByName(childNode, name);
				if (node != null)
					return node;
			}

			return null;
		}

		public static ModelInfo ModelInfoFromJson(string jsonContent)
		{
			ModelInfo modelInfo = JsonUtility.FromJson<ModelInfo>(jsonContent);

			try
			{
				const string haircutFullInfoKey = "haircut_full_info";
				JSONNode rootNode = JSONNode.Parse(jsonContent);
				if (rootNode.HasKey(haircutFullInfoKey))
				{
					JSONNode haircutFullInfoNode = rootNode[haircutFullInfoKey];
					modelInfo.haircut_full_info = new HaircutInfo[haircutFullInfoNode.Count];

					List<HaircutInfo> haircutsInfoList = new List<HaircutInfo>();
					foreach (KeyValuePair<string, JSONNode> kvp in (JSONObject)haircutFullInfoNode)
						haircutsInfoList.Add(new HaircutInfo() { name = kvp.Key, confidence = kvp.Value.AsFloat });

					modelInfo.haircut_full_info = haircutsInfoList.ToArray();
				}
			}
			catch(Exception exc)
			{
				Debug.LogErrorFormat("Exception: {0} during parsing json: {1}", exc, jsonContent);
			}

			return modelInfo;
		}
	}
}
