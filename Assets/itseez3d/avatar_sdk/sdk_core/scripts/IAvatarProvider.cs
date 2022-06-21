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
	/// High-level interface that provides uniform code to work with avatars for both Cloud and LocalCompute SDK.
	public interface IAvatarProvider : IDisposable
	{
		/// <summary>
		/// Initialization status
		/// </summary>
		bool IsInitialized { get; }

		/// <summary>
		/// Performs SDK initialization.
		/// </summary>
		AsyncRequest InitializeAsync();

		/// <summary>
		/// Initializes avatar and prepares for calculation.
		/// </summary>
		/// <param name="photoBytes">Photo bytes (jpg or png encoded).</param>
		/// <param name="name">Name of the avatar</param>
		/// <param name="description">Description of the avatar</param>
		/// <param name="pipeline">Calculation pipeline to use, see PipelineType</param>
		/// <param name="computationParams">Computation parameters that will be applied to this avatar</param>
		/// <returns>Avatar code</returns>
		AsyncRequest<string> InitializeAvatarAsync(byte[] photoBytes, string name, string description, PipelineType pipeline,
			ComputationParameters computationParams = null);

		/// <summary>
		/// Starts and waits while avatar is being calculated.
		/// </summary>
		AsyncRequest StartAndAwaitAvatarCalculationAsync(string avatarCode);

		/// <summary>
		/// Generates additional haircuts for existing avatar.
		/// </summary>
		AsyncRequest GenerateHaircutsAsync(string avatarCode, List<string> haircutsList);

		/// <summary>
		/// Moves files from the server to local storage if it is required.
		/// </summary>
		AsyncRequest MoveAvatarModelToLocalStorageAsync(string avatarCode, bool withHaircutPointClouds, bool withBlendshapes, MeshFormat format = MeshFormat.PLY);

		/// <summary>
		/// Makes TexturedMesh with generated head.
		/// </summary>
		/// <param name="detailsLevel">Level of mesh details in range [0..3]</param>
		/// <param name="additionalTextureName">Name of the texture that should be applied intead of default</param>
		AsyncRequest<TexturedMesh> GetHeadMeshAsync(string avatarCode, bool withBlendshapes, int detailsLevel = 0, MeshFormat format = MeshFormat.PLY, 
			string additionalTextureName = null);

		/// <summary>
		/// Returns avatar texture by name or standard texture if name isn't specified.
		/// </summary>
		/// <param name="avatarCode">avatar code</param>
		/// <param name="textureName">Texture name or pass null for standard texture.</param>
		AsyncRequest<Texture2D> GetTextureAsync(string avatarCode, string textureName);

		/// <summary>
		/// Returns list with haircut identities available for this avatar.
		/// </summary>
		AsyncRequest<string[]> GetHaircutsIdAsync(string avatarCode);

		/// <summary>
		/// Makes TexturedMesh with haircut.
		/// </summary>
		AsyncRequest<TexturedMesh> GetHaircutMeshAsync(string avatarCode, string haircutId);

		/// <summary>
		/// Returns haircut preview image as bytes array
		/// </summary>
		AsyncRequest<byte[]> GetHaircutPreviewAsync(string avatarCode, string haircutId);

		/// <summary>
		/// Returns list of avatars identities created by the current user.
		/// </summary>
		AsyncRequest<string[]> GetAllAvatarsAsync(int maxItems);

		/// <summary>
		/// Deletes avatar.
		/// </summary>
		AsyncRequest DeleteAvatarAsync(string avatarCode);

		/// <summary>
		/// Checks if the pipeline is supported
		/// </summary>
		AsyncRequest<bool> IsPipelineSupportedAsync(PipelineType pipelineType);

		/// <summary>
		/// Returns requested set of parameters for pipeline.
		/// There are two possible sets.
		/// ALL - All available resources
		/// DEFAULT - default set of resources
		/// </summary>
		AsyncRequest<ComputationParameters> GetParametersAsync(ComputationParametersSubset parametersSubset, PipelineType pipelineType);
	}
}
