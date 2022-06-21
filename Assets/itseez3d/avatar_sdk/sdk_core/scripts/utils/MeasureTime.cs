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
using UnityEngine;

namespace ItSeez3D.AvatarSdk.Core
{
	/// <summary>
	/// Convenient timer to measure execution time.
	/// Can be used in "using" statement (IDisposable).
	/// </summary>
	public class MeasureTime : IDisposable
	{
		public static bool disableProfileTraces = false;

		private string msg;
		private DateTime start;
		private int startFrameNumber;
		private bool stopped = false;

		public MeasureTime(string _msg)
		{
			msg = _msg;
			start = DateTime.Now;
			startFrameNumber = Time.frameCount;
		}

		public double MillisecondsPassed()
		{
			return (DateTime.Now - start).TotalMilliseconds;
		}

		public double SecondsPassed()
		{
			return (DateTime.Now - start).TotalSeconds;
		}

		public int FramesPassed()
		{
			return Time.frameCount - startFrameNumber;
		}

		public void Measure()
		{
			if (!disableProfileTraces)
				Debug.LogFormat ("{0} took {1} ms", msg, MillisecondsPassed ());
		}

		public void MeasureSeconds()
		{
			if (!disableProfileTraces)
				Debug.LogFormat("{0} took {1} sec", msg, SecondsPassed());
		}

		public void MeasureFramesCount()
		{
			if (!disableProfileTraces)
				Debug.LogFormat("{0} took {1} frames", msg, FramesPassed());
		}

		public void Stop(bool showFramesCount = false)
		{
			if (!stopped)
			{
				MeasureSeconds();
				if (showFramesCount)
					MeasureFramesCount();
				stopped = true;
			}
		}

		public void Dispose ()
		{
			Stop ();
		}
	}
}
