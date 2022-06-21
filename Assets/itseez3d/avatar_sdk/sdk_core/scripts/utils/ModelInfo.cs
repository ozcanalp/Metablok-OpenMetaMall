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
using UnityEngine;

namespace ItSeez3D.AvatarSdk.Core
{
	public enum AvatarAgeGroup
	{
		Unknown,
		Child, 
		Adult
	}

	public enum AvatarGender
	{
		Unknown,
		Male,
		Female,
		NonBinary
	}


	[Serializable]
	public class AvatarColor
	{
		public int blue = -1;

		public int green = -1;

		public int red = -1;

		public bool IsClear()
		{
			return blue == -1 && red == -1 && green == -1;
		}

		public Color ToUnityColor()
		{
			if (!IsClear())
				return new Color(red / 255.0f, green / 255.0f, blue / 255.0f);
			return Color.clear;
		}
	}

	[Serializable]
	public class RacesInfo
	{
		public float asian = float.NaN;
		public float black = float.NaN;
		public float white = float.NaN;
		public bool IsClear()
		{
			return float.IsNaN(asian) || float.IsNaN(black) || float.IsNaN(white);
		}
		public string GetPredictedRace()
		{
			if(IsClear()) { return ""; }
			int ab = asian.CompareTo(black);
			int aw = asian.CompareTo(white);
			int bw = black.CompareTo(white);
			if (ab >= 0 && aw >= 0) return "asian";
			if (ab <= 0 && bw >= 0) return "black";
			if (aw <= 0 && bw <= 0) return "white";
			return "";
		}
	}

	public class HaircutInfo
	{
		public string name;
		public float confidence;
	}

	[Serializable]
	public class ModelInfo
	{
		public string pipeline;

		public string pipeline_subtype;

		public string haircut_name;

		public HaircutInfo[] haircut_full_info;

		public AvatarColor hair_color;

		public AvatarColor skin_color;

		public AvatarColor eye_iris_color;

		public AvatarColor lips_color;

		public AvatarColor eye_sclera_color;

		public RacesInfo races;

		public bool remove_smile;

		public string gender;

		public float gender_confidence;

		public string age;

		public float age_confidence;

		public float[] facial_landmarks_68;


		public AvatarAgeGroup AgeGroup
		{
			get
			{
				if (string.IsNullOrEmpty(age))
					return AvatarAgeGroup.Unknown;

				if (age == "child")
					return AvatarAgeGroup.Child;

				if (age == "not_child")
					return AvatarAgeGroup.Adult;

				return AvatarAgeGroup.Unknown;
			}
		}

		public AvatarGender Gender
		{
			get
			{
				if (string.IsNullOrEmpty(gender))
					return AvatarGender.Unknown;

				if (gender == "male")
					return AvatarGender.Male;

				if (gender == "female")
					return AvatarGender.Female;

				return AvatarGender.Unknown;
			}
		}

		public static bool HasPredictedData(ModelInfo modelInfo)
		{
			if (modelInfo == null)
				return false;

			if (!string.IsNullOrEmpty(modelInfo.haircut_name) ||
				!string.IsNullOrEmpty(modelInfo.gender) ||
				!string.IsNullOrEmpty(modelInfo.age))
				return true;

			if (modelInfo.hair_color != null && !modelInfo.hair_color.IsClear())
				return true;

			if (modelInfo.skin_color != null && !modelInfo.skin_color.IsClear())
				return true;

			if (modelInfo.eye_iris_color != null && !modelInfo.eye_iris_color.IsClear())
				return true;

			if (modelInfo.lips_color != null && !modelInfo.lips_color.IsClear())
				return true;

			if (modelInfo.eye_sclera_color != null && !modelInfo.eye_sclera_color.IsClear())
				return true;

			if (modelInfo.facial_landmarks_68 != null && modelInfo.facial_landmarks_68.Length > 0)
				return true;

			if (modelInfo.races != null && !modelInfo.races.IsClear())
			{
				return true;
			}

			return false;
		}

	}
}
