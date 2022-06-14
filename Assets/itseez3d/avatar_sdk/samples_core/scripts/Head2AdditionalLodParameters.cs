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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ItSeez3D.AvatarSdkSamples.Core
{
	public class Head2AdditionalLodParameters : MonoBehaviour
	{
		public class Head2AdditionalLodParametersModel
		{
			public TextureSize ModelTextureSize = null;
			public TextureSize HaircutTextureSize = null;
			public int? NumberOfFaces = null;

			private bool ValidateInt(int val, int upperBound)
			{
				if (val > 0 && val <= upperBound) return true;
				return false;
			}

			private bool ValidateSize(TextureSize val, int upperBound)
			{
				if (val == null)
				{
					return true;
				}
				if (ValidateInt(val.height, upperBound) && ValidateInt(val.width, upperBound))
				{
					return true;
				}
				return false;
			}

			public bool IsValid(out string errorText)
			{
				if (!ValidateSize(ModelTextureSize, 4096))
				{
					errorText = "Texture size should be between 1 and 4096";
					return false;
				}
				if (!ValidateSize(HaircutTextureSize, 4096))
				{
					errorText = "Haircut texture size should be between 1 and 4096";
					return false;
				}
				if (!ValidateInt(NumberOfFaces.Value, 10000))
				{
					errorText = "Number of faces should be between 1 and 10000";
					return false;
				}
				errorText = "";
				return true;
			}
		}

		public Head2AdditionalLodParametersModel Model
		{
			get
			{
				Head2AdditionalLodParametersModel result = new Head2AdditionalLodParametersModel();
				int? modelW = ExtractValue(ModelTextureWidthInput);
				int? modelH = ExtractValue(ModelTextureHeightInput);
				if (modelW.HasValue && modelH.HasValue) { result.ModelTextureSize = new TextureSize(modelW.Value, modelH.Value); }

				int? haircutW = ExtractValue(HaircutTextureWidthInput);
				int? haircutH = ExtractValue(HaircutTextureHeightInput);
				if (haircutW.HasValue && haircutH.HasValue) { result.HaircutTextureSize = new TextureSize(haircutW.Value, haircutH.Value); }

				result.NumberOfFaces = ExtractValue(FacesCountInput);
				return result;
			}
		}
		public Text ModelTextureWidthInput;
		public Text ModelTextureHeightInput;
		public Text HaircutTextureHeightInput;
		public Text HaircutTextureWidthInput;
		public Text FacesCountInput;
		private int? ExtractValue(Text input)
		{

			int result = 0;
			if (int.TryParse(input.text, out result))
			{
				return result;
			}
			else
			{
				return null;
			}

		}
		// Start is called before the first frame update
		void Start()
		{
		}

		// Update is called once per frame
		void Update()
		{

		}
	}
}
