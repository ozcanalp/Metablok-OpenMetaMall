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
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace ItSeez3D.AvatarSdk.Core
{
	/// <summary>
	/// This class is used to provide different implementations of interfaces for Cloud and Local Compute versions of SDK 
	/// </summary>
	public class IoCContainer
	{
		SdkType sdkType = SdkType.Cloud;

		Dictionary<Type, string> cloudImplementations = new Dictionary<Type, string>()
		{
			{ typeof(IAvatarProvider), "ItSeez3D.AvatarSdk.Cloud.CloudAvatarProvider" },
			{ typeof(IFullbodyAvatarProvider), "ItSeez3D.AvatarSdk.Cloud.CloudFullbodyAvatarProvider" },
			{ typeof(IPipelineTraitsKeeper), "ItSeez3D.AvatarSdk.Cloud.PipelineTraits.CloudTraitsKeeper" },
			{ typeof(IHaircutsPersistentStorage), "ItSeez3D.AvatarSdk.Cloud.CloudHaircutsPersistentStorage" },
		};

		Dictionary<Type, string> localComputeImplementations = new Dictionary<Type, string>()
		{
			{ typeof(IAvatarProvider), "ItSeez3D.AvatarSdk.LocalCompute.LocalComputeFullbodyAvatarProvider" },
			{ typeof(IFullbodyAvatarProvider), "ItSeez3D.AvatarSdk.LocalCompute.LocalComputeFullbodyAvatarProvider" },
			{ typeof(IPipelineTraitsKeeper), "ItSeez3D.AvatarSdk.LocalCompute.PipelineTraits.LocalComputeTraitsKeeper" }
		};

		public IoCContainer(SdkType sdkType)
		{
			this.sdkType = sdkType;
		}

		public T Create<T>()
		{
			Type type = typeof(T);
			var currentImplementations = GetCurrentImplementations();
			if (currentImplementations.ContainsKey(type))
			{
				string className =  currentImplementations[type];
				Assembly assembly = Assembly.GetExecutingAssembly();
				Type implType = assembly.GetType(className);
				if (implType == null)
				{
					Debug.LogErrorFormat("Unable to create instance of: {0}", implType);
					return default(T);
				}
				return (T)Activator.CreateInstance(implType);
			}
			else
			{
				Debug.LogErrorFormat("There is no implementation for: {0}", type);
				return default(T);
			}
		}

		public void SetSdkType(SdkType sdkType)
		{
			this.sdkType = sdkType;
		}

		private Dictionary<Type, string> GetCurrentImplementations()
		{
			if (sdkType == SdkType.LocalCompute)
				return localComputeImplementations;
			else
				return cloudImplementations;
		}
	}
}
