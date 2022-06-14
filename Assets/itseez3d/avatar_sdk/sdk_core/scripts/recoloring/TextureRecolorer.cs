/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
* You may not use this file except in compliance with an authorized license
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
* See the License for the specific language governing permissions and limitations under the License.
* Written by Itseez3D, Inc. <support@avatarsdk.com>, September 2021
*/

using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace ItSeez3D.AvatarSdk.Core
{
	[ExecuteInEditMode]
	public abstract class TextureRecolorer : MonoBehaviour
	{
		public bool enableRecoloring = true;

		public Color color = Color.white;

		public Material recoloringByMaterial;
		public Material materialToRecolor;
		public Texture textureToRecolor;

		public ColorPicker colorPicker;

		protected int recoloringByMaterialHash = 0;
		protected Material materialToDestroy = null;

		protected Color defaultColor = Color.white;

		protected RenderTexture renderTexture = null;

		protected Material targetMaterialToRecolor = null;
		protected ColorPicker targetColorPicker = null;
		protected bool targetEnableRecoloring = true;
		protected Color targetColor = Color.white;

		protected Texture initialMainTexture = null;

		protected bool isResettingColorInProgress = false;
		protected bool disableRecoloringWhenResetting = true;
		protected bool recoloringTemporaryDisabled = false;

		protected string defaultMaterialPath = string.Empty;

		protected string avatarCode = string.Empty;

		protected void Start()
		{
			SetDefaultRecoloringMaterial();

			if (materialToRecolor == null)
				materialToRecolor = FindMaterialToRecolor();

			if (materialToRecolor != targetMaterialToRecolor)
				SetMaterialToRecolor(materialToRecolor);

			if (colorPicker != targetColorPicker)
				SetColorPicker(colorPicker);

			ResetColor();
		}

		private void OnDestroy()
		{
			ResetMaterialMainTexture();

			if (renderTexture != null)
			{
				renderTexture.Release();
				DestroyImmediate(renderTexture);
				renderTexture = null;
			}

			if (materialToDestroy != null)
			{
				DestroyImmediate(materialToDestroy);
				materialToDestroy = null;
			}
		}

		void Update()
		{
			bool forceUpdate = !Application.isPlaying;

			if (materialToRecolor != targetMaterialToRecolor)
				SetMaterialToRecolor(materialToRecolor);

			if (colorPicker != targetColorPicker)
				SetColorPicker(colorPicker);

			if (targetColor != color)
				SetTargetColor(color);

			if (enableRecoloring != targetEnableRecoloring)
			{
				targetEnableRecoloring = enableRecoloring;
				forceUpdate = true;
			}

			UpdateTextures(forceUpdate);
		}

		public void UpdateTextures(bool forceUpdate)
		{
			if (materialToRecolor != null && textureToRecolor != null)
			{
				if (recoloringByMaterial != null)
				{
					if (forceUpdate || recoloringByMaterialHash != recoloringByMaterial.ComputeCRC())
					{
						if (enableRecoloring && !recoloringTemporaryDisabled)
							materialToRecolor.mainTexture = CreateTexture(textureToRecolor.width, textureToRecolor.height, recoloringByMaterial);
						else
							materialToRecolor.mainTexture = textureToRecolor;
						recoloringByMaterialHash = recoloringByMaterial.ComputeCRC();
					}
				}
			}
		}

		// Just updates material without resetting values and initial texture 
		public void UpdateMaterialToRecolor(Material material)
		{
			ResetMaterialMainTexture();
			materialToRecolor = material;
			targetMaterialToRecolor = material;
		}

		public void SetMaterialToRecolor(Material material)
		{
			ResetMaterialMainTexture();

			materialToRecolor = material;
			targetMaterialToRecolor = material;
			if (targetMaterialToRecolor != null)
			{
				initialMainTexture = targetMaterialToRecolor.mainTexture;
				textureToRecolor = targetMaterialToRecolor.mainTexture;
			}
			else
			{
				initialMainTexture = null;
				textureToRecolor = null;
			}

			DetectDefaultColor(avatarCode);

			ResetColor();
		}

		public void SetColorPicker(ColorPicker colorPicker)
		{
			this.colorPicker = colorPicker;
			targetColorPicker = colorPicker;

			if (colorPicker != null)
			{
				colorPicker.SetOnValueChangeCallback(OnColorChange);
				colorPicker.SetOnResetButtonClickCallback(ResetColor);
				colorPicker.Color = targetColor;
			}
		}

		public void ResetColor()
		{
			isResettingColorInProgress = true;
			SetTargetColor(defaultColor);
			isResettingColorInProgress = false;
		}

		public abstract void SetDefaultColor(Color color);

		public abstract void DetectDefaultColor(string avatarCode);

		protected abstract void SetMaterialColor(Color color);

		protected virtual Material FindMaterialToRecolor()
		{
			Renderer meshRenderer = GetComponentInChildren<Renderer>();
			if (meshRenderer != null)
				return meshRenderer.sharedMaterial;
			return null;
		}

		private void SetDefaultRecoloringMaterial()
		{
			Material mat = Resources.Load<Material>(defaultMaterialPath);
			if (mat != null)
			{
				recoloringByMaterial = Instantiate(mat);
				materialToDestroy = recoloringByMaterial;
			}
		}

		private void SetTargetColor(Color color)
		{
			if (colorPicker != null)
				colorPicker.Color = color;
			else
				OnColorChange(color);
		}

		private void OnColorChange(Color color)
		{
			recoloringTemporaryDisabled = disableRecoloringWhenResetting && isResettingColorInProgress;
			targetColor = color;
			this.color = targetColor;
			SetMaterialColor(targetColor);
			UpdateTextures(true);
		}

		private Texture CreateTexture(int textureWidth, int textureHeight, Material material)
		{
			int mipmapsCount = 12;

			if (renderTexture == null)
			{
				renderTexture = new RenderTexture(textureWidth, textureHeight, 0, GraphicsFormat.R8G8B8A8_UNorm, mipmapsCount);
				renderTexture.filterMode = FilterMode.Point;
				renderTexture.autoGenerateMips = false;
				renderTexture.useMipMap = true;
			}

			RenderTexture backup = RenderTexture.active;
			RenderTexture.active = renderTexture;

			GL.Clear(true, true, new Color(1.0f, 0.0f, 0.0f));
			GL.PushMatrix();
			GL.LoadPixelMatrix(0, textureWidth, textureHeight, 0);

			Graphics.DrawTexture(new Rect(0, 0, textureWidth, textureHeight), textureToRecolor, material);
			renderTexture.GenerateMips();

			GL.PopMatrix();
			RenderTexture.active = backup;

			return renderTexture;
		}

		private void ResetMaterialMainTexture()
		{
			if (targetMaterialToRecolor != null)
				targetMaterialToRecolor.mainTexture = initialMainTexture;
		}
	}

#if UNITY_EDITOR
	public class TextureRecoloringEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();

			var textureRecoloring = (TextureRecolorer)target;
			if (textureRecoloring.materialToRecolor != null && textureRecoloring.recoloringByMaterial != null)
			{
				if (GUILayout.Button("Save recolored texture"))
				{
					SaveTexture(textureRecoloring.materialToRecolor.mainTexture);
				}
			}
		}

		private void SaveTexture(Texture texture)
		{
			var destRenderTexture = RenderTexture.GetTemporary(texture.width, texture.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);

			Graphics.Blit(texture, destRenderTexture);

			var exportTexture = new Texture2D(texture.width, texture.height, GraphicsFormat.B8G8R8A8_SRGB, TextureCreationFlags.MipChain);
			exportTexture.ReadPixels(new Rect(0, 0, destRenderTexture.width, destRenderTexture.height), 0, 0);
			exportTexture.Apply();

			var textureFilePath = EditorUtility.SaveFilePanel("Save texture as JPG", "", "", "jpg");

			if (textureFilePath.Length != 0)
			{
				byte[] textureBytes = exportTexture.EncodeToJPG(95);
				File.WriteAllBytes(textureFilePath, textureBytes);
			}

			RenderTexture.ReleaseTemporary(destRenderTexture);
			if (Application.isEditor)
			{
				GameObject.DestroyImmediate(exportTexture);
			}
			else
			{
				GameObject.Destroy(exportTexture);
			}
		}
	}
#endif
}
