/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
* You may not use this file except in compliance with an authorized license
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
* See the License for the specific language governing permissions and limitations under the License.
* Written by Itseez3D, Inc. <support@avatarsdk.com>, February 2020
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ItSeez3D.AvatarSdk.Core
{
	public class CoroutineExecutionResult
	{
		public bool success = true;
		public Exception exception = null;
	}

	public class CoroutineOutVar<T>
	{
		public CoroutineOutVar(T val)
		{
			value = val;
		}

		public T value;
	}

	public static class CoroutineUtils
	{
		/// <summary>
		/// Method allows to catch exceptions during execution of coroutine.
		/// If the exception is caught, couroutine's execution will be interrupted but the result will be returned.
		/// </summary>
		public static IEnumerator AwaitCoroutine(IEnumerator coroutine, CoroutineExecutionResult outResult = null)
		{
			if (outResult == null)
				outResult = new CoroutineExecutionResult();

			while (true)
			{
				try
				{
					if (!coroutine.MoveNext())
						yield break;
				}
				catch (Exception exc)
				{
					Debug.LogErrorFormat("Coroutine execution exception: {0}", exc);
					outResult.success = false;
					outResult.exception = exc;
					yield break;
				}
				IEnumerator nested = coroutine.Current as IEnumerator;
				if (nested == null)
					yield return coroutine.Current;
				else
				{
					yield return AwaitCoroutine(nested, outResult);
					if (!outResult.success)
						yield break;
				}
			}
		}

		/// <summary>
		/// Method that allows to run coroutine synchronously.
		/// </summary>
		public static void WaitCoroutine(IEnumerator coroutine)
		{
			while (coroutine.MoveNext())
			{
				if (coroutine.Current != null)
				{
					if (coroutine.Current is IEnumerator)
						WaitCoroutine(coroutine.Current as IEnumerator);
					else
					{
						Debug.LogErrorFormat("Unexpected enumerator type: {0}", coroutine.Current.GetType());
						return;
					}
				}
			}
		}
	}
}
