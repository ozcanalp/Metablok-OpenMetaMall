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
using UnityEngine.UI;

namespace ItSeez3D.AvatarSdkSamples.Core
{
	public class FullbodyModelInfoSetter : ComputationParametersPanel, IModelInfoSetter
	{
		public Toggle hairColorToggle;
		public Toggle skinColorToggle;
		public Toggle genderToggle;
		public Toggle ageToggle;
		public Toggle landmarksToggle;
		public Toggle eyeScleraColorToggle;
		public Toggle eyeIrisColorToggle;
		public Toggle raceToggle;

		private ModelInfoGroup allParameters = null;

		public void UpdateParameters(ModelInfoGroup allParameters, ModelInfoGroup defaultParameters)
		{
			this.allParameters = allParameters;

			SelectDefaultParameters();
		}

		public ModelInfoGroup GetParameters()
		{
			ModelInfoGroup modelInfoParams = new ModelInfoGroup();
			modelInfoParams.hairColor = CreateBoolPropertyAndSetValue(allParameters.hairColor, hairColorToggle);
			modelInfoParams.skinColor = CreateBoolPropertyAndSetValue(allParameters.skinColor, skinColorToggle);
			modelInfoParams.gender = CreateBoolPropertyAndSetValue(allParameters.gender, genderToggle);
			modelInfoParams.age = CreateBoolPropertyAndSetValue(allParameters.age, ageToggle);
			modelInfoParams.facialLandmarks68 = CreateBoolPropertyAndSetValue(allParameters.facialLandmarks68, landmarksToggle);
			modelInfoParams.eyeScleraColor = CreateBoolPropertyAndSetValue(allParameters.eyeScleraColor, eyeScleraColorToggle);
			modelInfoParams.eyeIrisColor = CreateBoolPropertyAndSetValue(allParameters.eyeIrisColor, eyeIrisColorToggle);
			modelInfoParams.race = CreateBoolPropertyAndSetValue(allParameters.race, raceToggle);
			return modelInfoParams;
		}

		public override void SelectAllParameters()
		{
			SetToggleValue(hairColorToggle, allParameters.hairColor.IsAvailable, allParameters.hairColor.IsAvailable);
			SetToggleValue(skinColorToggle, allParameters.skinColor.IsAvailable, allParameters.skinColor.IsAvailable);
			SetToggleValue(genderToggle, allParameters.gender.IsAvailable, allParameters.gender.IsAvailable);
			SetToggleValue(ageToggle, allParameters.age.IsAvailable, allParameters.age.IsAvailable);
			SetToggleValue(landmarksToggle, allParameters.facialLandmarks68.IsAvailable, allParameters.facialLandmarks68.IsAvailable);
			SetToggleValue(eyeScleraColorToggle, allParameters.eyeScleraColor.IsAvailable, allParameters.eyeScleraColor.IsAvailable);
			SetToggleValue(eyeIrisColorToggle, allParameters.eyeIrisColor.IsAvailable, allParameters.eyeIrisColor.IsAvailable);
			SetToggleValue(raceToggle, allParameters.race.IsAvailable, allParameters.race.IsAvailable);
		}

		public override void DeselectAllParameters()
		{
			SetToggleValue(hairColorToggle, allParameters.hairColor.IsAvailable, false);
			SetToggleValue(skinColorToggle, allParameters.skinColor.IsAvailable, false);
			SetToggleValue(genderToggle, allParameters.gender.IsAvailable, false);
			SetToggleValue(ageToggle, allParameters.age.IsAvailable, false);
			SetToggleValue(landmarksToggle, allParameters.facialLandmarks68.IsAvailable, false);
			SetToggleValue(eyeScleraColorToggle, allParameters.eyeScleraColor.IsAvailable, false);
			SetToggleValue(eyeIrisColorToggle, allParameters.eyeIrisColor.IsAvailable, false);
			SetToggleValue(raceToggle, allParameters.race.IsAvailable, false);
		}

		public override void SelectDefaultParameters()
		{
			DeselectAllParameters();
		}
	}
}
