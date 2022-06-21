/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
* You may not use this file except in compliance with an authorized license
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
* See the License for the specific language governing permissions and limitations under the License.
* Written by Itseez3D, Inc. <support@avatarsdk.com>, May 2019
*/

using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ItSeez3D.AvatarSdk.Core
{
	/// <summary>
	/// Represents the value in the list of the computation params (such as haircuts, blendshapes, etc)
	/// </summary>
	public class ComputationListValue
	{
		public ComputationListValue(string groupName, string name)
		{
			GroupName = groupName;
			Name = name;
		}

		public ComputationListValue(string fullName)
		{
			string[] parts = fullName.Split('\\', '/');
			if (parts.Length == 2)
			{
				GroupName = parts[0];
				Name = parts[1];
			}
			else
				Debug.LogErrorFormat("Unable to parse parameter full name: {0}", fullName);
		}

		/// <summary>
		/// Group to which this property belongs to (base, indie, plus etc)
		/// </summary>
		public string GroupName { get; set; }

		/// <summary>
		/// Name of the parameter
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Full name of the parameter (GroupName + Name)
		/// </summary>
		public string FullName
		{
			get
			{
				return string.Format("{0}/{1}", GroupName, Name);
			}
		}
	}

	/// <summary>
	/// List of computation values such as haircuts, blendshapes
	/// </summary>
	public class ComputationList
	{
		private List<ComputationListValue> values = new List<ComputationListValue>();

		public ComputationList() { }

		public ComputationList(List<string> fullNames)
		{
			foreach (string fullName in fullNames)
				values.Add(new ComputationListValue(fullName));
		}

		public ComputationList(JSONNode jsonNode)
		{
			foreach (JSONNode tag in jsonNode.Keys)
			{
				var parametersArray = jsonNode[tag.Value];
				foreach (var p in parametersArray)
					values.Add(new ComputationListValue(tag.Value, p.Value));
			}
		}

		public List<ComputationListValue> Values
		{
			get { return values; }
		}

		public List<string> FullNames
		{
			get
			{
				List<string> fullNames = new List<string>();
				values.ForEach(v => fullNames.Add(v.FullName));
				return fullNames;
			}
		}

		public void Merge(ComputationList computationList)
		{
			foreach(var value in computationList.Values)
			{
				if (values.FirstOrDefault(v => v.FullName == value.FullName) == null)
				{
					values.Add(value);
				}
			}
		}

		public void AddValue(string fullName)
		{
			values.Add(new ComputationListValue(fullName));
		}

		public JSONNode ToJson(bool useGroupName = true)
		{
			if (useGroupName)
			{
				Dictionary<string, JSONArray> groups = new Dictionary<string, JSONArray>();

				foreach (ComputationListValue v in values)
				{
					if (!groups.ContainsKey(v.GroupName))
						groups.Add(v.GroupName, new JSONArray());
					groups[v.GroupName][""] = v.Name;
				}

				JSONObject node = new JSONObject();
				foreach (var group in groups)
				{
					node[group.Key] = group.Value;
				}
				return node;
			}
			else
			{
				JSONArray node = new JSONArray();
				foreach(ComputationListValue v in values)
					node[""] = v.Name;
				return node;
			}
		}
	}
}
