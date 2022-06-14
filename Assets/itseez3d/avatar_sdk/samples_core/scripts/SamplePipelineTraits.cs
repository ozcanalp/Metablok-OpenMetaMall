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

namespace ItSeez3D.AvatarSdkSamples.SamplePipelineTraits
{
	public abstract class SampleHead2AbstractTraits : PipelineTypeSampleTraits
	{
		private static string BASE_GENERATED_HAIRCUT = "base/generated";
		private static string BASE_GENERATED_HAIRCUT_SHORT = "generated";
		public override bool HaircutsSupported { get { return Type.Traits().HaircutsSupported; } }
		public override bool HaircutNeedsPreview(string haircutId)
		{
			if (haircutId == BASE_GENERATED_HAIRCUT || haircutId == BASE_GENERATED_HAIRCUT_SHORT)
			{
				return false;
			}
			return base.HaircutNeedsPreview(haircutId);
		}
		public override string GetDefaultAvatarHaircut(string avatarCode)
		{
			return BASE_GENERATED_HAIRCUT;
		}
	}

	public class SampleHead2Traits : SampleHead2AbstractTraits
	{
		public override bool isCompatibleWithFullBody { get { return true; } }
		public override PipelineType Type { get { return PipelineType.HEAD_2_0_HEAD_MOBILE; } }
		public override Vector3 ViewerDisplayScale { get { return new Vector3(1.0f, 1.0f, 1.0f); } }
		public override Vector3 ViewerLocalPosition { get { return new Vector3(0.0f, 0.02f, 0.0f); } }
	}

	public class SampleBust2Traits : SampleHead2AbstractTraits
	{
		public override PipelineType Type { get { return PipelineType.HEAD_2_0_BUST_MOBILE; } }
		public override Vector3 ViewerDisplayScale { get { return new Vector3(0.8f, 0.8f, 0.8f); } }
		public override Vector3 ViewerLocalPosition { get { return new Vector3(0.0f, -0.02f, 0.0f); } }
	}

	public class SampleUmaMaleTraits : SampleHead2AbstractTraits
	{
		public override PipelineType Type { get { return PipelineType.UMA_MALE; } }
		public override Vector3 ViewerDisplayScale { get { return new Vector3(5, 5, 5); } }
		public override Vector3 ViewerLocalPosition { get { return new Vector3(0.0f, -8.5f, 0.0f); } }
	}

	public class SampleUmaFemaleTraits : SampleHead2AbstractTraits
	{
		public override PipelineType Type { get { return PipelineType.UMA_FEMALE; } }
		public override Vector3 ViewerDisplayScale { get { return new Vector3(5, 5, 5); } }
		public override Vector3 ViewerLocalPosition { get { return new Vector3(0.0f, -8.5f, 0.0f); } }
	}

	public class SampleFullbodyTraits : SampleHead2AbstractTraits
	{
		public override PipelineType Type { get { return PipelineType.FIT_PERSON; } }
		public override Vector3 ViewerDisplayScale { get { return new Vector3(5, 5, 5); } }
		public override Vector3 ViewerLocalPosition { get { return new Vector3(0.0f, -8.5f, 0.0f); } }
	}

	public class SampleFaceTraits : PipelineTypeSampleTraits
	{
		public override bool isCompatibleWithFullBody { get { return true; } }
		public override bool HaircutsSupported { get { return true; } }
		public override PipelineType Type { get { return PipelineType.FACE; } }
	}

	public class SampleHeadTraits : PipelineTypeSampleTraits
	{
		public override bool HaircutsSupported { get { return false; } }
		public override PipelineType Type { get { return PipelineType.HEAD_1_2; } }
		public override Vector3 ViewerDisplayScale { get { return new Vector3(0.8f, 0.8f, 0.8f); } }
		public override Vector3 ViewerLocalPosition { get { return new Vector3(0.0f, -0.04f, 0.0f); } }
	}
	public class SampleStyledFaceTraits : PipelineTypeSampleTraits
	{
		public override bool HaircutsSupported { get { return true; } }
		public override PipelineType Type { get { return PipelineType.STYLED_FACE; } }
	}

