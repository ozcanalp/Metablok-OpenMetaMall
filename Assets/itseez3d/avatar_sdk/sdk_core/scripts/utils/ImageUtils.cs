/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
* You may not use this file except in compliance with an authorized license
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
* See the License for the specific language governing permissions and limitations under the License.
* Written by Itseez3D, Inc. <support@avatarsdk.com>, June 2019
*/

using ExifLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace ItSeez3D.AvatarSdk.Core
{
	/// <summary>
	/// This class's goal is to struggle problems with Unity's Texture2D usage in separate thread.
	/// </summary>
	public class ImageWrapper
	{
		public const int NumberOfChannels = 4;
		public int Stride
		{
			/* There are no strides in Unity */
			get
			{
				int stride = Width * NumberOfChannels;
				/*if (stride % 4 != 0)
				{
					stride += (4 - (stride % 4));
				}*/
				return stride;
			}
		}

		public bool IsEqualSize(ImageWrapper img)
		{
			return (this.Width == img.Width && this.Height == img.Height);
		}

		public ImageWrapper(Texture2D texture)
		{
			Width = texture.width;
			Height = texture.height;
			if (texture.format == TextureFormat.RGBA32)
			{
				Data32 = texture.GetPixels32();
			}
			else
			{
				Texture2D temp = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);
				temp.SetPixels32(texture.GetPixels32());
				temp.Apply();
				Data32 = temp.GetPixels32();
			}
		}

		public ImageWrapper(int w, int h)
		{
			Width = w;
			Height = h;
			Data32 = new Color32[w * h];
		}

		/// <summary>
		/// Full copy
		/// </summary>
		/// <param name="src"></param>
		public ImageWrapper(ImageWrapper src)
		{
			Width = src.Width;
			Height = src.Height;
			Data32 = src.Data32 == null ? null : (Color32[])src.Data32.Clone();
			Data = src.Data == null ? null : (byte[])src.Data.Clone();
		}

		public Texture2D ToTexture2D()
		{
			var texture = new Texture2D(Width, Height, TextureFormat.RGBA32, false);
			if (Data32 != null)
			{
				texture.SetPixels32(Data32);
			}
			else
			{
				texture.LoadRawTextureData(Data);
			}
			texture.Apply();
			return texture;
		}

		public void Resize(int w, int h)
		{
			Width = w;
			Height = h;
			if (Data32 != null)
			{
				Data32 = new Color32[w * h];
			}
			if (Data != null)
			{
				Data = new byte[w * h * NumberOfChannels];
			}
		}

		public bool TryCopyData(ImageWrapper from)
		{
			if (IsEqualSize(from))
			{
				from.Data32.CopyTo(this.Data32, 0);
				return true;
			}
			else
			{
				return false;
			}
		}
		public Color32[] Data32 { get; set; }
		public byte[] Data { get; set; }
		public int Width;
		public int Height;
	}

	public static class ImageUtils
	{
		public static void ResizeImagePyramidal(ImageWrapper srcImage, ImageWrapper dstImage)
		{
			const double pyrScale = 0.5;
			ImageWrapper resizedImage = new ImageWrapper(srcImage);

			while (Convert.ToInt32(resizedImage.Width * pyrScale) > dstImage.Width &&
				   Convert.ToInt32(resizedImage.Height * pyrScale) > dstImage.Height)
			{
				Interpolation.BicubicInterpolation(resizedImage, resizedImage, Convert.ToInt32(resizedImage.Width * pyrScale), Convert.ToInt32(resizedImage.Height * pyrScale));
			}
			Interpolation.BicubicInterpolation(resizedImage, dstImage);
		}

		public static AsyncRequest<byte[]> DownscaleImageIfNeedAsync(byte[] srcImageBytes)
		{
			var request = new AsyncRequest<byte[]>("Rescaling image");
			AvatarSdkMgr.SpawnCoroutine(DownscaleImageIfNeedFunc(srcImageBytes, request));
			return request;
		}

		public static AsyncRequest<ImageWrapper> DownscaleImageAsync(byte[] srcImgBuffer, int minSide)
		{
			Texture2D texture = new Texture2D(1, 1);
			texture.LoadImage(srcImgBuffer);
			int minDim = Math.Min(texture.width, texture.height);

			int textureWidth = texture.width;
			int textureHeight = texture.height;

			ImageWrapper srcImg = new ImageWrapper(texture);
			Func<ImageWrapper> scaleFunc = () =>
			{
				if (minDim > minSide)
				{
					float scale = minSide / (float)minDim;
					ImageWrapper dstImg = new ImageWrapper(Convert.ToInt32(textureWidth * scale), Convert.ToInt32(textureHeight * scale));
					ResizeImagePyramidal(srcImg, dstImg);
					return dstImg;
				}
				else
				{
					return null;
				}
			};
			AsyncRequest<ImageWrapper> request = new AsyncRequestThreaded<ImageWrapper>(() => scaleFunc(), "Resampling");
			AvatarSdkMgr.SpawnCoroutine(request.Await());
			return request;
		}

		public static void RecolorTexture(Texture2D texture, Color color)
		{
			Color averageColor = CoreTools.CalculateAverageColor(texture);
			Vector4 tint = CoreTools.CalculateTint(color, averageColor);
			RecolorTexture(texture, color, tint);
		}

		public static void RecolorTexture(Texture2D texture, Color color, Vector4 tint)
		{
			Color[] pixels = texture.GetPixels();
			float threshold = 0.2f, tintCoeff = 0.8f;  // should be the same as in the shader
			for (int i = 0; i < pixels.Length; ++i)
			{
				Color tinted = pixels[i] + tintCoeff * new Color(tint.x, tint.y, tint.z);
				float maxTargetChannel = Math.Max(color.r, Math.Max(color.g, color.b));
				if (maxTargetChannel < threshold)
				{
					float darkeningCoeff = Math.Min(0.85f, (threshold - maxTargetChannel) / threshold);
					tinted = (1.0f - darkeningCoeff) * tinted + darkeningCoeff * (color * pixels[i]);
				}
				pixels[i].r = tinted.r;
				pixels[i].g = tinted.g;
				pixels[i].b = tinted.b;
			}
			texture.SetPixels(pixels);
			texture.Apply();
		}

		/// <summary>
		/// Recolor texture and create Texture2D object.
		/// </summary>
		public static Texture2D RecolorTexture(string srcTextureFile, Color color, Vector4 tint)
		{
			byte[] bytes = File.ReadAllBytes(srcTextureFile);
			Texture2D texture = new Texture2D(2, 2);
			texture.LoadImage(bytes);
			RecolorTexture(texture, color, tint);
			return texture;
		}

		/// <summary>
		/// Recolor texture and save it.
		/// </summary>
		public static void RecolorTexture(string srcTextureFile, string dstTextureFile, Color color, Vector4 tint)
		{
			Texture2D texture = RecolorTexture(srcTextureFile, color, tint);
			SaveTextureToFile(texture, dstTextureFile);
		}

		/// <summary>
		/// Save texture to file
		/// </summary>
		public static void SaveTextureToFile(Texture2D texture, string textureFilePath)
		{
			byte[] textureBytes = null;
			string extension = Path.GetExtension(textureFilePath).ToLower();
			if (extension == ".png")
				textureBytes = texture.EncodeToPNG();
			else if (extension == ".jpg" || extension == ".jpeg")
				textureBytes = texture.EncodeToJPG(95);
			else
			{
				Debug.LogErrorFormat("Unable to save recolored texture. Invalid file extension: {0}", textureFilePath);
				return;
			}

			File.WriteAllBytes(textureFilePath, textureBytes);
		}

		public static Texture2D CopyTexture(Texture srcTexture)
		{
			RenderTexture dstRenderTexture = RenderTexture.GetTemporary(srcTexture.width, srcTexture.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);

			Graphics.Blit(srcTexture, dstRenderTexture);

			var dstTexture = new Texture2D(srcTexture.width, srcTexture.height);
			dstTexture.ReadPixels(new Rect(0, 0, dstRenderTexture.width, dstRenderTexture.height), 0, 0);
			dstTexture.Apply();

			RenderTexture.ReleaseTemporary(dstRenderTexture);

			return dstTexture;
		}

		public static void ExportTexture(Texture2D texture, string textureFilePath)
		{
			Texture2D exportTexture = CopyTexture(texture);

			byte[] textureBytes = null;
			string extension = Path.GetExtension(textureFilePath).ToLower();
			if (extension == ".png")
				textureBytes = exportTexture.EncodeToPNG();
			else if (extension == ".jpg" || extension == ".jpeg")
				textureBytes = exportTexture.EncodeToJPG(95);

			File.WriteAllBytes(textureFilePath, textureBytes);

			if (Application.isEditor)
			{
				UnityEngine.Object.DestroyImmediate(exportTexture);
			}
			else
			{
				UnityEngine.Object.Destroy(exportTexture);
			}
		}

		public static Texture2D FixExifRotation(Texture2D inText, byte[] image)
		{
			var jpeg = ExifReader.ReadJpeg(image, "image");
			if (!jpeg.IsValid)
			{
				return inText;
			}
			int orientation = (int)jpeg.Orientation;
			return Rotate(inText, orientation);
		}

		public static Texture2D Rotate(Texture2D txt, int exifValue)
		{
			Texture2D dstTexture = null;
			Color[] srcPxcel = txt.GetPixels();
			Color[] dstPxcel = txt.GetPixels();

			if (new List<int>() { 5, 6, 7, 8 }.Contains(exifValue))
			{
				dstTexture = new Texture2D(txt.height, txt.width);
				for (int i = 0; i < txt.height; i++)
				{
					for (int j = 0; j < txt.width; j++)
					{
						if (exifValue == 8)
						{
							dstPxcel[(txt.height - i - 1) + j * txt.height] = srcPxcel[j + i * txt.width];
						}
						else if (exifValue == 7)
						{
							dstPxcel[i + j * txt.height] = srcPxcel[j + i * txt.width];
						}
						else if (exifValue == 5)
						{
							dstPxcel[(txt.height - i - 1) + (txt.width - j - 1) * txt.height] = srcPxcel[j + i * txt.width];
						}
						else if (exifValue == 6)
						{
							dstPxcel[i + (txt.width - j - 1) * txt.height] = srcPxcel[j + i * txt.width];
						}
					}
				}
			}
			else
			{
				dstTexture = new Texture2D(txt.width, txt.height);
				for (int i = 0; i < txt.height; i++)
				{
					for (int j = 0; j < txt.width; j++)
					{
						if (exifValue == 2)
						{
							dstPxcel[i * txt.width + j] = srcPxcel[i * txt.width + (txt.width - j - 1)];
						}
						else if (exifValue == 3)
						{
							dstPxcel[i * txt.width + j] = srcPxcel[(txt.height - i - 1) * txt.width + (txt.width - j - 1)];
						}
						else if (exifValue == 4)
						{
							dstPxcel[i * txt.width + j] = srcPxcel[(txt.height - i - 1) * txt.width + j];
						}

					}
				}
			}
			dstTexture.SetPixels(dstPxcel);
			dstTexture.Apply();

			return dstTexture;
		}

		public static Texture2D LoadTexture(string textureFilename)
		{
			Texture2D texture = new Texture2D(0, 0);
			texture.LoadImage(File.ReadAllBytes(textureFilename));
			return texture;
		}

		public static AsyncRequest<Texture2D> LoadTextureAsync(string textureFilename)
		{
			var request = new AsyncRequest<Texture2D>();
			AvatarSdkMgr.SpawnCoroutine(LoadTextureFunc(textureFilename, request));
			return request;
		}

		public static AsyncRequest<Texture2D> LoadNormalMapTextureAsync(string textureFilename)
		{
			var request = new AsyncRequest<Texture2D>();
			AvatarSdkMgr.SpawnCoroutine(LoadNormalMapTextureFunc(textureFilename, request));
			return request;
		}

		private static bool? isDXT5Encoding = null;
		public static bool IsDXT5EncodingUsedForNormalMap()
		{
			if (isDXT5Encoding != null)
				return isDXT5Encoding.Value;

			Texture2D sampleTexture = Resources.Load<Texture2D>("textures/avatar_sdk_sample_normal_map");
			if (sampleTexture != null)
			{
				Color32[] pixels = sampleTexture.GetPixels32();
				isDXT5Encoding = pixels[0].r == 255;
			}
			else
			{
				Debug.LogError("Unable to find sample normal map texture -> normal map encoding isn't detected");
				isDXT5Encoding = false;
			}
			return isDXT5Encoding.Value;
		}

		public static void ConvertXYZtoDXT5(Color32[] pixels)
		{
			for (int i = 0; i < pixels.Length; i++)
			{
				pixels[i].a = pixels[i].r;
				pixels[i].r = 255;
				pixels[i].b = pixels[i].g;
			}
		}

		private static IEnumerator LoadTextureFunc(string textureFilename, AsyncRequest<Texture2D> request)
		{
			var textureBytesRequest = FileLoader.LoadFileAsync(textureFilename);
			yield return request.AwaitSubrequest(textureBytesRequest, 0.9f);
			if (request.IsError)
				yield break;

			Texture2D texture2D = new Texture2D(2, 2);
			texture2D.LoadImage(textureBytesRequest.Result);

			request.Result = texture2D;
			request.IsDone = true;
		}

		private static IEnumerator LoadNormalMapTextureFunc(string textureFilename, AsyncRequest<Texture2D> request)
		{
			var textureBytesRequest = FileLoader.LoadFileAsync(textureFilename);
			yield return request.AwaitSubrequest(textureBytesRequest, 0.9f);
			if (request.IsError)
				yield break;

			Texture2D texture = new Texture2D(2, 2);
			texture.LoadImage(textureBytesRequest.Result);

			if (texture == null)
			{
				Debug.LogErrorFormat("Unable to load texture: {0}", textureFilename);
			}
			else
			{
				Texture2D normalMapTexture = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, true, true);
				Color32[] pixels = texture.GetPixels32();
				if (IsDXT5EncodingUsedForNormalMap())
				{
					yield return null;
					ConvertXYZtoDXT5(pixels);
					yield return null;
				}

				normalMapTexture.SetPixels32(pixels);
				normalMapTexture.Apply();
				request.Result = normalMapTexture;
				UnityEngine.Object.DestroyImmediate(texture);
			}
			request.IsDone = true;
		}

		private const int IMAGE_SIZE_LIMIT = 960;
		/// <summary>
		/// Check if image needs to be downscaled and execute scaling if need
		/// </summary>
		/// <param name="srcImageBytes">Image to check</param>
		/// <returns>Image downscaled (if need, source image otherwise)</returns>
		private static IEnumerator DownscaleImageIfNeedFunc(byte[] srcImageBytes, AsyncRequest<byte[]> request)
		{
			var downscaleRequest = ImageUtils.DownscaleImageAsync(srcImageBytes, IMAGE_SIZE_LIMIT);
			yield return request.AwaitSubrequest(downscaleRequest, 0.9f);
			if (downscaleRequest.Result != null)
			{
				request.Result = downscaleRequest.Result.ToTexture2D().EncodeToJPG();
			}
			else
			{
				request.Result = srcImageBytes;
			}
			request.IsDone = true;
		}
	}
}
