﻿/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
* You may not use this file except in compliance with an authorized license
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
* See the License for the specific language governing permissions and limitations under the License.
* Written by Itseez3D, Inc. <support@avatarsdk.com>, April 2017
*/

using UnityEngine.Networking;

namespace ItSeez3D.AvatarSdk.Core
{
	/// <summary>
	/// Some basic network stuff needed in both Cloud and Local Compute SDK.
	/// </summary>
	public static class NetworkUtils
	{
		/// <summary>
		/// The root URL for Avatar SDK server.
		/// </summary>
		public const string rootUrl = "https://api.avatarsdk.com";
		
		public static bool IsWebRequestFailed(UnityWebRequest webRequest)
		{
#if UNITY_2020_2_OR_NEWER
			return webRequest.result == UnityWebRequest.Result.ConnectionError;
#else
			return webRequest.isNetworkError;
#endif
		}
	}
}
