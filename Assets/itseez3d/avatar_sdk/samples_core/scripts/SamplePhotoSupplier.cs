/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
* You may not use this file except in compliance with an authorized license
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
* See the License for the specific language governing permissions and limitations under the License.
* Written by Itseez3D, Inc. <support@avatarsdk.com>, June 2020
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ItSeez3D.AvatarSdkSamples.Core
{
	public class SamplePhotoSupplier : MonoBehaviour
	{
		public TextAsset[] malePhotos;
		public TextAsset[] femalePhotos;

		private List<TextAsset> allPhotos = new List<TextAsset>();

		void Start()
		{
			allPhotos.AddRange(malePhotos);
			allPhotos.AddRange(femalePhotos);
		}

		public byte[] GetRandomPhoto()
		{
			var photoIdx = Random.Range(0, allPhotos.Count);
			return allPhotos[photoIdx].bytes;
		}

		public byte[] GetMalePhoto()
		{
			var photoIdx = Random.Range(0, malePhotos.Length);
			return malePhotos[photoIdx].bytes;
		}

		public byte[] GetFemalePhoto()
		{
			var photoIdx = Random.Range(0, femalePhotos.Length);
			return femalePhotos[photoIdx].bytes;
		}
	}
}
