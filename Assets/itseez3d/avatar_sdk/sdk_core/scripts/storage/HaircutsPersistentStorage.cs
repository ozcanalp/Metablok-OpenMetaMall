/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
* You may not use this file except in compliance with an authorized license
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
* See the License for the specific language governing permissions and limitations under the License.
* Written by Itseez3D, Inc. <support@avatarsdk.com>, October 2019
*/

using System.Collections.Generic;

namespace ItSeez3D.AvatarSdk.Core
{
	public class HaircutMetadata
	{
		public string PathToPointCloud { get; set; }
		public string PathToPointCloudZip { get; set; }
		public string MeshPly { get; set; }
		public string MeshZip { get; set; }
		public string Texture { get; set; }
		public string CommonTexture { get; set; }
		public string Preview { get; set; }
		public string ShortId { get; set; }
		public string FullId { get; set; }
	}

	public interface IHaircutsPersistentStorage
	{
		void SetPersistentStorage(IPersistentStorage storage);
		HaircutMetadata GetHaircutMetadata(string haircutId, string avatarCode);
		List<HaircutMetadata> ReadHaircutsMetadataFromFile(string avatarCode);
		string GetCommonHaircutsDirectory();
	}

	public class HaircutsPersistentStorage
	{
		public static IHaircutsPersistentStorage Instance
		{
			get
			{
				if (instance == null)
				{
					instance = AvatarSdkMgr.IoCContainer.Create<IHaircutsPersistentStorage>();
					instance.SetPersistentStorage(AvatarSdkMgr.Storage());
				}
				return instance;
			}
		}
		private static IHaircutsPersistentStorage instance = null;
	}
}