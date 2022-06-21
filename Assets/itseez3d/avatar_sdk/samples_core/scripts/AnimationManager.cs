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
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace ItSeez3D.AvatarSdkSamples.Core
{
	/// <summary>
	/// Helper class to deal with head blendshape animations.
	/// </summary>
	public class AnimationManager : MonoBehaviour
	{
		public string[] animations;

		public event Action<string> onAnimationChanged;

		protected RuntimeAnimatorController animatorController;

		// animations-related data
		protected Animator animator = null;
		protected int currentAnimationIdx = -1;

		private void Start()
		{
			animator = gameObject.GetComponent<Animator>();
			if (animator == null)
				animator = gameObject.AddComponent<Animator>();
			animator.applyRootMotion = true;
			animator.runtimeAnimatorController = animatorController;

			// Play animation could be requested before animation manager starts. Play it now.
			if (currentAnimationIdx != -1)
				PlayCurrentAnimation();
			else
				currentAnimationIdx = 0;
			OnAnimationChanged();
		}

		public void AssignAnimatorController(RuntimeAnimatorController animatorController, string[] animationNames = null)
		{
			this.animatorController = animatorController;
			if (animator != null)
				animator.runtimeAnimatorController = animatorController;

			animations = animationNames;
		}

		public void PlayPrevAnimation ()
		{
			ChangeCurrentAnimation (-1);
		}

		public void PlayNextAnimation ()
		{
			ChangeCurrentAnimation (+1);
		}

		public void PlayCurrentAnimation ()
		{
			if (animator != null)
			{
				animator.Play(animations[currentAnimationIdx]);
			}
		}

		public void PlayAnimationByName(string animationName)
		{
			int animationIdx = animations.ToList().IndexOf(animationName);
			if (animationIdx < 0)
			{
				Debug.LogErrorFormat("There is no animation with name: {0}", animationName);
				return;
			}

			currentAnimationIdx = animationIdx;
			PlayCurrentAnimation();
		}

		private void ChangeCurrentAnimation(int delta)
		{
			var newIdx = currentAnimationIdx + delta;
			if (newIdx < 0)
				newIdx = animations.Length - 1;
			if (newIdx >= animations.Length)
				newIdx = 0;

			currentAnimationIdx = newIdx;

			OnAnimationChanged();

			PlayCurrentAnimation();
		}

		protected void OnAnimationChanged()
		{
			if (animations != null)
			{
				string animationName = animations[currentAnimationIdx].Replace('_', ' ');
				onAnimationChanged?.Invoke(animationName);
			}
		}
	}
}

