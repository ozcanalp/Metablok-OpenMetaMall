/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
* You may not use this file except in compliance with an authorized license
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
* See the License for the specific language governing permissions and limitations under the License.
* Written by Itseez3D, Inc. <support@avatarsdk.com>, January 2021
*/

using ItSeez3D.AvatarSdk.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ItSeez3D.AvatarSdkSamples.Core
{
	public class FullbodyHaircutsParametersSetter : MonoBehaviour
	{
		public ItemsSelectingView haircutsSelectingView;

		public Toggle meshFormatToggle;
		public Text meshFormatDropdownLabel;

		public Toggle textureSizeToggle;
		public InputField textureWidthInput;
		public InputField textureHeightInput;

		public Toggle haircutColorToggle;
		public InputField haircutColorInput;

		public Toggle embedToggle;

		public Toggle embedTexturesToggle;

		public HaircutsParameters GetParameters()
		{
			HaircutsParameters haircutsParameters = new HaircutsParameters();
			haircutsParameters.names = haircutsSelectingView.CurrentSelection;
			haircutsParameters.embed = embedToggle.isOn;
			if (meshFormatToggle != null && meshFormatToggle.isOn)
				haircutsParameters.meshFormat = MeshFormatExtensions.MeshFormatFromStr(meshFormatDropdownLabel.text);
			if (embedTexturesToggle != null)
				haircutsParameters.embedTextures = embedTexturesToggle.isOn;
			if (textureSizeToggle.isOn)
				haircutsParameters.textureSize = ConvertionUtils.StrToTextureSize(textureWidthInput.text, textureHeightInput.text);
			if (haircutColorToggle.isOn)
				haircutsParameters.color = ConvertionUtils.StrToColor(haircutColorInput.text);
			return haircutsParameters;
		}
	}
}
