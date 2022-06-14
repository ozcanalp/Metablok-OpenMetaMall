using System.Collections.Generic;
using ItSeez3D.AvatarSdk.Core;

namespace ItSeez3D.AvatarSdk.Cloud.PipelineTraits
{
	public class CloudTraitsKeeper : IPipelineTraitsKeeper
	{
		public PipelineType DefaultPipeline
		{
			get
			{
				return PipelineType.FACE;
			}
		}

		public Dictionary<PipelineType, PipelineTypeTraits> GetTraits()
		{
			var result = new Dictionary<PipelineType, PipelineTypeTraits>();
			result.Add(PipelineType.HEAD_2_0_BUST_MOBILE, new Bust2Traits());
			result.Add(PipelineType.HEAD_2_0_HEAD_MOBILE, new Head2Traits());
			result.Add(PipelineType.FACE, new FaceTraits());
			result.Add(PipelineType.HEAD_1_2, new HeadTraits());
			result.Add(PipelineType.STYLED_FACE, new StyledFaceTraits());
			result.Add(PipelineType.UMA_MALE, new UmaMaleTraits());
			result.Add(PipelineType.UMA_FEMALE, new UmaFemaleTraits());
			result.Add(PipelineType.FIT_PERSON, new FitPersonTraits());
			result.Add(PipelineType.META_PERSON_MALE, new MetaPersonMaleTraits());
			result.Add(PipelineType.META_PERSON_FEMALE, new MetaPersonFemaleTraits());
			return result;
		}
	}

	public class Head2Traits : Head2AbstractTraits
	{
		public sealed override string PipelineSubtypeName { get { return "head/mobile"; } }
		public sealed override string DisplayName { get { return "Head 2.0 | head/mobile"; } }
		public sealed override PipelineType Type { get { return PipelineType.HEAD_2_0_HEAD_MOBILE; } }
		public sealed override bool IsSkinRecoloringSupported { get { return true; } }
	}

	public class Bust2Traits : Head2AbstractTraits
	{
		public sealed override string PipelineSubtypeName { get { return "bust/mobile"; } }
		public sealed override string DisplayName { get { return "Head 2.0 | bust/mobile"; } }
		public sealed override PipelineType Type { get { return PipelineType.HEAD_2_0_BUST_MOBILE; } }
	}

	public class FaceTraits : PipelineTypeTraits
	{
		public sealed override string PipelineTypeName { get { return "animated_face"; } }
		public sealed override string PipelineSubtypeName { get { return "base/legacy"; } }
		public sealed override bool HaircutsSupported { get { return true; } }
		public sealed override string DisplayName { get { return "Animated Face"; } }
		public sealed override PipelineType Type { get { return PipelineType.FACE; } }
	}

	public class HeadTraits : PipelineTypeTraits
	{
		public override string PipelineTypeName { get { return "head_1.2"; } }
		public override string PipelineSubtypeName { get { return "base/mobile"; } }
		public override bool HaircutsSupported { get { return false; } }
		public override string DisplayName { get { return "Head 1.2"; } }
		public override PipelineType Type { get { return PipelineType.HEAD_1_2; } }
	}

	public class StyledFaceTraits : PipelineTypeTraits
	{
		public sealed override string PipelineTypeName { get { return "animated_face"; } }
		public sealed override string PipelineSubtypeName { get { return "indie/legacy_styled"; } }
		public sealed override bool HaircutsSupported { get { return true; } }
		public sealed override string DisplayName { get { return "Styled Face"; } }
		public sealed override PipelineType Type { get { return PipelineType.STYLED_FACE; } }
	}

	public class UmaMaleTraits : Head2AbstractTraits
	{
		public sealed override string PipelineTypeName { get { return "head_2.0"; } }
		public sealed override string PipelineSubtypeName { get { return "uma2/male"; } }
		public sealed override string DisplayName { get { return "UMA Male"; } }
		public sealed override PipelineType Type { get { return PipelineType.UMA_MALE; } }
	}

	public class UmaFemaleTraits : Head2AbstractTraits
	{
		public sealed override string PipelineTypeName { get { return "head_2.0"; } }
		public sealed override string PipelineSubtypeName { get { return "uma2/female"; } }
		public sealed override string DisplayName { get { return "UMA Female"; } }
		public sealed override PipelineType Type { get { return PipelineType.UMA_FEMALE; } }
	}

	public class FitPersonTraits : Head2AbstractTraits
	{
		public sealed override string PipelineTypeName { get { return "body_0.3"; } }
		public sealed override string PipelineSubtypeName { get { return "mobile"; } }
		public sealed override string DisplayName { get { return "FitPerson"; } }
		public sealed override PipelineType Type { get { return PipelineType.FIT_PERSON; } }
	}

	public class MetaPersonMaleTraits : Head2AbstractTraits
	{
		public sealed override string PipelineTypeName { get { return "body_0.3"; } }
		public sealed override string PipelineSubtypeName { get { return "male"; } }
		public sealed override string DisplayName { get { return "MetaPerson Male"; } }
		public sealed override PipelineType Type { get { return PipelineType.META_PERSON_MALE; } }
	}

	public class MetaPersonFemaleTraits : Head2AbstractTraits
	{
		public sealed override string PipelineTypeName { get { return "body_0.3"; } }
		public sealed override string PipelineSubtypeName { get { return "female"; } }
		public sealed override string DisplayName { get { return "MetaPerson Female"; } }
		public sealed override PipelineType Type { get { return PipelineType.META_PERSON_FEMALE; } }
	}
}
