/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
* You may not use this file except in compliance with an authorized license
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
* See the License for the specific language governing permissions and limitations under the License.
* Written by Itseez3D, Inc. <support@avatarsdk.com>, April 2017
*/

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace ItSeez3D.AvatarSdk.Core
{
	public static class ObjWriter
	{
		public static void WriteMeshDataToObj(string objFilePath, Vector3[] vertices, int[] triangles, Vector2[] uvMapping, string textureFilename, 
			bool useLeftHandedCoordinates = true)
		{
			StringBuilder sb = new StringBuilder();

			sb.Append("# Created by Avatar SDK\n");
			sb.Append("# Physical units: meters\n");
			if (vertices != null)
				sb.AppendFormat("# Vertices: {0}\n", vertices.Length);
			if (triangles != null)
				sb.AppendFormat("# Faces: {0}\n", triangles.Length / 3);
			if (uvMapping != null)
				sb.AppendFormat("# Texture points: {0}\n", uvMapping.Length);
			sb.AppendFormat("mtllib ./{0}.mtl\n", Path.GetFileNameWithoutExtension(objFilePath));

			if (vertices != null)
			{
				foreach (Vector3 v in vertices)
					sb.AppendFormat("v {0} {1} {2}\n", useLeftHandedCoordinates ? -v.x : v.x, v.y, v.z);
			}

			sb.Append("usemtl material_0\n");

			if (uvMapping != null)
			{
				foreach (Vector2 v in uvMapping)
					sb.AppendFormat("vt {0} {1}\n", v.x, v.y);
			}

			if (triangles != null)
			{
				for (int i = 0; i < triangles.Length; i += 3)
				{
					if (useLeftHandedCoordinates)
						sb.AppendFormat("f {0}/{0} {1}/{1} {2}/{2}\n", triangles[i + 2] + 1, triangles[i + 1] + 1, triangles[i] + 1);
					else
						sb.AppendFormat("f {0}/{0} {1}/{1} {2}/{2}\n", triangles[i] + 1, triangles[i + 1] + 1, triangles[i + 2] + 1);
				}
			}

			string mtlFilePath = string.Format("{0}/{1}.mtl", Path.GetDirectoryName(objFilePath), Path.GetFileNameWithoutExtension(objFilePath));
			SaveMTL(mtlFilePath, new List<string>() { textureFilename });

			sb.Append("# End of File\n");

			File.WriteAllText(objFilePath, sb.ToString());
		}

		private static void SaveMTL(string mtlFilePath, List<string> texturesFilenames)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("# Created by Avatar SDK\n");

			for (int i = 0; i < texturesFilenames.Count; i++)
				AddMaterialToMTL(sb, string.Format("material_{0}", i), texturesFilenames[i]);

			File.WriteAllText(mtlFilePath, sb.ToString());
		}

		private static void AddMaterialToMTL(StringBuilder sb, string materialName, string textureFilename)
		{
			sb.AppendFormat("\nnewmtl {0}\n", materialName);
			sb.AppendFormat("Ka 1.000000 0.000000 0.000000\n");
			sb.AppendFormat("Kd 1.000000 1.000000 1.000000\n");
			sb.AppendFormat("Ks 0.000000 0.000000 0.000000\n");
			sb.AppendFormat("Tr 0.000000\n");
			sb.AppendFormat("illum 0\n");
			sb.AppendFormat("Ns 0.000000\n");

			if (!string.IsNullOrEmpty(textureFilename))
				sb.AppendFormat("map_Kd {0}\n", Path.GetFileName(textureFilename));
		}
	}
}
