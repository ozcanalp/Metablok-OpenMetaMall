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
	public interface IFullbodyAvatarProvider: IAvatarProvider
	{
		/// <summary>
		/// Initializes the avatar and starts calculations
		/// </summary>
		AsyncRequest<string> InitializeFullbodyAvatarAsync(byte[] photoBytes, FullbodyAvatarComputationParameters computationParameters, PipelineType pipelineType,
			string name = "name", string description = "");

		/// <summary>
		/// Initializes the avatar by selfie code and starts calculations
		/// </summary>
		AsyncRequest<string> InitializeFullbodyAvatarAsync(string selfieCode, byte[] selfiePhotoBytes, FullbodyAvatarComputationParameters computationParameters, PipelineType pipelineType,
			string name = "name", string description = "");

		/// <summary>
		/// Some haircuts may be stored as a discrete GLTF files, not in the common GLTF with avatar.
		/// This method return such haircuts names.
		/// </summary>
		List<string> GetHaircutsInDiscreteFiles(string avatarCode);

		/// <summary>
		/// Some outfits may be stored as a discrete GLTF files, not in the common GLTF with avatar.
		/// This method return such outfits names.
		/// </summary>
		List<string> GetOutfitsInDiscreteFiles(string avatarCode);

		/// <summary>
		/// Downloads all avatar data from the Cloud and stores it on local drive.
		/// It downloads all haircuts and outfits.
		/// This method makes sense only for the Cloud version of the SDK. It do nothing in case of LocalCompute version.
		/// </summary>
		AsyncRequest RetrieveAllAvatarDataFromCloudAsync(string avatarCode);

		/// <summary>
		/// Downloads mesh file and textures for the body model if they are not exists.
		/// This method makes sense only for the Cloud version of the SDK. It do nothing in case of LocalCompute version.
		/// </summary>
		AsyncRequest RetrieveBodyModelFromCloudAsync(string avatarCode);

		/// <summary>
		/// Downloads mesh file and texture for the haircut if they are not exists.
		/// This method makes sense only for the Cloud version of the SDK. It do nothing in case of LocalCompute version.
		/// </summary>
		AsyncRequest RetrieveHaircutModelFromCloudAsync(string avatarCode, string haircutName);

		/// <summary>
		/// Downloads mesh file and textures for the outfit if they are not exists.
		/// This method makes sense only for the Cloud version of the SDK. It do nothing in case of LocalCompute version.
		/// </summary>
		AsyncRequest RetrieveOutfitModelFromCloudAsync(string avatarCode, string outfitName);

		/// <summary>
		/// Requests all available parameters for the fullbody pipeline
		/// </summary>
		AsyncRequest<FullbodyAvatarComputationParameters> GetAvailableComputationParametersAsync(PipelineType pipelineType);
	}
}
