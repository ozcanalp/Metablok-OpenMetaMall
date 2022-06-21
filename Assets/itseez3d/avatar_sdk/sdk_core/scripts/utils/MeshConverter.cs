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
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEditor;
using UnityEngine;

namespace ItSeez3D.AvatarSdk.Core
{
	public enum MeshFileFormat
	{
		OBJ,
		FBX,
		GLTF,
		GLB,
		PLY
	}

	public struct MeshRendererData
	{
		public string meshName;
		public Mesh mesh;
		public Material material;

		public MeshRendererData(string meshName, Mesh mesh, Material material)
		{
			this.meshName = meshName;
			this.mesh = mesh;
			this.material = material;
		}
	}

	public class MeshConverter
	{
		[DllImport(DllHelperCore.dll)]
		private static extern IntPtr createMesh(int verticesCount, int facesCount, IntPtr vertices, IntPtr faces, IntPtr uv,
			IntPtr textureDataRGBA, int textureWidth, int textureHeight);

		[DllImport(DllHelperCore.dll)]
		private static extern int addBlendshapeToMesh(IntPtr mesh, string blendshapeName, int verticesCount, IntPtr verticesDeltas);

		[DllImport(DllHelperCore.dll)]
		private static extern int addTextureToMesh(IntPtr mesh, IntPtr textureDataRGBA, int textureWidth, int textureHeight, string textureFilename);

		[DllImport(DllHelperCore.dll)]
		private static extern int releaseMesh(IntPtr mesh);

		[DllImport(DllHelperCore.dll)]
		private static extern int saveMesh(IntPtr mesh, string objModelFile, string mainTextureFilename, [MarshalAs(UnmanagedType.U1)] bool embedTextures);

		[DllImport(DllHelperCore.dll)]
		private static extern int mergeMeshes(IntPtr dstMesh, IntPtr srcMesh);

		public static bool IsExportAvailable
		{
			get
			{
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN || UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
				return true;
#else
				return false;
#endif
			}
		}

		public static bool IsFbxFormatSupported
		{
			get
			{
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN || (UNITY_STANDALONE_OSX && !UNITY_EDITOR_OSX)
				return true;
#else
				return false;
#endif
			}
		}

		public void SaveMesh(string outputDir, string meshName, Mesh mesh, Material material, MeshFileFormat meshFormat, bool saveBlendshapes, bool embedTextures, bool extractRoughness)
		{
			CoroutineUtils.WaitCoroutine(SaveMeshes(outputDir, new MeshRendererData[] { new MeshRendererData(meshName, mesh, material) }, meshFormat, saveBlendshapes, embedTextures, false, extractRoughness, null));
		}

		public AsyncRequest SaveMeshAsync(string outputDir, string meshName, Mesh mesh, Material material, MeshFileFormat meshFormat, bool saveBlendshapes, bool embedTextures, bool extractRoughness)
		{
			var request = new AsyncRequest("Exporting model...");
			AvatarSdkMgr.SpawnCoroutine(SaveMeshes(outputDir, new MeshRendererData[] { new MeshRendererData(meshName, mesh, material) }, meshFormat, saveBlendshapes, embedTextures, false, extractRoughness, request));
			return request;
		}

		public void SaveMeshes(string outputDir, MeshRendererData[] meshesData, MeshFileFormat meshFormat, bool saveBlendshapes, bool embedTextures, bool needToMergeMeshes, bool extractRoughness)
		{
			CoroutineUtils.WaitCoroutine(SaveMeshes(outputDir, meshesData, meshFormat, saveBlendshapes, embedTextures, needToMergeMeshes, extractRoughness, null));
		}

		public AsyncRequest SaveMeshesAsync(string outputDir, MeshRendererData[] meshesData, MeshFileFormat meshFormat, bool saveBlendshapes, bool embedTextures, bool needToMergeMeshes, bool extractRoughness)
		{
			var request = new AsyncRequest("Exporting model...");
			AvatarSdkMgr.SpawnCoroutine(SaveMeshes(outputDir, meshesData, meshFormat, saveBlendshapes, embedTextures, false, extractRoughness, request));
			return request;
		}

