/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
* You may not use this file except in compliance with an authorized license
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
* See the License for the specific language governing permissions and limitations under the License.
* Written by Itseez3D, Inc. <support@avatarsdk.com>, June 2020
*/

using ItSeez3D.AvatarSdk.Core;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace ItSeez3D.AvatarSdkSamples.Core
{
	public class FullbodyAvatarModificationsSetter : ComputationParametersPanel, IAvatarModificationsSetter
	{
		public Toggle smileRemovalToggle;
		public Toggle glassesRemovalToggle;
		public Toggle enhanceLightingToggle;
		public Toggle stubbleRemovalToggle;
		public Toggle eyeIrisColorToggle;
		public Toggle eyeScleraColorToggle;
		public Toggle parametricEyesToggle;
		public Toggle teethColorToggle;
		public Toggle generatedHaircutFacesNumberToggle;
		public Toggle generatedHaircutTextureSizeToggle;

		public InputField irisColorInput;
		public InputField scleraColorInput;
		public InputField teethColorInput;

		public InputField generatedHaircutTextureWidthInput;
		public InputField generatedHaircutTextureHeightInput;
		public InputField generatedHaircutFacesNumberInput;


		private AvatarModificationsGroup allParameters = null;

		public void UpdateParameters(AvatarModificationsGroup allParameters, AvatarModificationsGroup defaultParameters)
		{
			this.allParameters = allParameters;

			SelectDefaultParameters();
		}

		public AvatarModificationsGroup GetParameters()
		{
			AvatarModificationsGroup avatarModificationsParams = new AvatarModificationsGroup();
			avatarModificationsParams.parametricEyesTexture = CreateBoolPropertyAndSetValue(allParameters.parametricEyesTexture, parametricEyesToggle);
			avatarModificationsParams.eyeIrisColor = CreatePropertyAndSetValue(allParameters.eyeIrisColor, eyeIrisColorToggle, ConvertionUtils.StrToColor(irisColorInput.text));
			avatarModificationsParams.eyeScleraColor = CreatePropertyAndSetValue(allParameters.eyeScleraColor, eyeScleraColorToggle, ConvertionUtils.StrToColor(scleraColorInput.text));
			avatarModificationsParams.teethColor = CreatePropertyAndSetValue(allParameters.teethColor, teethColorToggle, ConvertionUtils.StrToColor(teethColorInput.text));
			
			avatarModificationsParams.generatedHaircutFacesCount = CreatePropertyAndSetValue(allParameters.generatedHaircutFacesCount, generatedHaircutFacesNumberToggle, GetInt(generatedHaircutFacesNumberInput.text));
			avatarModificationsParams.generatedHaircutTextureSize = CreatePropertyAndSetValue(allParameters.generatedHaircutTextureSize, generatedHaircutTextureSizeToggle, ConvertionUtils.StrToTextureSize(generatedHaircutTextureWidthInput.text, generatedHaircutTextureHeightInput.text));
			
			avatarModificationsParams.removeSmile = CreateBoolPropertyAndSetValue(allParameters.removeSmile, smileRemovalToggle);
			avatarModificationsParams.removeGlasses = CreateBoolPropertyAndSetValue(allParameters.removeGlasses, glassesRemovalToggle);
			avatarModificationsParams.enhanceLighting = CreateBoolPropertyAndSetValue(allParameters.enhanceLighting, enhanceLightingToggle);
			avatarModificationsParams.removeStubble = CreateBoolPropertyAndSetValue(allParameters.removeStubble, stubbleRemovalToggle);

			return avatarModificationsParams;
		}

		public override void SelectAllParameters()
		{
			SetToggleValue(parametricEyesToggle, allParameters.parametricEyesTexture.IsAvailable, allParameters.parametricEyesTexture.IsAvailable);
			SetToggleValue(eyeIrisColorToggle, allParameters.eyeIrisColor.IsAvailable, allParameters.eyeIrisColor.IsAvailable);
			SetToggleValue(eyeScleraColorToggle, allParameters.eyeScleraColor.IsAvailable, allParameters.eyeScleraColor.IsAvailable);
			SetToggleValue(teethColorToggle, allParameters.teethColor.IsAvailable, allParameters.teethColor.IsAvailable);
			SetToggleValue(generatedHaircutFacesNumberToggle, allParameters.generatedHaircutFacesCount.IsAvailable, allParameters.generatedHaircutFacesCount.IsAvailable);
			SetToggleValue(generatedHaircutTextureSizeToggle, allParameters.generatedHaircutTextureSize.IsAvailable, allParameters.generatedHaircutTextureSize.IsAvailable);
			SetToggleValue(smileRemovalToggle, allParameters.removeSmile.IsAvailable, allParameters.removeSmile.IsAvailable);
			SetToggleValue(glassesRemovalToggle, allParameters.removeGlasses.IsAvailable, allParameters.removeGlasses.IsAvailable);
			SetToggleValue(enhanceLightingToggle, allParameters.enhanceLighting.IsAvailable, allParameters.enhanceLighting.IsAvailable);
			SetToggleValue(stubbleRemovalToggle, allParameters.removeStubble.IsAvailable, allParameters.removeStubble.IsAvailable);
		}

		public override void DeselectAllParameters()
		{
			SetToggleValue(parametricEyesToggle, allParameters.parametricEyesTexture.IsAvailable, false);
			SetToggleValue(eyeIrisColorToggle, allParameters.eyeIrisColor.IsAvailable, false);
			SetToggleValue(eyeScleraColorToggle, allParameters.eyeScleraColor.IsAvailable, false);
			SetToggleValue(teethColorToggle, allParameters.teethColor.IsAvailable, false);
			SetToggleValue(generatedHaircutFacesNumberToggle, allParameters.generatedHaircutFacesCount.IsAvailable, false);
			SetToggleValue(generatedHaircutTextureSizeToggle, allParameters.generatedHaircutTextureSize.IsAvailable, false);
			SetToggleValue(smileRemovalToggle, allParameters.removeSmile.IsAvailable, false);
			SetToggleValue(glassesRemovalToggle, allParameters.removeGlasses.IsAvailable, false);
			SetToggleValue(enhanceLightingToggle, allParameters.enhanceLighting.IsAvailable, false);
			SetToggleValue(stubbleRemovalToggle, allParameters.removeStubble.IsAvailable, false);
		}

		public override void SelectDefaultParameters()
		{
			DeselectAllParameters();
		}

		private int GetInt(string str)
		{
			int result;
			if (int.TryParse(str, out result))
			{
				return result;
			}
			else
			{
				Debug.LogErrorFormat("Unable to parse string as int: {0}", str);
				return 0;
			}
		}

		private float GetFloat(string str)
		{
			float result;
			if (float.TryParse(str, NumberStyles.Float, CultureInfo.InvariantCulture, out result))
			{
				return result;
			}
			else
			{
				str = str.Replace(',', '.');
				if (float.TryParse(str, NumberStyles.Float, CultureInfo.InvariantCulture, out result))
				{
					return result;
				}
				else
				{
					Debug.LogErrorFormat("Unable to parse string as float: {0}", str);
					return 0;
				}
			}
		}
	}
}
