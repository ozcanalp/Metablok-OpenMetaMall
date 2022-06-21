/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
* You may not use this file except in compliance with an authorized license
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
* See the License for the specific language governing permissions and limitations under the License.
* Written by Itseez3D, Inc. <support@avatarsdk.com>, June 2019
*/

using ItSeez3D.AvatarSdk.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ItSeez3D.AvatarSdkSamples.Core
{
	public class CommonShapeModificationsSetter : ComputationParametersPanel, IShapeModificationsSetter
	{
		public Toggle cartoonishV03Toggle;
		public Toggle cartoonishV1Toggle;
		public Slider cartoonishV03Slider;
		public Slider cartoonishV1Slider;

		private ShapeModificationsGroup allParameters;
		private ShapeModificationsGroup defaultParameters;

		public void UpdateParameters(ShapeModificationsGroup allParameters, ShapeModificationsGroup defaultParameters)
		{
			this.allParameters = allParameters;
			this.defaultParameters = defaultParameters;

			SelectDefaultParameters();
		}

		public ShapeModificationsGroup GetParameters()
		{
			ShapeModificationsGroup shapeModificationsParams = new ShapeModificationsGroup();
			if (cartoonishV03Toggle != null && cartoonishV03Slider != null)
				shapeModificationsParams.cartoonishV03 = CreatePropertyAndSetValue(allParameters.cartoonishV03, cartoonishV03Toggle, cartoonishV03Slider.value);
			if (cartoonishV1Toggle != null && cartoonishV1Slider != null)
				shapeModificationsParams.cartoonishV1 = CreatePropertyAndSetValue(allParameters.cartoonishV1, cartoonishV1Toggle, cartoonishV1Slider.value);
			return shapeModificationsParams;
		}

		public override void SelectAllParameters()
		{
			if (cartoonishV03Toggle != null && cartoonishV03Slider != null)
				SetToggleValue(cartoonishV03Toggle, allParameters.cartoonishV03.IsAvailable, allParameters.cartoonishV03.IsAvailable);
			if (cartoonishV1Toggle != null && cartoonishV1Slider != null)
				SetToggleValue(cartoonishV1Toggle, allParameters.cartoonishV1.IsAvailable, allParameters.cartoonishV1.IsAvailable);
		}

		public override void DeselectAllParameters()
		{
			if (cartoonishV03Toggle != null && cartoonishV03Slider != null)
				SetToggleValue(cartoonishV03Toggle, allParameters.cartoonishV03.IsAvailable, false);
			if (cartoonishV1Toggle != null && cartoonishV1Slider != null)
				SetToggleValue(cartoonishV1Toggle, allParameters.cartoonishV1.IsAvailable, false);
		}

		public override void SelectDefaultParameters()
		{
			if (cartoonishV03Toggle != null && cartoonishV03Slider != null)
				SetToggleValue(cartoonishV03Toggle, allParameters.cartoonishV03.IsAvailable, defaultParameters.cartoonishV03.IsAvailable);
			if (cartoonishV1Toggle != null && cartoonishV1Slider != null)
				SetToggleValue(cartoonishV1Toggle, allParameters.cartoonishV1.IsAvailable, defaultParameters.cartoonishV1.IsAvailable);
		}
	}
}
