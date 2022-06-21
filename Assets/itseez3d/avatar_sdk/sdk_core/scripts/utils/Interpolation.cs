/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
* You may not use this file except in compliance with an authorized license
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
* See the License for the specific language governing permissions and limitations under the License.
* Written by Itseez3D, Inc. <support@avatarsdk.com>, June 2019
*/

using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace ItSeez3D.AvatarSdk.Core
{
	public class Interpolation
	{
		class RGB
		{
			public const short R = 0;
			public const short G = 1;
			public const short B = 2;
			public const short A = 3;
		}

		private static double BiCubicKernel(double x)
		{
			if (x < 0)
			{
				x = -x;
			}

			double biCoef = 0;

			if (x <= 1)
			{
				biCoef = (1.5 * x - 2.5) * x * x + 1;
			}
			else if (x < 2)
			{
				biCoef = ((-0.5 * x + 2.5) * x - 4) * x + 2;
			}

			return biCoef;
		}


		public static void BicubicInterpolation(ImageWrapper sourceData, ImageWrapper destinationData, int dstWidth = 0, int dstHeight = 0)
		{
			try
			{
				ImageWrapper dummySrc = null;
				ImageWrapper dummyDst = destinationData;
				if (sourceData == destinationData)
				{
					if (dstWidth > 0 && dstHeight > 0)
					{
						dummySrc = new ImageWrapper(sourceData);
						dummyDst.Resize(dstWidth, dstHeight);
					}
					else
					{
						Debug.LogError("Interpolation: Source and desination data are same object and no destination size provided");
						return;
					}
				}
				else if (sourceData.IsEqualSize(destinationData) && (dstWidth <= 0 && dstHeight <= 0))
				{

					destinationData.TryCopyData(sourceData);
					Debug.LogWarning("Interpolation: Target size is same as source");
					return;
				}
				else
				{
					dummySrc = sourceData;
				}
				BicubicInterpolationRGB32(dummySrc, dummyDst);
			}
			catch (Exception ex)
			{
				Debug.LogErrorFormat("Interpolation error: {0}", ex.Message);
				throw ex;
			}
		}

		private static unsafe void BicubicInterpolationRGB32(ImageWrapper sourceData, ImageWrapper destinationData)
		{
			try
			{
				int nChannels = ImageWrapper.NumberOfChannels;
				// get source image size
				int width = sourceData.Width;
				int height = sourceData.Height;

				int newWidth = destinationData.Width;
				int newHeight = destinationData.Height;

				int pixelSize = nChannels;
				int srcStride = sourceData.Stride;
				int dstOffset = destinationData.Stride - pixelSize * newWidth;
				double xFactor = (double)width / newWidth;
				double yFactor = (double)height / newHeight;

				//We start with Color32[] Data32 field for first interation and then proceed with byte Data[]
				if (destinationData.Data == null)
				{
					destinationData.Data = new byte[newHeight * destinationData.Stride];
					destinationData.Data32 = null;
				}

				byte[] srcByteArray = sourceData.Data;
				Color32[] src32Array = sourceData.Data32;

				// do the job
				IntPtr unmanagedDst = Marshal.AllocHGlobal(newHeight * destinationData.Stride);
				byte* dst = (byte*)unmanagedDst.ToPointer();

				// coordinates of source points and cooefficiens
				double ox, oy, dx, dy, k1, k2;
				int ox1, oy1, ox2, oy2;
				// destination pixel values
				double r, g, b, a;
				// width and height decreased by 1
				int ymax = height - 1;
				int xmax = width - 1;
				// temporary pointer
				byte* p;

				// RGB
				for (int y = 0; y < newHeight; y++)
				{
					// Y coordinates
					oy = (double)y * yFactor - 0.5f;
					oy1 = (int)oy;
					dy = oy - (double)oy1;

					for (int x = 0; x < newWidth; x++, dst += 4)
					{
						// X coordinates
						ox = (double)x * xFactor - 0.5f;
						ox1 = (int)ox;
						dx = ox - (double)ox1;

						// initial pixel value
						r = g = b = a = 0;

						for (int n = -1; n < 3; n++)
						{
							// get Y cooefficient
							k1 = Interpolation.BiCubicKernel(dy - (double)n);

							oy2 = oy1 + n;
							if (oy2 < 0)
								oy2 = 0;
							if (oy2 > ymax)
								oy2 = ymax;

							for (int m = -1; m < 4; m++)
							{
								// get X cooefficient
								k2 = k1 * Interpolation.BiCubicKernel((double)m - dx);

								ox2 = ox1 + m;
								if (ox2 < 0)
									ox2 = 0;
								if (ox2 > xmax)
									ox2 = xmax;

								if (srcByteArray == null)
								{
									fixed (Color32* src32ArrayPtr = src32Array)
									{
										IntPtr src32ArrayIntPtr = (IntPtr)src32ArrayPtr;
										byte* src = (byte*)src32ArrayIntPtr.ToPointer();
										// get pixel of original image
										p = src + oy2 * srcStride + ox2 * 4;
									}
								}
								else
								{
									fixed (byte* src = srcByteArray)
									{
										// get pixel of original image
										p = src + oy2 * srcStride + ox2 * 4;
									}
								}

								r += k2 * p[RGB.R];
								g += k2 * p[RGB.G];
								b += k2 * p[RGB.B];
								a += k2 * p[RGB.A];
							}
						}

						dst[RGB.R] = (byte)Math.Max(0, Math.Min(255, r));
						dst[RGB.G] = (byte)Math.Max(0, Math.Min(255, g));
						dst[RGB.B] = (byte)Math.Max(0, Math.Min(255, b));
						dst[RGB.A] = (byte)Math.Max(0, Math.Min(255, a));
					}
					dst += dstOffset;
				}
				Marshal.Copy(unmanagedDst, destinationData.Data, 0, destinationData.Data.Length);
				Marshal.FreeHGlobal(unmanagedDst);
				unmanagedDst = IntPtr.Zero;
			}
			catch (Exception ex)
			{
				Debug.LogErrorFormat("Bicubic interpolation exception: {0}", ex.Message);
				throw ex;
			}
		}
	}
}
