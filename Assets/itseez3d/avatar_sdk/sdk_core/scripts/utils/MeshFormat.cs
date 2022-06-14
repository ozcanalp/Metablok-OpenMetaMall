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
using UnityEngine;

namespace ItSeez3D.AvatarSdk.Core
{
	public enum MeshFormat
	{
		PLY,
		OBJ,
		FBX,
		GLTF,
		GLB,
		BIN
	}

	public static class MeshFormatExtensions
	{
		public static string MeshFormatToStr(this MeshFormat format)
		{
			switch (format)
			{
				case MeshFormat.BIN: return "bin";
				case MeshFormat.FBX: return "fbx";
				case MeshFormat.PLY: return "ply";
				case MeshFormat.GLTF: return "gltf";
				case MeshFormat.GLB: return "glb";
				case MeshFormat.OBJ: return "obj";
				default: return "unknown";
			}
		}

		public static MeshFormat MeshFormatFromStr(string str)
		{
			str = str.ToLower();
			if (str == "bin")
				return MeshFormat.BIN;
			if (str == "fbx")
				return MeshFormat.FBX;
			if (str == "ply")
				return MeshFormat.PLY;
			if (str == "gltf")
				return MeshFormat.GLTF;
			if (str == "glb")
				return MeshFormat.GLB;
			if (str == "obj")
				return MeshFormat.OBJ;

			Debug.LogErrorFormat("Unable to convert string '{0}' to mesh format.", str);
			return MeshFormat.PLY;
		}
	}

}