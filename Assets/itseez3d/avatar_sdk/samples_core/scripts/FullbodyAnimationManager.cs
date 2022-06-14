/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
* You may not use this file except in compliance with an authorized license
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
* See the License for the specific language governing permissions and limitations under the License.
* Written by Itseez3D, Inc. <support@avatarsdk.com>, May 2021
*/

using ItSeez3D.AvatarSdk.Core;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ItSeez3D.AvatarSdkSamples.Core
{
	/// <summary>
	/// Helper class to deal with humanoid animations.
	/// </summary>
	public class FullbodyAnimationManager : AnimationManager
	{
		private enum SkeletonType
		{
			SMPL,
			MIXAMO
		}

		public bool standOnHeels = false;

		private Avatar animatorAvatar = null;
		private Avatar animatorAvatarOnHeels = null;

		private bool isOnHeels = false;

		private Dictionary<SkeletonType, string> humanBonesAssetsMap = new Dictionary<SkeletonType, string>()
		{
			{ SkeletonType.SMPL, "human_bones_smpl" },
			{ SkeletonType.MIXAMO, "human_bones_mixamo" }
		};

		private void Start()
		{
			SkinnedMeshRenderer meshRenderer = FindBodyMeshRenderer();
			if (meshRenderer == null)
			{
				Debug.LogError("Unable to find body mesh renderer.");
				return;
			}

			animator = gameObject.AddComponent<Animator>();
			animator.applyRootMotion = true;
			animator.runtimeAnimatorController = animatorController;
			animatorAvatar = AvatarBuilder.BuildHumanAvatar(gameObject, BuildHumanDescription(meshRenderer, false));
			animatorAvatarOnHeels = AvatarBuilder.BuildHumanAvatar(gameObject, BuildHumanDescription(meshRenderer, true));
			animator.avatar = ChooseAvatar();

			// Playing animation could be requested before animation manager starts. Play it now.
			if (currentAnimationIdx != -1)
				PlayCurrentAnimation();
			else
				currentAnimationIdx = 0;
			OnAnimationChanged();
		}

		private void Update()
		{
			if (standOnHeels != isOnHeels)
			{
				isOnHeels = standOnHeels;
				if (animator != null)
				{
					animator.avatar = ChooseAvatar();
				}
			}
		}

		private HumanDescription BuildHumanDescription(SkinnedMeshRenderer meshRenderer, bool modelOnHeels)
		{
			HumanDescription description = new HumanDescription();
			description.armStretch = 0.05f;
			description.legStretch = 0.05f;
			description.upperArmTwist = 0.5f;
			description.lowerArmTwist = 0.5f;
			description.upperLegTwist = 0.5f;
			description.lowerLegTwist = 0.5f;
			description.feetSpacing = 0;

			SkeletonType skeletonType = DetectSkeletonTypeByRootBone(meshRenderer);
			TextAsset humanBonesContent = Resources.Load<TextAsset>(humanBonesAssetsMap[skeletonType]);
			string[] lines = humanBonesContent.text.Split(new string[] { "\r\n", "\n" }, System.StringSplitOptions.RemoveEmptyEntries);

			List<HumanBone> humanBones = new List<HumanBone>();
			for (int i = 0; i < lines.Length; i++)
			{
				string[] names = lines[i].Split(',');
				humanBones.Add(new HumanBone() { boneName = names[0], humanName = names[1], limit = new HumanLimit() { useDefaultValues = true } });
			}
			description.human = humanBones.ToArray();

			List<Transform> bones = meshRenderer.bones.ToList();
			Matrix4x4[] bindPoses = meshRenderer.sharedMesh.bindposes;
			List<SkeletonBone> skeletonBones = new List<SkeletonBone>();
			for (int i = 0; i < bones.Count; i++)
			{
				Matrix4x4 boneLocalPosition = bindPoses[i].inverse;
				int parentIdx = bones.FindIndex(b => b.name == bones[i].parent.name);
				if (parentIdx > 0)
					boneLocalPosition = bindPoses[parentIdx] * boneLocalPosition;

				SkeletonBone bone = new SkeletonBone()
				{
					name = bones[i].name,
					position = boneLocalPosition.GetPosition(),
					rotation = boneLocalPosition.GetRotation(),
					scale = boneLocalPosition.GetScale()
				};

				if (skeletonType == SkeletonType.SMPL || IsFemaleModelSkeleton(bones))
					ModifyBoneTransformIfRequired(ref bone, skeletonType, modelOnHeels);

				skeletonBones.Add(bone);
			}
			description.skeleton = skeletonBones.ToArray();

			return description;
		}

		private Avatar ChooseAvatar()
		{
			return isOnHeels ? animatorAvatarOnHeels : animatorAvatar;
		}

		private SkinnedMeshRenderer FindBodyMeshRenderer()
		{
			var meshRenderers = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
			if (meshRenderers == null || meshRenderers.Length == 0)
				return null;

			SkinnedMeshRenderer bodyMeshRenderer = meshRenderers.FirstOrDefault(r => r.name == "mesh");
			if (bodyMeshRenderer != null)
				return bodyMeshRenderer;

			Debug.LogWarning("Unable to find body mesh renderer with name \"mesh\". Return the first in a list");
			return meshRenderers[0];
		}

		private SkeletonType DetectSkeletonTypeByRootBone(SkinnedMeshRenderer meshRenderer)
		{
			if (meshRenderer.rootBone.name == "RootNode")
				return SkeletonType.MIXAMO;
			return SkeletonType.SMPL;
		}

		private void ModifyBoneTransformIfRequired(ref SkeletonBone bone, SkeletonType skeletonType, bool isOnHeels)
		{
			if (skeletonType == SkeletonType.SMPL)
			{
				if (isOnHeels)
				{
					if (bone.name == "L_Ankle" || bone.name == "R_Ankle")
						bone.rotation = Quaternion.Euler(20.0f, 0.0f, 0.0f);

					if (bone.name == "L_Foot" || bone.name == "R_Foot")
						bone.rotation = Quaternion.Euler(-20.0f, 0.0f, 0.0f);
				}
			}
			if (skeletonType == SkeletonType.MIXAMO)
			{
				if (!isOnHeels)
				{
					if (bone.name == "RightFoot" || bone.name == "LeftFoot")
					{
						Vector3 eulers = bone.rotation.eulerAngles;
						bone.rotation = Quaternion.Euler(73, eulers.y, eulers.z);
					}
				}
			}
		}

		private bool IsFemaleModelSkeleton(List<Transform> bones)
		{
			Transform rightToeBone = bones.FirstOrDefault(b => b.name == "RightToeBase");
			Transform leftToeBone = bones.FirstOrDefault(b => b.name == "LeftToeBase");
			if (rightToeBone != null && leftToeBone != null)
				return rightToeBone.localPosition.z < 0.01 && leftToeBone.localPosition.z < 0.01;
			return false;
		}
	}
}
