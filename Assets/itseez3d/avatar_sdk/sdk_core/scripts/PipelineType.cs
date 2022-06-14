/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
* You may not use this file except in compliance with an authorized license
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
* See the License for the specific language governing permissions and limitations under the License.
* Written by Itseez3D, Inc. <support@avatarsdk.com>, April 2017
*/

using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace ItSeez3D.AvatarSdk.Core
{
	public enum PipelineType
	{
		/// <summary>
		/// Standard pipeline (bald head with the neck, supports different haircuts, supports blendshapes).
		/// "animated_face" pipeline, "base/legacy" pipeline subtype
		/// </summary>
		FACE,

		/// <summary>
		/// Face pipeline with the cartoonish stylization
		/// "animated_face" pipeline, "indie/legacy_styled" pipeline subtype
		/// </summary>
		STYLED_FACE,

		/// <summary>
		/// Pipeline that generates head with hair and shoulders (supports blendshapes)
		/// "head_1.2" pipeline, "base/mobile" pipeline subtype
		/// </summary>
		HEAD_1_2,

		/// <summary>
		/// "head_2.0" pipeline, "head/mobile" pipeline subtype
		/// </summary>
		HEAD_2_0_HEAD_MOBILE,

		/// <summary>
		/// "head_2.0" pipeline, "bust/mobile" pipeline subtype
		/// </summary>
		HEAD_2_0_BUST_MOBILE,

		/// <summary>
		/// "head_2.0" pipeline, "uma2/male" pipeline subtype
		/// </summary>
		UMA_MALE,

		/// <summary>
		/// "head_2.0" pipeline, "uma2/female" pipeline subtype
		/// </summary>
		UMA_FEMALE,

		/// <summary>
		/// "body_0.3" pipeline, "body/mobile" pipeline subtype
		/// </summary>
		FIT_PERSON,

		/// <summary>
		/// "body_0.3" pipeline, "male" pipeline subtype
		/// </summary>
		META_PERSON_MALE,

		/// <summary>
		/// "body_0.3" pipeline, "female" pipeline subtype
		/// </summary>
		META_PERSON_FEMALE
	}

	/// <summary>
	/// PipelineType extensions
	/// </summary>
	public static class PipelineTypeExtensions
	{
		public static PipelineTypeTraits Traits(this PipelineType pipelineType)
		{
			return (PipelineTypeTraits)pipelineType;
		}

		public static bool IsFullbodyPipeline(this PipelineType pipelineType)
		{
			return pipelineType == PipelineType.FIT_PERSON || pipelineType == PipelineType.META_PERSON_MALE || pipelineType == PipelineType.META_PERSON_FEMALE;
		}
	}

	public interface IPipelineTraitsKeeper
	{
		Dictionary<PipelineType, PipelineTypeTraits> GetTraits();
		PipelineType DefaultPipeline { get; }
	}

	/// <summary>
	/// Class that responsible for keeping instances of pipeline traits
	/// </summary>
	public class PipelineTraitsFactory
	{
		static PipelineTraitsFactory()
		{
			if (keeper == null)
			{
				keeper = AvatarSdkMgr.IoCContainer.Create<IPipelineTraitsKeeper>();
			}
		}
		private static TraitsFactory<PipelineTypeTraits> traits = null;
		private static IPipelineTraitsKeeper keeper = null;
		
		public static TraitsFactory<PipelineTypeTraits> Instance
		{
			get
			{
				
				if(traits == null)
				{
					traits = new TraitsFactory<PipelineTypeTraits>();
					traits.TraitsDictionary = keeper.GetTraits();
				}
				return traits;
			}
		}

		public static PipelineType GetDefaultPipelineType()
		{
			return keeper.DefaultPipeline;
		}
	}

	/// <summary>
	/// Generalized keeper of traits (for convenient work with traits extensions, e.g. SamplePipelineTraits)
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class TraitsFactory<T> where T : PipelineTypeTraits
	{
		public Dictionary<PipelineType, T> TraitsDictionary = null;
		public bool IsHaircutSupported(string pipeline)
		{
			return TraitsDictionary.FirstOrDefault(t => t.Value.PipelineTypeName.Equals(pipeline)).Value.HaircutsSupported;
		}

		public T GetTraitsFromPipelineName(string pipelineName)
		{
			T result = null;
			result = TraitsDictionary.FirstOrDefault(element => element.Value.PipelineTypeName.Equals(pipelineName)).Value;
			if (result == null)
				Debug.LogErrorFormat("Unable to get PipelineTypeTraits for pipeline: {0}", pipelineName);
			return result;
		}
 
		public T GetTraitsFromPipelineName(string pipelineName, string pipelineSubtypeName)
		{
			T result = null;
			foreach(var pair in TraitsDictionary)
			{
				T traits = pair.Value;
				if (traits.IsSuppoted && traits.PipelineTypeName.Equals(pipelineName) && traits.PipelineSubtypeName.Equals(pipelineSubtypeName))
				{
					result = traits;
					break;
				}
			}
			if (result == null)
				Debug.LogErrorFormat("Unable to get PipelineTypeTraits for pipeline: {0}, pipelineSubtype: {1}", pipelineName, pipelineSubtypeName);
			return result;
		}

		public T GetTraitsFromAvatarCode(string avatarCode)
		{
			PipelineType pipelineType = CoreTools.LoadPipelineType(avatarCode);
			return TraitsDictionary[pipelineType];
		}

		public bool IsTypeSupported(PipelineType pipelineType)
		{
			return TraitsDictionary.ContainsKey(pipelineType);
		}

		public T GetTraits(PipelineType pipelineType)
		{
			if (!TraitsDictionary.ContainsKey(pipelineType))
			{
				throw new System.Exception("Pipeline type " + pipelineType.ToString() + " is not supported");
			}
			return TraitsDictionary[pipelineType];
		}
	}

	/// <summary>
	/// Traits class contains properties of pipeline that may be used anywhere. For the sake of separation of concerns class may be extended 
	/// (in a way SamplePipelineTraits extends it for providing pipeline type's specific settings for all samples)
	/// </summary>
	public abstract class PipelineTypeTraits
	{
		public abstract string PipelineTypeName { get; }
		public abstract string PipelineSubtypeName { get; }
		public abstract bool HaircutsSupported { get; }
		public abstract string DisplayName { get; }
		public abstract PipelineType Type { get; }
		public virtual bool IsSkinRecoloringSupported { get { return false; } }

		public virtual bool IsPointcloudApplicableToHaircut(string haircutId)
		{
			return true;
		}

		public virtual bool IsSuppoted { get { return PipelineTraitsFactory.Instance.IsTypeSupported(Type); } }

		public static explicit operator PipelineTypeTraits(PipelineType pipelineType)
		{
			return PipelineTraitsFactory.Instance.GetTraits(pipelineType);
		}
	}

	public abstract class Head2AbstractTraits : PipelineTypeTraits
	{
		private static string BASE_GENERATED_HAIRCUT = "base/generated";
		private static string BASE_GENERATED_HAIRCUT_SHORT = "generated";
		public override string PipelineTypeName { get { return "head_2.0"; } }
		public override bool HaircutsSupported { get { return true; } }
		protected static bool isNativeHaircut(string id)
		{
			return id.Equals(BASE_GENERATED_HAIRCUT) || id.Equals(BASE_GENERATED_HAIRCUT_SHORT);
		}
		public override bool IsPointcloudApplicableToHaircut(string haircutId)
		{
			return !isNativeHaircut(haircutId);
		}
	}
}
