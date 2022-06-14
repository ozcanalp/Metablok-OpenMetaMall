/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
* You may not use this file except in compliance with an authorized license
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
* See the License for the specific language governing permissions and limitations under the License.
* Written by Itseez3D, Inc. <support@avatarsdk.com>, November 2020
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ItSeez3D.AvatarSdk.Core
{
	/// <summary>
	/// Predefined subdirectories inside avatar folder.
	/// </summary>
	public enum AvatarSubdirectory
	{
		BLENDSHAPES,
		OBJ_EXPORT,
		FBX_EXPORT,
		LOD,
	}

	/// <summary>
	/// "Types" of files encountered during avatar generation and loading.
	/// </summary>
	public enum AvatarFile
	{
		PHOTO,
		MESH_PLY,
		MESH_GLTF,
		MESH_ZIP,
		TEXTURE,
		METALLIC_MAP_TEXTURE,
		ROUGHNESS_MAP_TEXTURE,
		HAIRCUT_POINT_CLOUD_PLY,
		HAIRCUT_POINT_CLOUD_ZIP,
		ALL_HAIRCUT_POINTS_ZIP,
		HAIRCUTS_JSON,
		BLEDNSHAPES_JSON,
		BLENDSHAPES_ZIP,
		BLENDSHAPES_FBX_ZIP,
		BLENDSHAPES_PLY_ZIP,
		PARAMETERS_JSON,
		EXPORT_PARAMETERS_JSON,
		PIPELINE_INFO,
		MODEL_JSON
	}

	/// <summary>
	/// "Types" of files encountered during haircut loading.
	/// </summary>
	public enum HaircutFile
	{
		HAIRCUT_MESH_PLY,
		HAIRCUT_MESH_ZIP,
		HAIRCUT_TEXTURE,
		HAIRCUT_PREVIEW
	}

	/// <summary>
	/// SDK uses this interface to interact with the filesystem, e.g. save/load files and metadata.
	/// By default SDK will use DefaultPersistentStorage implementation. If your application stores files differently
	/// you can implement this interface and pass instance of your implementation to AvatarSdkMgr.Init() - this
	/// will override the default behavior. Probably the best way to implement IPersistentStorage is to derive from
	/// DefaultPersistentStorage.
	/// </summary>
	public interface IPersistentStorage
	{
		Dictionary<AvatarSubdirectory, string> AvatarSubdirectories { get; }

		Dictionary<AvatarFile, string> AvatarFilenames { get; }

		Dictionary<HaircutFile, string> HaircutFilenames { get; }

		string EnsureDirectoryExists(string d);

		string GetDataDirectory();

		string GetResourcesDirectory();

		string GetAvatarsDirectory();

		string GetAvatarDirectory(string avatarCode, int levelOfDetails = 0);

		string GetAvatarSubdirectory(string avatarCode, AvatarSubdirectory dir, int levelOfDetails = 0);

		string GetAvatarFilename(string avatarCode, AvatarFile file, int levelOfDetails = 0);

		/// <summary>
		/// Returns avatar texture filename or additional texture filename if it is specified
		/// </summary>
		string GetAvatarTextureFilename(string avatarCode, string additionalTextureName = null);

		List<string> GetAvatarBlendshapesDirs(string avatarCode, int levelOfDetails = 0);

		string GetAvatarBlendshapesRootDir(string avatarCode, int levelOfDetails = 0);

		List<string> GetFullBlendshapesNames(string avatarCode);

		void StorePlayerUID(string identifier, string uid);

		string LoadPlayerUID(string identifier);
	}
}
