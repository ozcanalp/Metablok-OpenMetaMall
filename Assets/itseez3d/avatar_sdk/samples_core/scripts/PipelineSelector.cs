/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
* You may not use this file except in compliance with an authorized license
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
* See the License for the specific language governing permissions and limitations under the License.
* Written by Itseez3D, Inc. <support@avatarsdk.com>, April 2017
*/

using ItSeez3D.AvatarSdk.Core;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

namespace ItSeez3D.AvatarSdkSamples.Core
{
	public class PipelineSelector : MonoBehaviour
	{
		public PipelineType PipelineType { get; set; }
		public List<PipelineType> AvailablePipelines;
		public Dropdown dropdown;
		public delegate void PipelineTypeChangedDelegate(PipelineType newType);
		public event PipelineTypeChangedDelegate PipelineTypeChanged;
		public bool Interactable
		{
			get
			{
				return dropdown.interactable;
			}
			set
			{
				dropdown.interactable = value;
			}
		}
		
		// Start is called before the first frame update
		public void Start()
		{
			dropdown.ClearOptions();
			List<string> options = AvailablePipelines.Select(type => type.Traits().DisplayName).ToList();
			dropdown.AddOptions(options);
			dropdown.onValueChanged.AddListener(delegate { DropdownValueChanged(dropdown); });

			dropdown.value = 0;
			PipelineType = AvailablePipelines.FirstOrDefault();
			PipelineValueChanged(PipelineType);
		}
 
		// Update is called once per frame
		void Update()
		{

		}

		private void PipelineValueChanged(PipelineType type)
		{
			var evt = PipelineTypeChanged;
			if (evt != null)
			{
				evt(type);
			}
		}

		void DropdownValueChanged(Dropdown change)
		{
			PipelineValueChanged(AvailablePipelines[change.value]);
		}
	}
}