		private IEnumerator SaveMeshes(string outputDir, MeshRendererData[] meshesData, MeshFileFormat meshFormat, bool saveBlendshapes, bool embedTextures, bool needToMergeMeshes, bool extractRoughness, 
			AsyncRequest request)
		{
			string errorMessage = string.Empty;
			if (!IsExportAvailable)
				errorMessage = "Export isn't available on current platform.";
			else if (meshFormat == MeshFileFormat.FBX && !IsFbxFormatSupported)
				errorMessage = "FBX format isn't supported on current platform.";

			if (!string.IsNullOrEmpty(errorMessage))
			{
				Debug.LogError(errorMessage);
				if (request != null)
					request.SetError(errorMessage);
				yield break;
			}

			List<IntPtr> nativeMeshes = new List<IntPtr>();
			foreach (MeshRendererData meshData in meshesData)
			{
				IntPtr meshPtr = CreateNativeMeshFromMeshData(meshData, meshData.material != null, saveBlendshapes, extractRoughness);
				if (needToMergeMeshes)
				{
					nativeMeshes.Add(meshPtr);
				}
				else
				{
					string modelFilePath = Path.Combine(outputDir, meshData.meshName, string.Format("{0}{1}", meshData.meshName, MeshFormatToExtension(meshFormat)));
					CoroutineOutVar<int> res = new CoroutineOutVar<int>(-1);
					yield return SaveNativeMeshAsync(meshPtr, modelFilePath, "_MainTex.png", embedTextures, res);
					if (res.value != 0)
						Debug.LogErrorFormat("Unable to save model. Error code: {0}", res);

					releaseMesh(meshPtr);
				}
			}

			if (needToMergeMeshes)
			{
				for (int i = 1; i < nativeMeshes.Count; i++)
					mergeMeshes(nativeMeshes[0], nativeMeshes[i]);

				string modelFilePath = Path.Combine(outputDir, string.Format("{0}{1}", "model", MeshFormatToExtension(meshFormat)));
				string mainTextureFilename = "_MainTex.png";
				CoroutineOutVar<int> res = new CoroutineOutVar<int>(-1);
				yield return SaveNativeMeshAsync(nativeMeshes[0], modelFilePath, mainTextureFilename, embedTextures, res);
				if (res.value != 0)
					Debug.LogErrorFormat("Unable to save model. Error code: {0}", res);
			}

			foreach (IntPtr meshPtr in nativeMeshes)
				releaseMesh(meshPtr);

			if (request != null)
				request.IsDone = true;
		}

		private IEnumerator SaveNativeMeshAsync(IntPtr mesh, string modelFilename, string mainTextureFilename, bool embedTextures, CoroutineOutVar<int> result)
		{
			result.value = -1;
			Thread thread = new Thread(() => 
			{
				result.value = saveMesh(mesh, modelFilename, mainTextureFilename, embedTextures);
			});
			thread.Start();

			while (thread.IsAlive)
				yield return null;
		}

