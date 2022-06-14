/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
* You may not use this file except in compliance with an authorized license
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
* See the License for the specific language governing permissions and limitations under the License.
* Written by Itseez3D, Inc. <support@avatarsdk.com>, April 2017
*/

using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace ItSeez3D.AvatarSdk.Core
{
	/// <summary>
	/// Represents subset of avatar computation parameters
	/// </summary>
	public enum ComputationParametersSubset
	{
		/// <summary>
		/// All available parameters
		/// </summary>
		ALL,

		/// <summary>
		/// Default subset of parameters
		/// </summary>
		DEFAULT
	}

	/// <summary>
	/// Parameters for avatar generation
	/// </summary>
	public class ComputationParameters
	{
		private ComputationParameters() { }

		public static ComputationParameters Empty
		{
			get { return new ComputationParameters(); }
		}

		public ComputationList blendshapes = new ComputationList();

		public ComputationList haircuts = new ComputationList();

		public ModelInfoGroup modelInfo = new ModelInfoGroup();

		public AvatarModificationsGroup avatarModifications = new AvatarModificationsGroup();

		public ShapeModificationsGroup shapeModifications = new ShapeModificationsGroup();

		public ComputationList additionalTextures = new ComputationList();

		public BodyShapeGroup bodyShape = new BodyShapeGroup();

		public ComputationList outfits = new ComputationList();

		public void Merge(ComputationParameters computationParameters)
		{
			haircuts.Merge(computationParameters.haircuts);
			blendshapes.Merge(computationParameters.blendshapes);
			additionalTextures.Merge(computationParameters.additionalTextures);
			outfits.Merge(computationParameters.outfits);
		}

		public void CopyFrom(ComputationParameters computationParameters)
		{
			blendshapes = computationParameters.blendshapes;
			haircuts = computationParameters.haircuts;
			modelInfo = computationParameters.modelInfo;
			avatarModifications = computationParameters.avatarModifications;
			shapeModifications = computationParameters.shapeModifications;
			additionalTextures = computationParameters.additionalTextures;
			bodyShape = computationParameters.bodyShape;
			outfits = computationParameters.outfits;
		}
	}
}
