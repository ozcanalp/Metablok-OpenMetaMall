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
	public enum FullbodyCommonFileType
	{
		ExportDataJson,
		HaircutsList,
		OutfitsList
	}

	public enum FullbodyAvatarFileType
	{
		Model,
		Texture,
		MetallnessMap,
		RoughnessMap,
		NormalMap,
		ModelInfo
	}

	public enum FullbodyHaircutFileType
	{
		Model,
		Texture
	}

	public enum OutfitFileType
	{
		Model,
		Texture,
		MetallnessMap,
		RoughnessMap,
		NormalMap,
		BodyVisibilityMask,
	}


	/// <summary>
	/// SDK uses this interface to save/load files for fullbody avatars.
	/// By default SDK will use FullbodyPersistentStorage implementation. If your application stores files differently
	/// you can implement this interface and pass instance of your implementation to AvatarSdkMgr.Init() - this
	/// will override the default behavior. Probably the best way to implement IFullbodyPersistentStorage is to derive from
	/// FullbodyPersistentStorage.
	/// </summary>
	public interface IFullbodyPersistentStorage
	{
		string GetAvatarsDirectory();

		string GetAvatarDirectory(string avatarCode);

		string GetCommonFile(string avatarCode, FullbodyCommonFileType fileType); 

		string GetAvatarFile(string avatarCode, FullbodyAvatarFileType fileType);

		string GetHaircutFile(string avatarCode, string haircutName, FullbodyHaircutFileType fileType);

		string GetHaircutFileInDir(string haircutDir, string haircutName, FullbodyHaircutFileType fileType);

		string GetOutfitFile(string avatarCode, string outfitName, OutfitFileType outfitFileType);

		string GetOutfitFileInDir(string outfitDir, string outfitName, OutfitFileType outfitFileType);

		string GetStaticFilesDirectory();

		string GetStaticFilePath(string relativePath);

		string GetIntermediateDataDir(string avatarCode);
	}
}
