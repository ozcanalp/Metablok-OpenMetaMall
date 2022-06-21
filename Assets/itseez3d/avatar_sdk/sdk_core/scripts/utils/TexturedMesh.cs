/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
* You may not use this file except in compliance with an authorized license
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
* See the License for the specific language governing permissions and limitations under the License.
* Written by Itseez3D, Inc. <support@avatarsdk.com>, April 2017
*/

using System.IO;
using UnityEngine;

namespace ItSeez3D.AvatarSdk.Core
{
	/// <summary>
	/// Combines Unity mesh and it's texture.
	/// </summary>
	public class TexturedMesh
	{
		public Mesh mesh;
		public Texture2D texture;

		public TexturedMesh() { }

		public TexturedMesh(Mesh mesh, Texture2D texture)
		{
			this.mesh = mesh;
			this.texture = texture;
		}

		public TexturedMesh(Mesh mesh, string textureFilePath)
		{
			this.mesh = mesh;
			texture = new Texture2D(0, 0);
			byte[] textureBytes = File.ReadAllBytes(textureFilePath);
			texture.LoadImage(textureBytes);
		}
	}
}

