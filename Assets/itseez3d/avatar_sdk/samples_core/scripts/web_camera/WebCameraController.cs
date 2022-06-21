/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
* You may not use this file except in compliance with an authorized license
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
* See the License for the specific language governing permissions and limitations under the License.
* Written by Itseez3D, Inc. <support@itseez3D.com>, January 2019
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace ItSeez3D.AvatarSdkSamples.Core.WebCamera
{
#if UNITY_EDITOR || UNITY_STANDALONE_WIN  || UNITY_STANDALONE_OSX
	public class WebCameraController : MonoBehaviour
	{
		WebCamTexture webCamTexture;

		private ImageBuffer imageBuffer = null;

		int capturedImageWidth = 0;
		int capturedImageHeight = 0;

		public event Action OnResolutionChanged;

		public bool StartCapturingWithFrontCamera(int width = 1920, int height = 1080, int fps = 30)
		{
			WebCamDevice cameraDevice = new WebCamDevice();
			WebCamDevice[] devices = WebCamTexture.devices;
			bool existFrontCamera = false;
			for (int i = 0; i < devices.Length; i++)
			{
				if (devices[i].isFrontFacing)
				{
					cameraDevice = devices[i];
					existFrontCamera = true;
					break;
				}
			}
			if (!existFrontCamera)
				cameraDevice = devices[0];

			return StartCapturing(cameraDevice.name, width, height, fps);
		}

		public bool StartCapturing(int width = 1920, int height = 1080, int fps = 30)
		{
			WebCamDevice[] devices = WebCamTexture.devices;
			if (devices == null || devices.Length == 0)
			{
				Debug.LogError("There are no web cameras!");
				return false;
			}

			for (int i = 0; i < devices.Length; i++)
			{
				if (StartCapturing(devices[i].name, width, height, fps))
					return true;
				else
					Debug.LogErrorFormat("Unable ot start camera capturing: {0}", devices[i].name);
			}
			return false;
		}

		public bool StartCapturing(string deviceName, int width = 1920, int height = 1080, int fps = 30)
		{
			if (webCamTexture != null && webCamTexture.isPlaying)
				webCamTexture.Stop();

			webCamTexture = new WebCamTexture(deviceName, width, height, fps);
			webCamTexture.Play();
			if (!webCamTexture.isPlaying)
				return false;

			Debug.LogFormat("Image size: {0}, {1}", webCamTexture.width, webCamTexture.height);

			capturedImageWidth = webCamTexture.width;
			capturedImageHeight = webCamTexture.height;
			imageBuffer = new ImageBuffer(capturedImageWidth, capturedImageHeight, 3);
			return true;
		}

		public void StopCapturing()
		{
			if (IsCapturing)
				webCamTexture.Stop();
		}

		public Texture2D CapturePhoto()
		{
			if (IsExistCapturedFrame())
			{
				Texture2D photoPreview = new Texture2D(ImageWidth, ImageHeight);
				ImageFrame frame = AcquireCapturedFrame();
				photoPreview.SetPixels32(frame.imageData);
				photoPreview.Apply();
				ReleaseFrame(frame);
				return photoPreview;
			}
			else
			{
				return null;
			}
		}

		public void AddCameraFinishListener(Action<bool> listener) { }

		public bool IsCapturing
		{
			get { return webCamTexture != null && webCamTexture.isPlaying; }
		}

		public int ImageWidth { get { return capturedImageWidth; } }

		public int ImageHeight { get { return capturedImageHeight; } }

		public ImageFrame AcquireCapturedFrame()
		{
			ImageFrame capturedFrame = null;
			while (true)
			{
				if (imageBuffer.GetOrWaitForCapturedFrame(out capturedFrame))
					break;
				else
					Debug.LogWarning("Unable to get captured frame");
			}
			return capturedFrame;
		}

		public void ReleaseFrame(ImageFrame frame)
		{
			imageBuffer.PushFrame(frame);
		}

		public bool IsExistCapturedFrame()
		{
			return imageBuffer.IsExistCapturedFrame();
		}

		public WebCamTexture Texture
		{
			get { return webCamTexture; }
		}

		public WebCamDevice[] Devices
		{
			get { return WebCamTexture.devices; }
		}

		public void Update()
		{
			if (IsCapturing && webCamTexture.didUpdateThisFrame)
			{
				if (webCamTexture.width != capturedImageWidth || webCamTexture.height != capturedImageHeight)
				{
					capturedImageWidth = webCamTexture.width;
					capturedImageHeight = webCamTexture.height;
					imageBuffer = new ImageBuffer(capturedImageWidth, capturedImageHeight, 3);
					if (OnResolutionChanged != null)
						OnResolutionChanged();
				}
				ImageFrame frame = imageBuffer.GetFreeFrame();
				webCamTexture.GetPixels32(frame.imageData);
				imageBuffer.PushCapturedFrame(frame);
			}
		}
	}
#else
	public class WebCameraController : MonoBehaviour
	{
		public event Action OnResolutionChanged;

		public bool IsCapturing
		{
			get { return false; }
		}

		public void StopCapturing() { }

		public WebCamDevice[] Devices
		{
			get { return null; }
		}

		public Texture Texture
		{
			get { return null; }
		}

		public bool StartCapturing(string deviceName, int width, int height)
		{
			Debug.LogError("You are running mock version of the WebCameraController!");
			return false;
		}

		public Texture2D CapturePhoto()
		{
			return null;
		}
	}
#endif
}
