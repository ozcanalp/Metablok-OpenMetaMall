/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
* You may not use this file except in compliance with an authorized license
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
* See the License for the specific language governing permissions and limitations under the License.
* Written by Itseez3D, Inc. <support@itseez3D.com>, January 2019
*/

using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;


namespace ItSeez3D.AvatarSdkSamples.Core.WebCamera
{
	public class ImageFrame
	{
		private int width, height;

		public Color32[] imageData;

		public ImageFrame(int w, int h)
		{
			width = w;
			height = h;
			imageData = new Color32[width * height];
		}
	}

	public class ImageBuffer
	{
		private Queue<ImageFrame> freeFrames = new Queue<ImageFrame>();
		private ImageFrame lastCapturedFrame = null;
		private object syncLock = new object();
		private AutoResetEvent capturedFrameEvent = new AutoResetEvent(false);

		int imageWidth = 0;
		int imageHeight = 0;

		public ImageBuffer(int imageWidth, int imageHeight, int bufferLength)
		{
			this.imageWidth = imageWidth;
			this.imageHeight = imageHeight;

			for (int i = 0; i < bufferLength; i++)
				freeFrames.Enqueue(new ImageFrame(imageWidth, imageHeight));
		}

		public ImageFrame GetFreeFrame()
		{
			lock (syncLock)
			{
				if (freeFrames.Count == 0)
				{
					Debug.LogWarning("There is no free frames in the buffer. Create new.");
					return new ImageFrame(imageWidth, imageHeight);
				}
				else
					return freeFrames.Dequeue();
			}
		}

		public void PushCapturedFrame(ImageFrame frame)
		{
			lock (syncLock)
			{
				if (lastCapturedFrame != null)
					freeFrames.Enqueue(lastCapturedFrame);
				lastCapturedFrame = frame;
				capturedFrameEvent.Set();
			}
		}

		public void PushFrame(ImageFrame frame)
		{
			lock (syncLock)
				freeFrames.Enqueue(frame);
		}

		public bool GetOrWaitForCapturedFrame(out ImageFrame resultFrame)
		{
			capturedFrameEvent.WaitOne();
			lock (syncLock)
			{
				if (lastCapturedFrame != null)
				{
					resultFrame = lastCapturedFrame;
					lastCapturedFrame = null;
					return true;
				}
			}
			resultFrame = null;
			return false;
		}

		public bool IsExistCapturedFrame()
		{
			return lastCapturedFrame != null;
		}
	}
}
