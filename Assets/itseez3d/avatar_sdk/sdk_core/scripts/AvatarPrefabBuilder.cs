/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
* You may not use this file except in compliance with an authorized license
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
* See the License for the specific language governing permissions and limitations under the License.
* Written by Itseez3D, Inc. <support@avatarsdk.com>, April 2017
*/

#if UNITY_EDITOR
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using System;

namespace ItSeez3D.AvatarSdk.Core
{
	public static class AvatarPrefabBuilder
	{
		public static void CreateAvatarPrefab(string prefabDir, GameObject avatarObject)
		{
			PluginStructure.CreatePluginDirectory(prefabDir);
			GameObject instantiatedAvatarObject = UnityEngine.Object.Instantiate(avatarObject);

			SkinnedMeshRenderer[] skineedMeshRenderers = instantiatedAvatarObject.GetComponentsInChildren<SkinnedMeshRenderer>();
			if (skineedMeshRenderers != null)
			{
				foreach (SkinnedMeshRenderer meshRenderer in skineedMeshRenderers)
				{
					meshRenderer.sharedMesh = SaveMeshAsset(prefabDir, meshRenderer.name, meshRenderer.sharedMesh);

					meshRenderer.sharedMaterial = UnityEngine.Object.Instantiate(meshRenderer.sharedMaterial);
					SaveMaterialTextures(meshRenderer.sharedMaterial, Path.Combine(prefabDir, meshRenderer.name), string.Empty);
					AssetDatabase.CreateAsset(meshRenderer.sharedMaterial, Path.Combine(prefabDir, meshRenderer.name, "model_material.mat"));
				}
			}

			MeshRenderer[] meshRenderers = instantiatedAvatarObject.GetComponentsInChildren<MeshRenderer>();
			if (meshRenderers != null)
			{
				foreach (MeshRenderer meshRenderer in meshRenderers)
				{
					MeshFilter meshFilter = meshRenderer.GetComponent<MeshFilter>();
					meshFilter.sharedMesh = SaveMeshAsset(prefabDir, meshRenderer.name, meshFilter.sharedMesh);

					meshRenderer.sharedMaterial = UnityEngine.Object.Instantiate(meshRenderer.sharedMaterial);
					SaveMaterialTextures(meshRenderer.sharedMaterial, Path.Combine(prefabDir, meshRenderer.name), string.Empty);
					AssetDatabase.CreateAsset(meshRenderer.sharedMaterial, Path.Combine(prefabDir, meshRenderer.name, "model_material.mat"));
				}
			}

			var objectsToRemove = instantiatedAvatarObject.GetComponents<MonoBehaviour>();
			foreach (var obj in objectsToRemove)
			{
				UnityEngine.Object.DestroyImmediate(obj);
			}

			CopyBlendshapesWeights(avatarObject, instantiatedAvatarObject);

			PrefabUtility.SaveAsPrefabAsset(instantiatedAvatarObject, prefabDir + "/avatar.prefab");

			UnityEngine.Object.DestroyImmediate(instantiatedAvatarObject);
			EditorUtility.DisplayDialog("Prefab created successfully!", string.Format("You can find your prefab in '{0}' folder", prefabDir), "Ok");
		}

		private static Mesh SaveMeshAsset(string outputDir, string meshName, Mesh mesh)
		{
			MeshConverter meshConverter = new MeshConverter();
			meshConverter.SaveMesh(outputDir, meshName, mesh, null, MeshFileFormat.FBX, true, false, false);
			AssetDatabase.Refresh();

			string modelDirPath = Path.Combine(outputDir, meshName);
			string fbxPath = Path.Combine(modelDirPath, meshName + ".fbx");
			ModelImporter modelImporter = ModelImporter.GetAtPath(fbxPath) as ModelImporter;
			modelImporter.isReadable = true;
			modelImporter.normalCalculationMode = ModelImporterNormalCalculationMode.Unweighted;
			modelImporter.SaveAndReimport();

			return AssetDatabase.LoadAssetAtPath<Mesh>(fbxPath);
		}

		private static void SaveMaterialTextures(Material material, string outputDir, string textureNamePrefix)
		{
			string[] texturesNames = material.GetTexturePropertyNames();
			foreach (string textureName in texturesNames)
			{
				Texture texture = material.GetTexture(textureName);
				if (texture != null)
				{
					string textureAssetPath = AssetDatabase.GetAssetPath(texture);
					if (string.IsNullOrEmpty(textureAssetPath))
					{
						Texture2D textureCopy = ImageUtils.CopyTexture(material.GetTexture(textureName));
						string extension = textureCopy.format == TextureFormat.RGB24 ? "jpg" : "png";
						textureAssetPath = Path.Combine(outputDir, string.Format("{0}{1}.{2}", textureNamePrefix, textureName, extension));
						Texture2D savedTexture = SaveTextureAsset(textureCopy, textureAssetPath, textureName.Contains("BumpMap"));
						material.SetTexture(textureName, savedTexture);
						UnityEngine.Object.Destroy(textureCopy);
					}
				}
			}
		}

		private static Texture2D SaveTextureAsset(Texture2D texture, string texturePath, bool isNormalMap)
		{
			ImageUtils.ExportTexture(texture, texturePath);
			AssetDatabase.Refresh();

			TextureImporter textureImporter = (TextureImporter)TextureImporter.GetAtPath(texturePath);
			textureImporter.isReadable = true;
			if (isNormalMap)
				textureImporter.textureType = TextureImporterType.NormalMap;
			textureImporter.SaveAndReimport();

			return (Texture2D)AssetDatabase.LoadAssetAtPath(texturePath, typeof(Texture2D));
		}

		private static void CopyBlendshapesWeights(GameObject srcAvatarObject, GameObject dstAvatarObject)
		{
			SkinnedMeshRenderer[] srcMeshRenderers = srcAvatarObject.GetComponentsInChildren<SkinnedMeshRenderer>();
			SkinnedMeshRenderer[] dstMeshRenderers = dstAvatarObject.GetComponentsInChildren<SkinnedMeshRenderer>();

			if (srcMeshRenderers != null && dstMeshRenderers != null)
			{
				foreach (SkinnedMeshRenderer srcRenderer in srcMeshRenderers)
				{
					SkinnedMeshRenderer dstRenderer = dstMeshRenderers.FirstOrDefault(r => r.name == srcRenderer.name);
					for (int i = 0; i < srcRenderer.sharedMesh.blendShapeCount; i++)
					{
						string blendshapeName = srcRenderer.sharedMesh.GetBlendShapeName(i);
						int idx = dstRenderer.sharedMesh.GetBlendShapeIndex(blendshapeName);
						dstRenderer.SetBlendShapeWeight(idx, srcRenderer.GetBlendShapeWeight(i));
					}
				}
			}
		}
	}
}
#endif
