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
	public enum Flavour { CLOUD = 1, LOCAL_COMPUTE = 2, FLAVOUR_UNKNOWN = 0 };
	public static class FlavourTraitsFacotry
	{
		static Dictionary<Flavour, FlavourTraits> flavourTraits = new Dictionary<Flavour, FlavourTraits>()
		{
			{
				Flavour.CLOUD,
				new FlavourTraits()
					{
						UpdateCheckMemo = "avatar_sdk_last_update_check_cloud",
						UpdateCheckUrl = "https://releases.avatarsdk.com/unity-plugin/version_cloud.txt",
						Version = new Version(3, 0, 1),
						Name = "Avatar SDK Cloud"
					}
			},
			{
				Flavour.LOCAL_COMPUTE,
				new FlavourTraits()
					{
						UpdateCheckMemo = "avatar_sdk_last_update_check_local_compute",
						UpdateCheckUrl = "https://releases.avatarsdk.com/unity-plugin/version_fullbody_local_compute.txt",
						Version = new Version(3, 0, 0),
						Name = "Avatar SDK Local Compute"
					}
			}
		};
		public static FlavourTraits GetTraits(this Flavour traits)
		{
			return traits == Flavour.FLAVOUR_UNKNOWN ? new FlavourTraits() : flavourTraits[traits];
		}
	}
	public struct FlavourTraits
	{
		public string UpdateCheckMemo { get; set; }
		public string UpdateCheckUrl { get; set; }
		public Version Version { get; set; }
		public string Name { get; set; }
	}
}
