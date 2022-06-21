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

namespace ItSeez3D.AvatarSdk.Cloud
{
	[Serializable]
	public class ExportItem
	{
		public string file = string.Empty;

		public string identity = string.Empty;

		public string category = string.Empty;

		public string[] static_files = null;
	}


	[Serializable]
	public class ExportData
	{
		public string avatar_code = string.Empty;

		public string code = string.Empty;

		public string created_on = string.Empty;

		public string status = string.Empty;

		public string url = string.Empty;

		public ExportItem[] files = null;

		public bool IsEmpty()
		{
			return string.IsNullOrEmpty(avatar_code) &&
				string.IsNullOrEmpty(code) &&
				string.IsNullOrEmpty(created_on) &&
				string.IsNullOrEmpty(status) &&
				string.IsNullOrEmpty(url) &&
				files == null;
		}
	}
}
