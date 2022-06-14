/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
* You may not use this file except in compliance with an authorized license
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
* See the License for the specific language governing permissions and limitations under the License.
* Written by Itseez3D, Inc. <support@avatarsdk.com>, November 2021
*/

using ItSeez3D.AvatarSdk.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ItSeez3D.AvatarSdkSamples.Core
{
	public class FullbodyGeneralParametersSetter : MonoBehaviour
	{
		public Text meshFormatDropdownLabel;
		public Text lodDropdownLabel;
		public Text templateDropdownLabel;

		public void SetGeneralParameters(ref FullbodyAvatarComputationParameters computationParameters)
		{
			if (meshFormatDropdownLabel != null)
				computationParameters.meshFormat = MeshFormatExtensions.MeshFormatFromStr(meshFormatDropdownLabel.text);
			computationParameters.lod = Convert.ToInt32(lodDropdownLabel.text);
			computationParameters.template = ExportTemplateExtensions.ExportTemplateFromStr(templateDropdownLabel.text);
		}
	}
}
