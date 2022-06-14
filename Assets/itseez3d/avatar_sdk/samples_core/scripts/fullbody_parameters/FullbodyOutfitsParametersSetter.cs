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
	public class FullbodyOutfitsParametersSetter : MonoBehaviour
	{
		public ItemsSelectingView outfitsSelectingView;
		public ItemsSelectingView additionalTexturesSelectingView;

		public Toggle meshFormatToggle;
		public Text meshFormatDropdownLabel;

		public Toggle textureSizeToggle;
		public InputField textureWidthInput;
		public InputField textureHeightInput;

		public Toggle embedToggle;

		public Toggle embedTexturesToggle;

		public OutfitsParameters GetParameters()
		{
			OutfitsParameters outfitsParameters = new OutfitsParameters();
			outfitsParameters.names = outfitsSelectingView.CurrentSelection;
			outfitsParameters.additionalTextures = additionalTexturesSelectingView.CurrentSelection;
			outfitsParameters.embed = embedToggle.isOn;
			if (meshFormatToggle != null && meshFormatToggle.isOn)
				outfitsParameters.meshFormat = MeshFormatExtensions.MeshFormatFromStr(meshFormatDropdownLabel.text);
			if (embedTexturesToggle != null)
				outfitsParameters.embedTextures = embedToggle.isOn;
			if (textureSizeToggle.isOn)
				outfitsParameters.textureSize = ConvertionUtils.StrToTextureSize(textureWidthInput.text, textureHeightInput.text);
			return outfitsParameters;
		}
	}
}
