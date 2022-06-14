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
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace ItSeez3D.AvatarSdkSamples.Core
{
	public class ModelInfoDataPanel : MonoBehaviour
	{
		public GameObject dataPanel;
		public GameObject showButton;

		public Text hairColorText;
		public Text skinColorText;
		public Text genderText;
		public Text ageText;
		public Text landmarksText;
		public Text eyeScleraColorText;
		public Text eyeIrisColorText;
		public Text lipsColorText;
		public Text predictHaircutText;
		public Text race;

		public bool showLandmarks = true;
		public bool showConfidence = true;

		public void OnShowButtonClick()
		{
			dataPanel.SetActive(!dataPanel.activeSelf);
		}

		public void SetActive(bool isActive)
		{
			dataPanel.SetActive(isActive);
		}

		public void UpdateData(ModelInfo modelInfo)
		{
			if (!ModelInfo.HasPredictedData(modelInfo))
			{
				showButton.gameObject.SetActive(false);
				return;
			}
			showButton.gameObject.SetActive(true);

			UpdateColor(hairColorText, modelInfo.hair_color);
			UpdateColor(skinColorText, modelInfo.skin_color);

			AvatarGender gender = modelInfo.Gender;
			if (gender == AvatarGender.Female || gender == AvatarGender.Male)
			{
				if (showConfidence)
					genderText.text = string.Format("Gender: {0}, Confidence: {1:0.##}", gender, modelInfo.gender_confidence);
				else
					genderText.text = string.Format("Gender: {0}", gender);
				SetActive(genderText, true);
			}
			else
				SetActive(genderText, false);

			AvatarAgeGroup ageGroup = modelInfo.AgeGroup;
			if (ageGroup != AvatarAgeGroup.Unknown)
			{
				if (showConfidence)
					ageText.text = string.Format("Age Group: {0}, Confidence: {1:0.##}", ageGroup, modelInfo.age_confidence);
				else
					ageText.text = string.Format("Age Group: {0}", ageGroup);
				SetActive(ageText, true);
			}
			else
				SetActive(ageText, false);

			SetActive(landmarksText, modelInfo.facial_landmarks_68 != null && modelInfo.facial_landmarks_68.Length > 0 && showLandmarks);

			UpdateColor(eyeScleraColorText, modelInfo.eye_sclera_color);
			UpdateColor(eyeIrisColorText, modelInfo.eye_iris_color);
			UpdateColor(lipsColorText, modelInfo.lips_color);

			if (!string.IsNullOrEmpty(modelInfo.haircut_name))
			{
				predictHaircutText.text = modelInfo.haircut_name;
				predictHaircutText.transform.parent.gameObject.SetActive(true);
			}
			else
				predictHaircutText.transform.parent.gameObject.SetActive(false);

			var predictedRace = modelInfo.races == null ? "" : modelInfo.races.GetPredictedRace();
			if (!string.IsNullOrEmpty(predictedRace))
			{
				predictedRace = predictedRace.First().ToString().ToUpper() + predictedRace.Substring(1);
				race.text = string.Format("Predicted Race: {0}", predictedRace);
				SetActive(race, true);
			}
			else
				SetActive(race, false);
		}

		private void UpdateColor(Text text, AvatarColor color)
		{
			if (color != null && !color.IsClear())
			{
				Image img = text.gameObject.GetComponentInChildren<Image>();
				img.color = color.ToUnityColor();
				text.gameObject.SetActive(true);
			}
			else
				text.gameObject.SetActive(false);
		}

		private void SetActive(Text text, bool isActive)
		{
			text.gameObject.SetActive(isActive);
		}
	}
}
