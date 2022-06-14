/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
* You may not use this file except in compliance with an authorized license
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
* See the License for the specific language governing permissions and limitations under the License.
* Written by Itseez3D, Inc. <support@avatarsdk.com>, October 2021
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ItSeez3D.AvatarSdk.Core
{
	public class ModelExporter : MonoBehaviour
	{
		public GameObject model;

		public MeshFileFormat meshFormat = MeshFileFormat.OBJ;

		public bool saveBlendshapes = true;

		public bool singleMesh = false;

		public bool extractRoughnessTexture = false;

		[HideInInspector]
		public bool embedTextures = true;

		void Start()
		{
			if (model == null)
				model = gameObject;
		}

		public void ExportModel(string outputDir)
		{
			List<MeshRendererData> meshesData = GetMeshesData();

			if (meshesData.Count > 0)
			{
				MeshConverter meshConverter = new MeshConverter();
				meshConverter.SaveMeshes(outputDir, meshesData.ToArray(), meshFormat, saveBlendshapes, embedTextures, singleMesh, extractRoughnessTexture);
			}
			else
				Debug.LogErrorFormat("There are no meshes to save.");
		}

		public IEnumerator ExportModelAsync(string outputDir)
		{
			List<MeshRendererData> meshesData = GetMeshesData();

			if (meshesData.Count > 0)
			{
				MeshConverter meshConverter = new MeshConverter();
				yield return meshConverter.SaveMeshesAsync(outputDir, meshesData.ToArray(), meshFormat, saveBlendshapes, embedTextures, singleMesh, extractRoughnessTexture);
			}
			else
				Debug.LogErrorFormat("There are no meshes to save.");
		}

		private List<MeshRendererData> GetMeshesData()
		{
			List<MeshRendererData> meshesData = new List<MeshRendererData>();
			SkinnedMeshRenderer[] skineedMeshRenderers = model.GetComponentsInChildren<SkinnedMeshRenderer>();
			if (skineedMeshRenderers != null)
			{
				foreach (SkinnedMeshRenderer meshRenderer in skineedMeshRenderers)
					meshesData.Add(new MeshRendererData(meshRenderer.name, meshRenderer.sharedMesh, meshRenderer.sharedMaterial));
			}

			MeshRenderer[] meshRenderers = model.GetComponentsInChildren<MeshRenderer>();
			if (meshRenderers != null)
			{
				foreach (MeshRenderer meshRenderer in meshRenderers)
				{
					MeshFilter meshFilter = meshRenderer.GetComponent<MeshFilter>();
					if (meshFilter != null)
						meshesData.Add(new MeshRendererData(meshRenderer.name, meshFilter.sharedMesh, meshRenderer.sharedMaterial));
				}
			}
			return meshesData;
		}
	}

#if UNITY_EDITOR
	[UnityEditor.CustomEditor(typeof(ModelExporter))]
	public class ModelExporterEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();

			var modelExporter = (ModelExporter)target;

			if (modelExporter.meshFormat == MeshFileFormat.FBX || modelExporter.meshFormat == MeshFileFormat.GLB || modelExporter.meshFormat == MeshFileFormat.GLTF)
				modelExporter.embedTextures = EditorGUILayout.Toggle("Embed Textures", modelExporter.embedTextures);

			if (modelExporter.model != null)
			{
				if (GUILayout.Button("Export Model"))
				{
					string outputDir = EditorUtility.OpenFolderPanel("Exporting Model", "", "");
					if (!string.IsNullOrEmpty(outputDir))
						modelExporter.ExportModel(outputDir);
				}

#if UNITY_EDITOR_WIN
				if (GUILayout.Button("Create Prefab"))
				{
					string outputDir = EditorUtility.OpenFolderPanel("Creating Prefab", "Assets", "");
					if (!string.IsNullOrEmpty(outputDir))
					{
						string pluginLocationPath = Application.dataPath;
						if (outputDir.StartsWith(pluginLocationPath))
						{
							outputDir = "Assets" + outputDir.Substring(pluginLocationPath.Length);
							AvatarPrefabBuilder.CreateAvatarPrefab(outputDir, modelExporter.model);
						}
						else
							EditorUtility.DisplayDialog("Invalid prefab location", "You should choose directory inside the project.", "OK");
					}
				}
#endif
			}
		}
	}
#endif
}
