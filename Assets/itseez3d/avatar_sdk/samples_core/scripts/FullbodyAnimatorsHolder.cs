/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
* You may not use this file except in compliance with an authorized license
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
* See the License for the specific language governing permissions and limitations under the License.
* Written by Itseez3D, Inc. <support@avatarsdk.com>, December 2021
*/

using ItSeez3D.AvatarSdk.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ItSeez3D.AvatarSdkSamples.Core
{
	public class FullbodyAnimatorsHolder : MonoBehaviour
	{
		public RuntimeAnimatorController bodyMobileAnimatorController;
		public RuntimeAnimatorController bodyFemaleAnimatorController;
		public RuntimeAnimatorController bodyMaleAnimatorController;

		public RuntimeAnimatorController GetAnimatorController(PipelineType pipelineType)
		{
			switch (pipelineType)
			{
				case PipelineType.FIT_PERSON:
					return bodyMobileAnimatorController;
				case PipelineType.META_PERSON_FEMALE:
					return bodyFemaleAnimatorController;
				case PipelineType.META_PERSON_MALE:
					return bodyMaleAnimatorController;
				default:
					{
						Debug.LogErrorFormat("There is no animator controller for {0} pipeline", pipelineType);
						return null;
					}
			}
		}
	}
}
