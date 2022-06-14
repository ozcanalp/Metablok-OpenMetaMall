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
using System.IO;
using UnityEngine;

namespace ItSeez3D.AvatarSdk.Core
{
	/// <summary>
	/// Helper class to load various avatar files
	/// </summary>
	public static class FileLoader
	{
		/// <summary>
		/// Loads file asynchronously in a separate thread.
		/// </summary>
		/// <param name="path">Absolute path to file.</param>
		public static AsyncRequest<byte[]> LoadFileAsync(string path)
		{
			var request = new AsyncRequestThreaded<byte[]>(() => File.ReadAllBytes(path), Strings.LoadingFiles);
			AvatarSdkMgr.SpawnCoroutine(request.Await());
			return request;
		}

		/// <summary>
		/// Loads the avatar file asynchronously.
		/// </summary>
		/// <param name="code">Avatar unique code.</param>
		/// <param name="file">File type (e.g. head texture).</param>
		/// <param name="levelOfDetails">Level of details</param>
		public static AsyncRequest<byte[]> LoadAvatarFileAsync(string code, AvatarFile file, int levelOfDetails = 0)
		{
			try
			{
				var filename = AvatarSdkMgr.Storage().GetAvatarFilename(code, file, levelOfDetails);
				return LoadFileAsync(filename);
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
				var request = new AsyncRequest<byte[]>();
				request.SetError(string.Format("Could not load {0}, reason: {1}", file, ex.Message));
				return request;
			}
		}



		/// <summary>
		/// Loads the avatar haircut points file asynchronously.
		/// </summary>
		/// <param name="code">Avatar unique code.</param>
		/// <param name="haircutId">Unique ID of a haircut.</param>
		public static AsyncRequest<byte[]> LoadAvatarHaircutPointcloudFileAsync(string code, string haircutId)
		{
			try
			{
				var filename = HaircutsPersistentStorage.Instance.GetHaircutMetadata(haircutId, code).PathToPointCloud;
				return LoadFileAsync(filename);
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
				var request = new AsyncRequest<byte[]>();
				request.SetError(string.Format("Could not load haircut {0} point cloud, reason: {1}", haircutId, ex.Message));
				return request;
			}
		}
	}
}
