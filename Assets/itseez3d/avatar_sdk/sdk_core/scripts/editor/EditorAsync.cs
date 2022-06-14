/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
* You may not use this file except in compliance with an authorized license
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
* See the License for the specific language governing permissions and limitations under the License.
* Written by Itseez3D, Inc. <support@avatarsdk.com>, April 2017
*/
using System.Collections;

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ItSeez3D.AvatarSdk.Core.Editor
{
	/// <summary>
	/// Provides a way to create async tasks in the editor.
	/// </summary>
	public static class EditorAsync
	{
		public class EditorAsyncTask
		{
			public Func<bool> IsDone { get; private set; }

			public Action OnCompleted { get; private set; }

			public EditorAsyncTask (Func<bool> isDone, Action onCompleted)
			{
				IsDone = isDone;
				OnCompleted = onCompleted;
			}
		}

		private static readonly List<EditorAsyncTask> tasks = new List<EditorAsyncTask> ();

		public static void ProcessTask (EditorAsyncTask task)
		{
			if (tasks.Count == 0)
				EditorApplication.update += Process;
			tasks.Add (task);
		}

		private static void Process ()
		{
			for (int i = tasks.Count - 1; i >= 0; --i) {
				try {
					var task = tasks [i];
					if (!task.IsDone ())
						continue;

					tasks.RemoveAt (i);
					task.OnCompleted ();
				} catch (Exception ex) {
					Debug.LogException (ex);
				}
			}

			if (tasks.Count == 0)
				EditorApplication.update -= Process;
		}
	}
}
#endif