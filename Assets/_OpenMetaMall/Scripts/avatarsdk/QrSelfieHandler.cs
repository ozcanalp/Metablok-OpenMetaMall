/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
* You may not use this file except in compliance with an authorized license
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
* See the License for the specific language governing permissions and limitations under the License.
* Written by Itseez3D, Inc. <support@avatarsdk.com>, February 2021
*/

using ItSeez3D.AvatarSdk.Cloud;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ItSeez3D.AvatarSdkSamples.Cloud
{
	public class QrSelfieHandler : MonoBehaviour
	{
		public GameObject mainObject;

		public Image qrImage;
		public Text statusText;
		public Text captionText;

		private Connection connection = null;

		private Action<bool> completeActionHandler = null;

		public byte[] Selfie { get; private set; }
		public string SelfieCode { get; private set; }

		public void Show(Connection connection, Action<bool> completeHandler)
		{
			Selfie = null;
			SelfieCode = null;
			this.connection = connection;
			completeActionHandler = completeHandler;
			mainObject.SetActive(true);
			qrImage.gameObject.SetActive(false);
			captionText.gameObject.SetActive(false);
			statusText.gameObject.SetActive(true);
			StartCoroutine(Run());
		}

		private void HandleError(string what)
		{
			Selfie = null;
			SelfieCode = null;
			captionText.text = what;
			Debug.LogError(what);
		}

		public void Back()
		{
			Selfie = null;
			SelfieCode = null;

			mainObject.SetActive(false);
			if (completeActionHandler != null)
				completeActionHandler(false);
		}

		bool stopDisplayLoading = false;
		public IEnumerator DisplayLoading()
		{
			stopDisplayLoading = false;
			List<string> values = new List<string>() { "Loading.", "Loading..", "Loading...", "Loading" };
			int idx = 0;
			captionText.gameObject.SetActive(false);
			while (!stopDisplayLoading)
			{
				statusText.text = values[idx++ % values.Count];
				yield return new WaitForSecondsRealtime(0.3f);
			}
			statusText.gameObject.SetActive(false);
			captionText.gameObject.SetActive(true);
		}

		private IEnumerator Run()
		{
			if (connection == null || !connection.IsAuthorized)
			{
				HandleError("Connection should be initialized!");
			}
			StartCoroutine(DisplayLoading());

			var createSelfieRequest = connection.CreateSelfie();
			yield return createSelfieRequest;

			stopDisplayLoading = true;
			if (createSelfieRequest.IsError)
			{
				HandleError("Error occured: unable to create a selfie");
				yield break;
			}
			var selfieCode = createSelfieRequest.Result.code;
			var retrieveQrCodeRequest = connection.RetrieveQrCode(selfieCode);
			yield return retrieveQrCodeRequest;
			if (retrieveQrCodeRequest.IsError)
			{
				HandleError("Error occured: unable to download selfie");
				yield break;
			}
			var qrImageData = retrieveQrCodeRequest.Result;
			Texture2D texture = new Texture2D(1, 1);
			texture.LoadImage(qrImageData);

			qrImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
			qrImage.gameObject.SetActive(true);

			var awaitImageRequest = connection.AwaitSelfieAsync(selfieCode, 3.0f);
			yield return awaitImageRequest;
			if (awaitImageRequest.IsError || !awaitImageRequest.Result)
			{
				HandleError("Error occurred while waiting for user to upload selfie");
				yield break;
			}

			captionText.text = "Downloading selfie...";
			var imageRequest = connection.RetrieveSelfieFileAsync(selfieCode);
			yield return imageRequest;
			if (imageRequest.IsError)
			{
				HandleError("Error occured: unable to download selfie");
				yield break;
			}
			Selfie = imageRequest.Result;
			SelfieCode = selfieCode;

			mainObject.SetActive(false);
			if (completeActionHandler != null)
				completeActionHandler(true);
		}
	}
}
