/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
* You may not use this file except in compliance with an authorized license
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
* See the License for the specific language governing permissions and limitations under the License.
* Written by Itseez3D, Inc. <support@avatarsdk.com>, June 2019
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ItSeez3D.AvatarSdk.Core;

namespace ItSeez3D.AvatarSdkSamples.Core
{
	public class CommonAvatarModificationsSetter : ComputationParametersPanel, IAvatarModificationsSetter
	{
		public Toggle curvedBottomToggle;
		public Toggle glareToggle;
		public Toggle eyelidShadowToggle;
		public Toggle eyeIrisColorToggle;
		public Toggle eyeScleraColorToggle;
		public Toggle hairColorToggle;
		public Toggle parametricEyesToggle;
		public Toggle parametricEyesV2Toggle;
		public Toggle allowModifyNeckToggle;
		public Toggle teethColorToggle;
		public Toggle lipsColorToggle;
		public Toggle caricatureToggle;
		public Toggle slightlyCartoonishTextureToggle;
		public Toggle generatedHaircutFacesNumberToggle;
		public Toggle generatedHaircutTextureSizeToggle;
		public Toggle modelTextureSizeToggle;
		public Toggle smileRemovalToggle;
		public Toggle glassesRemovalToggle;
		public Toggle enhanceLightingToggle;
		public Toggle stubbleRemovalToggle;

		public InputField irisColorInput;
		public InputField scleraColorInput;
		public InputField hairColorInput;
		public InputField lipsColorInput;
		public InputField teethColorInput;
		public Slider caricatureSlider;

		public InputField modelTextureWidthInput;
		public InputField modelTextureHeightInput;
		public InputField generatedHaircutTextureWidthInput;
		public InputField generatedHaircutTextureHeightInput;
		public InputField generatedHaircutFacesNumberInput;


		private AvatarModificationsGroup allParameters = null;
		private AvatarModificationsGroup defaultParameters = null;

		public void UpdateParameters(AvatarModificationsGroup allParameters, AvatarModificationsGroup defaultParameters)
		{
			this.allParameters = allParameters;
			this.defaultParameters = defaultParameters;

			SelectDefaultParameters();
		}

		public AvatarModificationsGroup GetParameters()
		{
			AvatarModificationsGroup avatarModificationsParams = new AvatarModificationsGroup();
			avatarModificationsParams.curvedBottom = CreateBoolPropertyAndSetValue(allParameters.curvedBottom, curvedBottomToggle);
			avatarModificationsParams.addGlare = CreateBoolPropertyAndSetValue(allParameters.addGlare, glareToggle);
			avatarModificationsParams.addEyelidShadow = CreateBoolPropertyAndSetValue(allParameters.addEyelidShadow, eyelidShadowToggle);
			avatarModificationsParams.parametricEyesTexture = CreateBoolPropertyAndSetValue(allParameters.parametricEyesTexture, parametricEyesToggle);
			avatarModificationsParams.parametricEyesTextureV2 = CreateBoolPropertyAndSetValue(allParameters.parametricEyesTextureV2, parametricEyesV2Toggle);
			avatarModificationsParams.eyeIrisColor = CreatePropertyAndSetValue(allParameters.eyeIrisColor, eyeIrisColorToggle, StringToColor(irisColorInput.text));
			avatarModificationsParams.hairColor = CreatePropertyAndSetValue(allParameters.hairColor, hairColorToggle, StringToColor(hairColorInput.text));
			avatarModificationsParams.eyeScleraColor = CreatePropertyAndSetValue(allParameters.eyeScleraColor, eyeScleraColorToggle, StringToColor(scleraColorInput.text));
			avatarModificationsParams.allowModifyNeck = CreateBoolPropertyAndSetValue(allParameters.allowModifyNeck, allowModifyNeckToggle);
			avatarModificationsParams.teethColor = CreatePropertyAndSetValue(allParameters.teethColor, teethColorToggle, StringToColor(teethColorInput.text));
			avatarModificationsParams.lipsColor = CreatePropertyAndSetValue(allParameters.lipsColor, lipsColorToggle, StringToColor(lipsColorInput.text));
			avatarModificationsParams.caricatureAmount = CreatePropertyAndSetValue(allParameters.caricatureAmount, caricatureToggle, caricatureSlider.value);
			avatarModificationsParams.slightlyCartoonishTexture = CreateBoolPropertyAndSetValue(allParameters.slightlyCartoonishTexture, slightlyCartoonishTextureToggle);

			avatarModificationsParams.generatedHaircutFacesCount = CreatePropertyAndSetValue(allParameters.generatedHaircutFacesCount, generatedHaircutFacesNumberToggle, ConvertionUtils.StrToInt(generatedHaircutFacesNumberInput.text));
			avatarModificationsParams.generatedHaircutTextureSize = CreatePropertyAndSetValue(allParameters.generatedHaircutTextureSize, generatedHaircutTextureSizeToggle, ConvertionUtils.StrToTextureSize(generatedHaircutTextureWidthInput.text, generatedHaircutTextureHeightInput.text));
			avatarModificationsParams.textureSize = CreatePropertyAndSetValue(allParameters.textureSize, modelTextureSizeToggle, ConvertionUtils.StrToTextureSize(modelTextureWidthInput.text, modelTextureHeightInput.text));

			avatarModificationsParams.removeSmile = CreateBoolPropertyAndSetValue(allParameters.removeSmile, smileRemovalToggle);
			avatarModificationsParams.removeGlasses = CreateBoolPropertyAndSetValue(allParameters.removeGlasses, glassesRemovalToggle);
			avatarModificationsParams.enhanceLighting = CreateBoolPropertyAndSetValue(allParameters.enhanceLighting, enhanceLightingToggle);
			avatarModificationsParams.removeStubble = CreateBoolPropertyAndSetValue(allParameters.removeStubble, stubbleRemovalToggle);

			return avatarModificationsParams;
		}

