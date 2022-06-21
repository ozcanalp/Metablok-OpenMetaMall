/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
* You may not use this file except in compliance with an authorized license
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
* See the License for the specific language governing permissions and limitations under the License.
* Written by Itseez3D, Inc. <support@avatarsdk.com>, November 2020
*/

using ItSeez3D.AvatarSdk.Core;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;


namespace ItSeez3D.AvatarSdkSamples.Core
{
	public class FullbodyBodyShapeSetter : ComputationParametersPanel
	{
		public Toggle genderToggle;
		public Toggle heightToggle;
		public Toggle weightToggle;
		public Toggle chestToggle;
		public Toggle waistToggle;
		public Toggle hipsToggle;

		public Dropdown genderDropdown;
		public InputField heightInput;
		public InputField weightInput;
		public InputField chestInput;
		public InputField waistInput;
		public InputField hipsInput;

		private BodyShapeGroup allParameters = null;

		private List<Selectable> modifiableShapeControls = null;

		private void Start()
		{
			modifiableShapeControls = new List<Selectable>()
			{
				genderToggle,
				heightToggle,
				weightToggle,
				chestToggle,
				waistToggle,
				hipsToggle,
				genderDropdown,
				heightInput,
				weightInput,
				chestInput,
				waistInput,
				hipsInput
			};
		}

		public override void SelectAllParameters() { }

		public override void DeselectAllParameters() { }

		public override void SelectDefaultParameters() { }

		public BodyShapeGroup GetParameters()
		{
			BodyShapeGroup bodyShapeParams = new BodyShapeGroup();

			bodyShapeParams.gender = CreatePropertyAndSetValue(allParameters.gender, genderToggle, GetSelectedGender());
			bodyShapeParams.height = CreatePropertyAndSetValue(allParameters.height, heightToggle, GetFloat(heightInput.text));
			bodyShapeParams.weight = CreatePropertyAndSetValue(allParameters.weight, weightToggle, GetFloat(weightInput.text));
			bodyShapeParams.waist = CreatePropertyAndSetValue(allParameters.waist, waistToggle, GetFloat(waistInput.text));
			bodyShapeParams.chest = CreatePropertyAndSetValue(allParameters.chest, chestToggle, GetFloat(chestInput.text));
			bodyShapeParams.hips = CreatePropertyAndSetValue(allParameters.hips, hipsToggle, GetFloat(hipsInput.text));

			return bodyShapeParams;
		}

		public void UpdateParameters(BodyShapeGroup allParameters, BodyShapeGroup defaultParameters)
		{
			this.allParameters = allParameters;
			SelectDefaultParameters();
		}

		public void OnTemplateToggleChanged(bool isOn)
		{
			modifiableShapeControls.ForEach(c => c.interactable = !isOn);
		}

		private AvatarGender GetSelectedGender()
		{
			switch (genderDropdown.value)
			{
				case 0: return AvatarGender.Male;
				case 1: return AvatarGender.Female;
				case 2: return AvatarGender.NonBinary;
				default: return AvatarGender.Unknown;
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