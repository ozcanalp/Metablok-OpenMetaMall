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
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ItSeez3D.AvatarSdk.Core
{
	public class TextureSize
	{
		public int width;
		public int height;

		public TextureSize(int w, int h)
		{
			width = w;
			height = h;
		}

		public JSONNode ToJson()
		{
			JSONObject node = new JSONObject();
			node["width"] = width;
			node["height"] = height;
			return node;
		}
	}

	/// <summary>
	/// Base class represents the computation property
	/// </summary>
	public abstract class ComputationProperty
	{
		/// <summary>
		/// Group to which this property belongs to (base, indie, plus etc)
		/// </summary>
		public string GroupName { get; set; }

		/// <summary>
		/// Property name
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Pipeline's property availability. Set when the parameters are parsed from the JSON.
		/// </summary>
		public bool IsAvailable { get; set; }

		/// <summary>
		/// True if the value is set
		/// </summary>
		public bool HasValue { get; set; }

		/// <summary>
		/// Clear value
		/// </summary>
		public void Clear()
		{
			HasValue = false;
		}

		/// <summary>
		/// Add this property to the JSONNode as Key-Value pair {Name: Value}
		/// </summary>
		public abstract void AddToJsonNode(JSONNode rootNode, bool useGroupName = true);

		/// <summary>
		/// Add the Name of this property to the JSONNode as an Item in the Array {GroupName:{Name}}
		/// </summary>
		public virtual void AddToJsonNodeAsListItem(JSONNode rootNode, bool useGroupName = true)
		{
			if (!IsAvailable)
				Debug.LogWarningFormat("Use unavailable property:{0}", Name);

			if (useGroupName)
			{
				JSONArray array = rootNode[GroupName].AsArray;
				array[""] = Name;
			}
			else
			{
				JSONArray array = rootNode.AsArray;
				array[""] = Name;
			}
		}
	}

	/// <summary>
	/// Generic avatar calculation property
	/// </summary>
	public class ComputationProperty<T> : ComputationProperty
	{
		public ComputationProperty(string name)
		{
			Name = name;
			HasValue = false;
			IsAvailable = true;
		}

		public ComputationProperty(string groupName, string name) : this(name)
		{
			GroupName = groupName;
		}

		private T value;
		public T Value
		{
			get { return value; }
			set
			{
				this.value = value;
				HasValue = true;
			}
		}

		public override void AddToJsonNode(JSONNode rootNode, bool useGroupName = true)
		{
			if (!IsAvailable)
				Debug.LogWarningFormat("Use unavailable property:{0}", Name);

			JSONNode node = null;
			if (typeof(T) == typeof(Color))
			{
				Color color = (Color)Convert.ChangeType(value, typeof(Color));
				node = new JSONObject();
				node["red"] = (int)(color.r * 255);
				node["green"] = (int)(color.g * 255);
				node["blue"] = (int)(color.b * 255);
			}
			else if (typeof(T) == typeof(TextureSize))
			{
				TextureSize size = (TextureSize)Convert.ChangeType(value, typeof(TextureSize));
				node = size.ToJson();
			}
			else if (typeof(T) == typeof(AvatarGender))
			{
				AvatarGender gender = (AvatarGender)Convert.ChangeType(value, typeof(AvatarGender));
				Func<AvatarGender, JSONString> castGender = new Func<AvatarGender, JSONString>(
					(AvatarGender ag) =>
					{
						switch (ag)
						{
							case AvatarGender.Female:
								return new JSONString("female");
							case AvatarGender.Male:
								return new JSONString("male");
							case AvatarGender.NonBinary:
								return new JSONString("non_binary");
							default:
								return new JSONString("auto");
						}
					});
				node = castGender(gender);
			}
			else if (typeof(T) == typeof(float))
				node = new JSONNumber((float)Convert.ChangeType(value, typeof(float)));
			else if (typeof(T) == typeof(int))
				node = new JSONNumber((int)Convert.ChangeType(value, typeof(int)));
			else if (typeof(T) == typeof(bool))
				node = new JSONBool((bool)Convert.ChangeType(value, typeof(bool)));
			else
				node = new JSONString(Value.ToString());

			if (useGroupName)
				rootNode[GroupName][Name] = node;
			else
				rootNode[Name] = node;
		}
	}
}