		public override void SelectAllParameters()
		{
			SetToggleValue(curvedBottomToggle, allParameters.curvedBottom.IsAvailable, allParameters.curvedBottom.IsAvailable);
			SetToggleValue(parametricEyesToggle, allParameters.parametricEyesTexture.IsAvailable, allParameters.parametricEyesTexture.IsAvailable);
			SetToggleValue(parametricEyesV2Toggle, allParameters.parametricEyesTextureV2.IsAvailable, allParameters.parametricEyesTextureV2.IsAvailable);
			SetToggleValue(glareToggle, allParameters.addGlare.IsAvailable, allParameters.addGlare.IsAvailable);
			SetToggleValue(eyelidShadowToggle, allParameters.addEyelidShadow.IsAvailable, allParameters.addEyelidShadow.IsAvailable);
			SetToggleValue(eyeIrisColorToggle, allParameters.eyeIrisColor.IsAvailable, allParameters.eyeIrisColor.IsAvailable);
			SetToggleValue(eyeScleraColorToggle, allParameters.eyeScleraColor.IsAvailable, allParameters.eyeScleraColor.IsAvailable);
			SetToggleValue(hairColorToggle, allParameters.hairColor.IsAvailable, allParameters.hairColor.IsAvailable);
			SetToggleValue(allowModifyNeckToggle, allParameters.allowModifyNeck.IsAvailable, allParameters.allowModifyNeck.IsAvailable);
			SetToggleValue(teethColorToggle, allParameters.teethColor.IsAvailable, allParameters.teethColor.IsAvailable);
			SetToggleValue(lipsColorToggle, allParameters.lipsColor.IsAvailable, allParameters.lipsColor.IsAvailable);
			SetToggleValue(caricatureToggle, allParameters.caricatureAmount.IsAvailable, allParameters.caricatureAmount.IsAvailable);
			SetToggleValue(slightlyCartoonishTextureToggle, allParameters.slightlyCartoonishTexture.IsAvailable, allParameters.slightlyCartoonishTexture.IsAvailable);
			SetToggleValue(modelTextureSizeToggle, allParameters.textureSize.IsAvailable, allParameters.textureSize.IsAvailable);
			SetToggleValue(generatedHaircutFacesNumberToggle, allParameters.generatedHaircutFacesCount.IsAvailable, allParameters.generatedHaircutFacesCount.IsAvailable);
			SetToggleValue(generatedHaircutTextureSizeToggle, allParameters.generatedHaircutTextureSize.IsAvailable, allParameters.generatedHaircutTextureSize.IsAvailable);
			SetToggleValue(smileRemovalToggle, allParameters.removeSmile.IsAvailable, allParameters.removeSmile.IsAvailable);
			SetToggleValue(glassesRemovalToggle, allParameters.removeGlasses.IsAvailable, allParameters.removeGlasses.IsAvailable);
			SetToggleValue(enhanceLightingToggle, allParameters.enhanceLighting.IsAvailable, allParameters.enhanceLighting.IsAvailable);
			SetToggleValue(stubbleRemovalToggle, allParameters.removeStubble.IsAvailable, allParameters.removeStubble.IsAvailable);
		}

