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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ItSeez3D.AvatarSdkSamples.Core
{
	public interface IModelInfoSetter
	{
		void UpdateParameters(ModelInfoGroup allParameters, ModelInfoGroup defaultParameters);
		ModelInfoGroup GetParameters();
	}

	public interface IAvatarModificationsSetter
	{
		void UpdateParameters(AvatarModificationsGroup allParameters, AvatarModificationsGroup defaultParameters);
		AvatarModificationsGroup GetParameters();
	}

	public interface IShapeModificationsSetter
	{
		void UpdateParameters(ShapeModificationsGroup allParameters, ShapeModificationsGroup defaultParameters);
		ShapeModificationsGroup GetParameters();
	}


	public class ParametersConfigurationPanel : MonoBehaviour
	{
		public GameObject haircutsPanel;
		public GameObject blendshapesPanel;
		public GameObject modelInfoPanel;
		public GameObject avatarModificationsPanel;
		public GameObject shapeModificationsPanel;
		public GameObject additionalTexturesPanel;

		public Toggle haircutsToggle;
		public Toggle blendshapesToggle;
		public Toggle modelInfoToggle;
		public Toggle avatarModificationsToggle;
		public Toggle shapeModificationsToggle;
		public Toggle additionalTexturesToggle;

		protected ItemsSelectingView haircutsSelectingView;
		protected ItemsSelectingView blendshapesSelectingView;
		protected IModelInfoSetter modelInfoSetter;
		protected IAvatarModificationsSetter avatarModificationsSetter;
		protected IShapeModificationsSetter shapeModificationsSetter;
		protected ItemsSelectingView additionalTexturesSelectingView;

		protected List<GameObject> panels = new List<GameObject>();

		protected virtual void Start()
		{
			if (haircutsToggle != null && haircutsPanel != null)
			{
				haircutsToggle.onValueChanged.AddListener(isOn => haircutsPanel.SetActive(isOn));
				panels.Add(haircutsPanel);
				haircutsSelectingView = haircutsPanel.GetComponentInChildren<ItemsSelectingView>();
			}
			if (blendshapesToggle != null && blendshapesPanel != null)
			{
				blendshapesToggle.onValueChanged.AddListener(isOn => blendshapesPanel.SetActive(isOn));
				panels.Add(blendshapesPanel);
				blendshapesSelectingView = blendshapesPanel.GetComponentInChildren<ItemsSelectingView>();
			}
			if (modelInfoToggle != null && modelInfoPanel != null)
			{
				modelInfoToggle.onValueChanged.AddListener(isOn => modelInfoPanel.SetActive(isOn));
				panels.Add(modelInfoPanel);
				modelInfoSetter = modelInfoPanel.GetComponentInChildren<IModelInfoSetter>();
			}
			if (avatarModificationsToggle != null && avatarModificationsPanel != null)
			{
				avatarModificationsToggle.onValueChanged.AddListener(isOn => avatarModificationsPanel.SetActive(isOn));
				panels.Add(avatarModificationsPanel);
				avatarModificationsSetter = avatarModificationsPanel.GetComponentInChildren<IAvatarModificationsSetter>();
			}
			if (shapeModificationsToggle != null && shapeModificationsPanel != null)
			{
				shapeModificationsToggle.onValueChanged.AddListener(isOn => shapeModificationsPanel.SetActive(isOn));
				panels.Add(shapeModificationsPanel);
				shapeModificationsSetter = shapeModificationsPanel.GetComponentInChildren<IShapeModificationsSetter>();
			}
			if (additionalTexturesToggle != null && additionalTexturesPanel != null)
			{
				additionalTexturesToggle.onValueChanged.AddListener(isOn => additionalTexturesPanel.SetActive(isOn));
				panels.Add(additionalTexturesPanel);
				additionalTexturesSelectingView = additionalTexturesPanel.GetComponentInChildren<ItemsSelectingView>();
			}
		}

		public void SetControlsInteractable(bool interactable)
		{
			foreach (GameObject obj in panels)
			{
				foreach (Selectable c in obj.GetComponentsInChildren<Selectable>())
				{
					ControlEnabling controlEnabling = c.GetComponent<ControlEnabling>();
					if (controlEnabling == null)
						controlEnabling = c.gameObject.transform.parent.GetComponent<ControlEnabling>();
					if (controlEnabling == null || controlEnabling.isEnabled)
						c.interactable = interactable;
				}
			}
		}

		public void UpdateParameters(ComputationParameters allParameters, ComputationParameters defaultParameters)
		{
			if (allParameters == null || defaultParameters == null)
			{
				if (haircutsSelectingView != null)
					haircutsSelectingView.InitItems(new List<string>());
				if (blendshapesSelectingView != null)
					blendshapesSelectingView.InitItems(new List<string>());
				if (modelInfoSetter != null)
					modelInfoSetter.UpdateParameters(new ModelInfoGroup(), new ModelInfoGroup());
				if (avatarModificationsSetter != null)
					avatarModificationsSetter.UpdateParameters(new AvatarModificationsGroup(), new AvatarModificationsGroup());
				if (additionalTexturesSelectingView != null)
					additionalTexturesSelectingView.InitItems(new List<string>());
				if (shapeModificationsSetter != null)
					shapeModificationsSetter.UpdateParameters(new ShapeModificationsGroup(), new ShapeModificationsGroup());
			}
			else
			{
				if (haircutsSelectingView != null)
					haircutsSelectingView.InitItems(allParameters.haircuts.FullNames, defaultParameters.haircuts.FullNames);
				if (blendshapesSelectingView != null)
					blendshapesSelectingView.InitItems(allParameters.blendshapes.FullNames, defaultParameters.blendshapes.FullNames);
				if (modelInfoSetter != null)
					modelInfoSetter.UpdateParameters(allParameters.modelInfo, defaultParameters.modelInfo);
				if (avatarModificationsSetter != null)
					avatarModificationsSetter.UpdateParameters(allParameters.avatarModifications, defaultParameters.avatarModifications);
				if (additionalTexturesSelectingView != null)
					additionalTexturesSelectingView.InitItems(allParameters.additionalTextures.FullNames, defaultParameters.additionalTextures.FullNames);
				if (shapeModificationsSetter != null)
					shapeModificationsSetter.UpdateParameters(allParameters.shapeModifications, defaultParameters.shapeModifications);
			}
		}

		public void ConfigureComputationParameters(ComputationParameters outComputationParameters)
		{
			if (haircutsSelectingView != null)
				outComputationParameters.haircuts = new ComputationList(haircutsSelectingView.CurrentSelection);
			if (blendshapesSelectingView != null)
				outComputationParameters.blendshapes = new ComputationList(blendshapesSelectingView.CurrentSelection);
			if (modelInfoSetter != null)
				outComputationParameters.modelInfo = modelInfoSetter.GetParameters();
			if (avatarModificationsSetter != null)
				outComputationParameters.avatarModifications = avatarModificationsSetter.GetParameters();
			if (shapeModificationsSetter != null)
				outComputationParameters.shapeModifications = shapeModificationsSetter.GetParameters();
			if (additionalTexturesSelectingView != null)
				outComputationParameters.additionalTextures = new ComputationList(additionalTexturesSelectingView.CurrentSelection);
		}
	}
}
