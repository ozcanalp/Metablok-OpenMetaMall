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
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ItSeez3D.AvatarSdkSamples.Cloud
{
	public class AllSamplesCloud : MonoBehaviour
	{
		public void RunGettingStartedSample()
		{
			SceneManager.LoadScene(PluginStructure.GetScenePath(SampleScene.CLOUD_01_GETTING_STARTED));
		}

		public void RunGallerySample()
		{
			SceneManager.LoadScene(PluginStructure.GetScenePath(SampleScene.CLOUD_02_GALLERY));
		}

		public void RunLODSample()
		{
			SceneManager.LoadScene(PluginStructure.GetScenePath(SampleScene.CLOUD_03_LOD));
		}

		public void RunParametersSample()
		{
			SceneManager.LoadScene(PluginStructure.GetScenePath(SampleScene.CLOUD_04_PARAMETERS));
		}

		public void RunCartoonishAvatarSample()
		{
			SceneManager.LoadScene(PluginStructure.GetScenePath(SampleScene.CLOUD_05_CARTOONISH_AVATAR));
		}

		public void RunWebglSample()
		{
			SceneManager.LoadScene(PluginStructure.GetScenePath(SampleScene.CLOUD_06_WEBGL));
		}

		public void RunFullbodyGettingStartedSample()
		{
			SceneManager.LoadScene(PluginStructure.GetScenePath(SampleScene.CLOUD_07_FULLBODY_GETTING_STARTED));
		}

		public void RunFullbodyParametersSample()
		{
			SceneManager.LoadScene(PluginStructure.GetScenePath(SampleScene.CLOUD_08_FULLBODY_PARAMETERS));
		}

		public void RunFullbodyExportSample()
		{
			SceneManager.LoadScene(PluginStructure.GetScenePath(SampleScene.CLOUD_09_FULLBODY_EXPORT));
		}
	}
}