		public override void DeselectAllParameters()
		{
			SetToggleValue(curvedBottomToggle, allParameters.curvedBottom.IsAvailable, false);
			SetToggleValue(parametricEyesToggle, allParameters.parametricEyesTexture.IsAvailable, false);
			SetToggleValue(parametricEyesV2Toggle, allParameters.parametricEyesTextureV2.IsAvailable, false);
			SetToggleValue(glareToggle, allParameters.addGlare.IsAvailable, false);
			SetToggleValue(eyelidShadowToggle, allParameters.addEyelidShadow.IsAvailable, false);
			SetToggleValue(eyeIrisColorToggle, allParameters.eyeIrisColor.IsAvailable, false);
			SetToggleValue(eyeScleraColorToggle, allParameters.eyeScleraColor.IsAvailable, false);
			SetToggleValue(hairColorToggle, allParameters.hairColor.IsAvailable, false);
			SetToggleValue(allowModifyNeckToggle, allParameters.allowModifyNeck.IsAvailable, false);
			SetToggleValue(teethColorToggle, allParameters.teethColor.IsAvailable, false);
			SetToggleValue(lipsColorToggle, allParameters.lipsColor.IsAvailable, false);
			SetToggleValue(caricatureToggle, allParameters.caricatureAmount.IsAvailable, false);
			SetToggleValue(slightlyCartoonishTextureToggle, allParameters.slightlyCartoonishTexture.IsAvailable, false);
			SetToggleValue(modelTextureSizeToggle, allParameters.textureSize.IsAvailable, false);
			SetToggleValue(generatedHaircutFacesNumberToggle, allParameters.generatedHaircutFacesCount.IsAvailable, false);
			SetToggleValue(generatedHaircutTextureSizeToggle, allParameters.generatedHaircutTextureSize.IsAvailable, false);
			SetToggleValue(smileRemovalToggle, allParameters.removeSmile.IsAvailable, false);
			SetToggleValue(glassesRemovalToggle, allParameters.removeGlasses.IsAvailable, false);
			SetToggleValue(enhanceLightingToggle, allParameters.enhanceLighting.IsAvailable, false);
			SetToggleValue(stubbleRemovalToggle, allParameters.removeStubble.IsAvailable, false);
		}

		public override void SelectDefaultParameters()
		{
			SetToggleValue(eyeIrisColorToggle, allParameters.eyeIrisColor.IsAvailable, defaultParameters.eyeIrisColor.IsAvailable);
			SetToggleValue(eyeScleraColorToggle, allParameters.eyeScleraColor.IsAvailable, defaultParameters.eyeScleraColor.IsAvailable);
			SetToggleValue(hairColorToggle, allParameters.hairColor.IsAvailable, defaultParameters.hairColor.IsAvailable);
			SetToggleValue(teethColorToggle, allParameters.teethColor.IsAvailable, defaultParameters.teethColor.IsAvailable);
			SetToggleValue(lipsColorToggle, allParameters.lipsColor.IsAvailable, defaultParameters.lipsColor.IsAvailable);
			SetToggleValue(caricatureToggle, allParameters.caricatureAmount.IsAvailable, defaultParameters.caricatureAmount.IsAvailable);
			SetToggleValue(modelTextureSizeToggle, allParameters.textureSize.IsAvailable, defaultParameters.textureSize.IsAvailable);
			SetToggleValue(generatedHaircutFacesNumberToggle, allParameters.generatedHaircutFacesCount.IsAvailable, defaultParameters.generatedHaircutFacesCount.IsAvailable);
			SetToggleValue(generatedHaircutTextureSizeToggle, allParameters.generatedHaircutTextureSize.IsAvailable, defaultParameters.generatedHaircutTextureSize.IsAvailable);

			//bool properties
			SetToggleValue(allowModifyNeckToggle, allParameters.allowModifyNeck.IsAvailable, defaultParameters.allowModifyNeck.Value);
			SetToggleValue(curvedBottomToggle, allParameters.curvedBottom.IsAvailable, defaultParameters.curvedBottom.Value);
			SetToggleValue(glareToggle, allParameters.addGlare.IsAvailable, defaultParameters.addGlare.Value);
			SetToggleValue(eyelidShadowToggle, allParameters.addEyelidShadow.IsAvailable, defaultParameters.addEyelidShadow.Value);
			SetToggleValue(parametricEyesToggle, allParameters.parametricEyesTexture.IsAvailable, defaultParameters.parametricEyesTexture.Value);
			SetToggleValue(parametricEyesV2Toggle, allParameters.parametricEyesTextureV2.IsAvailable, defaultParameters.parametricEyesTextureV2.Value);
			SetToggleValue(slightlyCartoonishTextureToggle, allParameters.slightlyCartoonishTexture.IsAvailable, defaultParameters.slightlyCartoonishTexture.Value);
			SetToggleValue(smileRemovalToggle, allParameters.removeSmile.IsAvailable, defaultParameters.removeSmile.IsAvailable);
			SetToggleValue(glassesRemovalToggle, allParameters.removeGlasses.IsAvailable, defaultParameters.removeGlasses.IsAvailable);
			SetToggleValue(enhanceLightingToggle, allParameters.enhanceLighting.IsAvailable, defaultParameters.enhanceLighting.IsAvailable);
			SetToggleValue(stubbleRemovalToggle, allParameters.removeStubble.IsAvailable, defaultParameters.removeStubble.IsAvailable);
		}

		private Color StringToColor(string str)
		{
			try
			{
				string[] parts = str.Split(',');
				int red = int.Parse(parts[0]);
				int green = int.Parse(parts[1]);
				int blue = int.Parse(parts[2]);
				return new Color(red / 255.0f, green / 255.0f, blue / 255.0f);
			}
			catch
			{
				Debug.LogErrorFormat("Unable to parse color value: {0}", str);
				return Color.white;
			}
		}
	}
}
