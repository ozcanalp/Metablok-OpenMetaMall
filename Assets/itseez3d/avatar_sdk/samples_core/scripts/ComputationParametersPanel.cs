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
	public abstract class ComputationParametersPanel : MonoBehaviour
	{
		public abstract void SelectAllParameters();

		public abstract void DeselectAllParameters();

		public abstract void SelectDefaultParameters();

		protected void SetToggleValue(Toggle toggle, bool isEnabled, bool value)
		{
			if (toggle != null)
			{
				ControlEnabling controlEnabling = toggle.GetComponentInChildren<ControlEnabling>();
				if (controlEnabling == null)
					controlEnabling = toggle.transform.parent.GetComponent<ControlEnabling>();
				controlEnabling.isEnabled = isEnabled;
				toggle.isOn = isEnabled && value;
			}
		}

		protected bool IsToggleEnabled(Toggle toggle)
		{
			ControlEnabling controlEnabling = toggle.GetComponentInChildren<ControlEnabling>();
			return controlEnabling.isEnabled;
		}

		protected ComputationProperty<bool> CreateBoolPropertyAndSetValue(ComputationProperty<bool> copyFromProperty, Toggle toggle)
		{
			if (IsToggleEnabled(toggle))
			{
				ComputationProperty<bool> property = new ComputationProperty<bool>(copyFromProperty.GroupName, copyFromProperty.Name);
				property.IsAvailable = copyFromProperty.IsAvailable;
				property.Value = toggle.isOn;
				return property;
			}
			else
				return null;
		}

		protected ComputationProperty<T> CreatePropertyAndSetValue<T>(ComputationProperty<T> copyFromProperty, Toggle toggle, T value)
		{
			if (toggle.isOn)
			{
				ComputationProperty<T> property = new ComputationProperty<T>(copyFromProperty.GroupName, copyFromProperty.Name);
				property.IsAvailable = copyFromProperty.IsAvailable;
				property.Value = value;
				return property;
			}
			else
				return null;
		}
	}
}