		private unsafe IntPtr CreateNativeMeshFromMeshData(MeshRendererData meshData, bool saveTextures, bool withBlendshapes, bool extractRoughnessFromGloss)
		{
			IntPtr meshPtr = IntPtr.Zero;
			Texture2D mainTexture = saveTextures ? ImageUtils.CopyTexture(meshData.material.mainTexture) : null;

			Vector3[] vertices = meshData.mesh.vertices;
			int[] faces = meshData.mesh.triangles;
			Vector2[] uv = meshData.mesh.uv;

			float[] verticesData = new float[vertices.Length * 3];
			for (int i = 0; i < vertices.Length; i++)
			{
				verticesData[i * 3] = -vertices[i].x;
				verticesData[i * 3 + 1] = vertices[i].y;
				verticesData[i * 3 + 2] = vertices[i].z;
			}

			for (int i=0; i<faces.Length / 3; i++)
			{
				int tmp = faces[i * 3];
				faces[i * 3] = faces[i * 3 + 2];
				faces[i * 3 + 2] = tmp;
			}

			float[] uvData = new float[uv.Length * 2];
			for (int i = 0; i < uv.Length; i++)
			{
				uvData[i * 2] = uv[i].x;
				uvData[i * 2 + 1] = uv[i].y;
			}

			fixed (float* verticesPtr = &verticesData[0])
			{
				fixed (int* facesPtr = &faces[0])
				{
					fixed (float* uvPtr = &uvData[0])
					{
						if (saveTextures)
						{
							Color32[] pixels = mainTexture.GetPixels32();
							fixed (Color32* pixelsPtr = &pixels[0])
							{
								meshPtr = createMesh(vertices.Length, faces.Length / 3, (IntPtr)verticesPtr, (IntPtr)facesPtr, (IntPtr)uvPtr, (IntPtr)pixelsPtr,
									mainTexture.width, mainTexture.height);
							}
						}
						else
						{
							meshPtr = createMesh(vertices.Length, faces.Length / 3, (IntPtr)verticesPtr, (IntPtr)facesPtr, (IntPtr)uvPtr, IntPtr.Zero, 0, 0);
						}
					}
				}
			}

			if (saveTextures)
			{
				UnityEngine.Object.Destroy(mainTexture);
				AddAdditionalTextures(meshPtr, meshData.material, extractRoughnessFromGloss);
			}

			if (withBlendshapes)
			{
				Vector3[] deltaVertices = new Vector3[vertices.Length];
				Vector3[] deltaNormals = new Vector3[vertices.Length];
				Vector3[] deltaTangents = new Vector3[vertices.Length];
				float[] deltaVerticesData = new float[vertices.Length * 3];

				for (int i = 0; i < meshData.mesh.blendShapeCount; i++)
				{
					meshData.mesh.GetBlendShapeFrameVertices(i, 0, deltaVertices, deltaNormals, deltaTangents);

					for (int vertexIdx = 0; vertexIdx < vertices.Length; vertexIdx++)
					{
						deltaVerticesData[vertexIdx * 3] = -deltaVertices[vertexIdx].x;
						deltaVerticesData[vertexIdx * 3 + 1] = deltaVertices[vertexIdx].y;
						deltaVerticesData[vertexIdx * 3 + 2] = deltaVertices[vertexIdx].z;
					}

					fixed (float* deltasPtr = &deltaVerticesData[0])
						addBlendshapeToMesh(meshPtr, meshData.mesh.GetBlendShapeName(i), vertices.Length, (IntPtr)deltasPtr);
				}
			}

			return meshPtr;
		}

		private unsafe void AddAdditionalTextures(IntPtr meshPtr, Material material, bool extractRoughness)
		{
			List<string> mainTextureNames = new List<string>() { "_MainTex", "_BaseMap" };
			string metallicGlossTexName = "_MetallicGlossMap";
			foreach(string textureName in material.GetTexturePropertyNames())
			{
				if (material.HasProperty(textureName) && !mainTextureNames.Contains(textureName))
				{
					Texture texture = material.GetTexture(textureName);
					if (texture != null)
					{
						Texture2D texture2D = ImageUtils.CopyTexture(texture);
						Color32[] pixels = texture2D.GetPixels32();
						fixed (Color32* pixelsPtr = &pixels[0])
						{
							int res = addTextureToMesh(meshPtr, (IntPtr)pixelsPtr, texture2D.width, texture2D.height, textureName + ".png");
							if (res != 0)
								Debug.LogErrorFormat("Unable to add {0} texture to mesh. Error code: {1}", textureName, res);
						}

						if (extractRoughness && textureName == metallicGlossTexName)
						{
							for (int i = 0; i < pixels.Length; i++)
							{
								pixels[i].r = (byte)(255 - pixels[i].a);
								pixels[i].g = (byte)(255 - pixels[i].a);
								pixels[i].b = (byte)(255 - pixels[i].a);
								pixels[i].a = 255;
							}

							fixed (Color32* pixelsPtr = &pixels[0])
							{
								int res = addTextureToMesh(meshPtr, (IntPtr)pixelsPtr, texture2D.width, texture2D.height, "_RoughnessMap.png");
								if (res != 0)
									Debug.LogErrorFormat("Unable to add {0} texture to mesh. Error code: {1}", textureName, res);
							}
						}

						UnityEngine.Object.Destroy(texture2D);
					}
				}
			}
		}

		private string MeshFormatToExtension(MeshFileFormat meshFormat)
		{
			if (meshFormat == MeshFileFormat.OBJ)
				return ".obj";
			if (meshFormat == MeshFileFormat.FBX)
				return ".fbx";
			if (meshFormat == MeshFileFormat.GLTF)
				return ".gltf";
			if (meshFormat == MeshFileFormat.GLB)
				return ".glb";
			if (meshFormat == MeshFileFormat.PLY)
				return ".ply";
			return ".unknown";
		}
	}
}
