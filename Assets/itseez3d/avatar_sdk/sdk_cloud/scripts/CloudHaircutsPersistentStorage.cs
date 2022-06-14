/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
* You may not use this file except in compliance with an authorized license
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
* See the License for the specific language governing permissions and limitations under the License.
* Written by Itseez3D, Inc. <support@avatarsdk.com>, October 2019
*/

using ItSeez3D.AvatarSdk.Core;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace ItSeez3D.AvatarSdk.Cloud
{
	public class CloudHaircutsPersistentStorage : IHaircutsPersistentStorage
	{

		public string GetCommonHaircutsDirectory()
		{
			return persistentStorage.EnsureDirectoryExists(Path.Combine(persistentStorage.GetDataDirectory(), "haircuts"));
		}

		public HaircutMetadata GetHaircutMetadata(string haircutId, string avatarCode)
		{
			HaircutMetadata result = new HaircutMetadata();

			result.ShortId = CoreTools.GetShortHaircutId(haircutId);
			result.FullId = haircutId;

			//pointclouds needeed for common haircuts
			bool haircutWithPointcloud = true;
			if (!string.IsNullOrEmpty(avatarCode))
			{
				PipelineTypeTraits pipelineTraits = PipelineTraitsFactory.Instance.GetTraitsFromAvatarCode(avatarCode);
				if (pipelineTraits.Type.IsFullbodyPipeline())
				{
					SetMetadataForFullbodyHaircut(result, avatarCode, pipelineTraits.IsPointcloudApplicableToHaircut(haircutId));
					return result;
				}

				haircutWithPointcloud = pipelineTraits.IsPointcloudApplicableToHaircut(haircutId);
			}

			string avatarHaircutsDirectory = "";
			if (avatarCode != null)
			{
				avatarHaircutsDirectory = Path.Combine(persistentStorage.GetAvatarDirectory(avatarCode), "haircuts"); //files are located at local avatar directory
			}

			if (haircutWithPointcloud)
			{
				result.Texture = Path.Combine(GetCommonHaircutsDirectory(), string.Format("{0}.png", haircutId));
				result.MeshPly = Path.Combine(GetCommonHaircutsDirectory(), string.Format("{0}.ply", haircutId));
				result.Preview = Path.Combine(GetCommonHaircutsDirectory(), string.Format("{0}_preview.png", haircutId));

				if(avatarCode != null)
				{
					result.PathToPointCloud = Path.Combine(avatarHaircutsDirectory, string.Format("cloud_{0}.ply", result.ShortId));
					result.PathToPointCloudZip = Path.Combine(avatarHaircutsDirectory, string.Format("{0}_points.zip", haircutId));
				}
			}
			else
			{
				result.Texture = Path.Combine(avatarHaircutsDirectory, string.Format("{0}.png", haircutId));
				result.MeshPly = Path.Combine(avatarHaircutsDirectory, string.Format("{0}.ply", haircutId));
				result.Preview = null;
			}

			return result;
		}

		public List<HaircutMetadata> ReadHaircutsMetadataFromFile(string avatarCode)
		{
			return new List<HaircutMetadata>();
		}

		private IPersistentStorage persistentStorage;
		public void SetPersistentStorage(IPersistentStorage storage)
		{
			persistentStorage = storage;
		}

		private void SetMetadataForFullbodyHaircut(HaircutMetadata haircutMetadata, string avatarCode, bool isCommonHaircut)
		{
			if (haircutMetadata.FullId == haircutMetadata.ShortId)
				haircutMetadata.ShortId = CoreTools.GetShortFullbodyHaircutId(haircutMetadata.FullId);
			string avatarHaircutsDirectory = Path.Combine(persistentStorage.GetAvatarDirectory(avatarCode), "haircuts");
			haircutMetadata.Texture = Path.Combine(avatarHaircutsDirectory, string.Format("{0}.png", haircutMetadata.ShortId));
			if (isCommonHaircut)
			{
				haircutMetadata.CommonTexture = Path.Combine(GetCommonHaircutsDirectory(), string.Format("{0}.png", haircutMetadata.FullId));
				haircutMetadata.Preview = Path.Combine(GetCommonHaircutsDirectory(), string.Format("{0}_preview.png", haircutMetadata.FullId));
			}
		}
	}
}
