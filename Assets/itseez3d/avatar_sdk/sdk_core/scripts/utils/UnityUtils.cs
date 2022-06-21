/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
* You may not use this file except in compliance with an authorized license
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
* See the License for the specific language governing permissions and limitations under the License.
* Written by Itseez3D, Inc. <support@avatarsdk.com>, November 2020
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ItSeez3D.AvatarSdk.Core
{
	public static class UnityUtils
	{
		public static GameObject FindSubobjectByName(GameObject obj, string name, bool includeInactive = true)
		{
			foreach (var trans in obj.GetComponentsInChildren<Transform>(includeInactive))
				if (trans.name == name)
					return trans.gameObject;

			return null;
		}

		public static void FindAllChildObjects(GameObject parentObject, List<GameObject> childList)
		{
			for(int i=0; i<parentObject.transform.childCount; i++)
			{
				GameObject childObject = parentObject.transform.GetChild(i).gameObject;
				childList.Add(childObject);
				FindAllChildObjects(childObject, childList);
			}
		}
	}
}
