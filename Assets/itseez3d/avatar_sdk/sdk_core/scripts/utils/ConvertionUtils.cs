/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
* You may not use this file except in compliance with an authorized license
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
* See the License for the specific language governing permissions and limitations under the License.
* Written by Itseez3D, Inc. <support@avatarsdk.com>, January 2021
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ItSeez3D.AvatarSdk.Core
{
	public static class ConvertionUtils
	{
		public static TextureSize StrToTextureSize(string wstr, string hstr)
		{
			return new TextureSize(StrToInt(wstr), StrToInt(hstr));
		}

		public static int StrToInt(string str)
		{
			int result;
			if (int.TryParse(str, out result))
			{
				return result;
			}
			else
			{
				Debug.LogErrorFormat("Unable to parse string as int: {0}", str);
				return 0;
			}
		}

		public static Color StrToColor(string str)
		{
			try
			{
				string[] parts = str.Split(',');
				int red = int.Parse(parts[0]);
				int green = int.Parse(parts[1]);
				int blue = int.Parse(parts[2]);
				return new Color(red / 255.0f, green / 255.0f, blue / 255.0f);
			}
			catch
			{
				Debug.LogErrorFormat("Unable to parse color value: {0}", str);
				return Color.white;
			}
		}
	}
}