	/// <summary>
	/// Base class where all extensions of PipelineTypeTraits needed in samples are defined
	/// </summary>
	public abstract class PipelineTypeSampleTraits : PipelineTypeTraits
	{
		public sealed override string PipelineTypeName { get { return Type.Traits().PipelineTypeName; } }
		public sealed override string PipelineSubtypeName { get { return Type.Traits().PipelineSubtypeName; } }
		public sealed override string DisplayName { get { return Type.Traits().DisplayName; } }

		public virtual bool isCompatibleWithFullBody { get { return false; } }
		public virtual Vector3 ViewerDisplayScale { get { return new Vector3(1.0f, 1.0f, 1.0f); } }
		public virtual Vector3 ViewerLocalPosition { get { return new Vector3(0.0f, 0.0f, 0.0f); } }
		public virtual string GetDefaultAvatarHaircut(string avatarCode)
		{
			return CoreTools.GetAvatarPredictedHaircut(avatarCode);
		}
		public virtual bool HaircutNeedsPreview(string haircutId)
		{	
			if (haircutId == "bald")
			{
				return false;
			}
			return true;
		}
		public static explicit operator PipelineTypeSampleTraits(PipelineType pipelineType)
		{
			return PipelineSampleTraitsFactory.Instance.GetTraits(pipelineType);
		}
		public static explicit operator PipelineType(PipelineTypeSampleTraits pipelineTraits)
		{
			return pipelineTraits.Type;
		}
		public static bool operator ==(PipelineTypeSampleTraits traits, PipelineType pipelineType)
		{
			return traits.Type == pipelineType;
		}
		public static bool operator !=(PipelineTypeSampleTraits traits, PipelineType pipelineType)
		{
			return traits.Type != pipelineType;
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}

	public static class PipelineTypeExtensions
	{
		public static PipelineTypeSampleTraits SampleTraits(this PipelineType pipelineType)
		{
			return (PipelineTypeSampleTraits)pipelineType;
		}
	}

	public class PipelineSampleTraitsFactory
	{
		private static TraitsFactory<PipelineTypeSampleTraits> traits = null;
		public static TraitsFactory<PipelineTypeSampleTraits> Instance
		{
			get
			{
				if (traits == null)
				{
					traits = new TraitsFactory<PipelineTypeSampleTraits>();
					traits.TraitsDictionary = new Dictionary<PipelineType, PipelineTypeSampleTraits>();
					traits.TraitsDictionary.Add(PipelineType.HEAD_2_0_BUST_MOBILE, new SampleBust2Traits());
					traits.TraitsDictionary.Add(PipelineType.HEAD_2_0_HEAD_MOBILE, new SampleHead2Traits());
					traits.TraitsDictionary.Add(PipelineType.FACE, new SampleFaceTraits());
					traits.TraitsDictionary.Add(PipelineType.HEAD_1_2, new SampleHeadTraits());
					traits.TraitsDictionary.Add(PipelineType.STYLED_FACE, new SampleStyledFaceTraits());
					traits.TraitsDictionary.Add(PipelineType.UMA_MALE, new SampleUmaMaleTraits());
					traits.TraitsDictionary.Add(PipelineType.UMA_FEMALE, new SampleUmaFemaleTraits());
					traits.TraitsDictionary.Add(PipelineType.FIT_PERSON, new SampleFullbodyTraits());
					traits.TraitsDictionary.Add(PipelineType.META_PERSON_MALE, new SampleFullbodyTraits());
					traits.TraitsDictionary.Add(PipelineType.META_PERSON_FEMALE, new SampleFullbodyTraits());
				}
				return traits;
			}
		}
	}
}
