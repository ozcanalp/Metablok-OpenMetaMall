/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
* You may not use this file except in compliance with an authorized license
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
* See the License for the specific language governing permissions and limitations under the License.
* Written by Itseez3D, Inc. <support@itseez3D.com>, February 2020
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace ItSeez3D.AvatarSdkSamples.Core.WebCamera
{
	public class WebCameraCapturer : MonoBehaviour
	{
		public RawImage image;
		public AspectRatioFitter aspectRatioFitter;
		public GameObject capturingStateControls;
		public GameObject capturedStateControls;
		public Dropdown camerasDropdown;

		private WebCameraController webCameraController = null;

		private Texture2D capturedTexture = null;

		private string lastSelectedCamera = string.Empty;

		private bool mirroringEnabled = true;
		private Vector3 defaultScale = new Vector3(1f, 1f, 1f);
		private Vector3 mirrorScale = new Vector3(-1f, 1f, 1f);

		public event Action<byte[]> OnPhotoMade = null;

		void Start()
		{
			webCameraController = gameObject.AddComponent<WebCameraController>();
			webCameraController.OnResolutionChanged += ConfigureImageTexture;
			StartCapturing();
		}

		void OnEnable()
		{
			StartCapturing();
		}

		void OnDisable()
		{
			if (webCameraController.IsCapturing)
				webCameraController.StopCapturing();

			image.texture = null;
			image.material.mainTexture = null;
		}

		public void OnTakePhotoButtonClick()
		{
			capturedTexture = webCameraController.CapturePhoto();
			webCameraController.StopCapturing();
			image.texture = capturedTexture;
			image.material.mainTexture = capturedTexture;
			image.rectTransform.localScale = defaultScale;

			capturingStateControls.SetActive(false);
			capturedStateControls.SetActive(true);
		}

		public void OnBackButtonClick()
		{
			gameObject.SetActive(false);
		}

		public void OnRetryButtonClick()
		{
			StartCapturing();
		}

		public void OnOkButtonClick()
		{
			if (OnPhotoMade != null && capturedTexture != null)
				OnPhotoMade(capturedTexture.EncodeToJPG());
			gameObject.SetActive(false);
		}

		public void OnCamerasDropdownItemChanged(int idx)
		{
			if (lastSelectedCamera != camerasDropdown.options[idx].text)
				StartCameraCapturing(camerasDropdown.options[idx].text);
		}

		public void OnMirroringToggleChanged(bool isEnabled)
		{
			mirroringEnabled = isEnabled;
			image.rectTransform.localScale = mirroringEnabled ? mirrorScale : defaultScale;
		}

		private void StartCapturing()
		{
			if (webCameraController == null)
				return;

			capturingStateControls.SetActive(true);
			capturedStateControls.SetActive(false);

			UpdateCamerasDropdown();

			// Try to start last active camera
			if (!string.IsNullOrEmpty(lastSelectedCamera))
			{
				if (StartCameraCapturing(lastSelectedCamera))
				{
					int camIdx = camerasDropdown.options.FindIndex(o => o.text == lastSelectedCamera);
					camerasDropdown.value = camIdx;
					camerasDropdown.RefreshShownValue();
					return;
				}
			}

			for (int i=0; i<webCameraController.Devices.Length; i++)
			{
				if (StartCameraCapturing(webCameraController.Devices[i].name))
				{
					camerasDropdown.value = i;
					camerasDropdown.RefreshShownValue();
					return;
				}
			}

			Debug.LogError("Unable to start camera capturing!");
			gameObject.SetActive(false);
		}

		private bool StartCameraCapturing(string cameraName)
		{
			lastSelectedCamera = cameraName;
			if (webCameraController.StartCapturing(cameraName, 1920, 1080))
			{
				ConfigureImageTexture();
				return true;
			}
			else
			{
				image.texture = null;
				image.material.mainTexture = null;
				Debug.LogErrorFormat("Unable to start {0}", cameraName);
			}
			return false;
		}

		private void ConfigureImageTexture()
		{
			Texture webCamTexture = webCameraController.Texture;
			image.texture = webCamTexture;
			image.material.mainTexture = webCamTexture;
			image.rectTransform.localScale = mirroringEnabled ? mirrorScale : defaultScale;

			aspectRatioFitter.aspectRatio = (float)webCamTexture.width / webCamTexture.height;
		}

		private void UpdateCamerasDropdown()
		{
			WebCamDevice[] devices = webCameraController.Devices;
			if (devices == null)
			{
				camerasDropdown.gameObject.SetActive(false);
				return;
			}

			camerasDropdown.ClearOptions();
			for (int i=0; i<devices.Length; i++)
			{
				camerasDropdown.options.Add(new Dropdown.OptionData(devices[i].name));
			}
			camerasDropdown.gameObject.SetActive(devices.Length > 1);
		}
	}
}
