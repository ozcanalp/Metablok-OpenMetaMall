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
using System.Collections.Generic;

namespace ItSeez3D.AvatarSdk.Core
{
	/// <summary>
	/// Strings that occur in the plugin and can be potentially displayed on the screen (for example when
	/// showing request progress, see the samples).
	/// This file contains strings used in both "LocalCompute" and "Cloud" versions of the plugin.
	/// </summary>
	public class Strings
	{
		#region Server calculation status

		public static string Pending { get { return "Pending"; } }

		public static string Uploading { get { return "Preprocessing"; } }

		public static string Queued { get { return "Queued"; } }

		public static string Computing { get { return "Computing"; } }

		public static string Failed { get { return "Failed"; } }

		public static string TimedOut { get { return "Timed Out"; } }

		public static string Completed { get { return "Completed"; } }

		public static readonly List<string> GoodFinalStates = new List<string> () { Strings.Completed };

		public static readonly List<string> BadFinalStates = new List<string> () { Strings.Failed, Strings.TimedOut };

		public static List<string> FinalStates {
			get {
				var finalStates = new List<string> ();
				finalStates.AddRange (GoodFinalStates);
				finalStates.AddRange (BadFinalStates);
				return finalStates;
			}
		}

		#endregion

		#region Web API

		public static string Authentication { get { return "Authentication"; } }

		public static string RequestingApiToken { get { return "Requesting Avatar API access"; } }

		public static string UploadingPhoto { get { return "Uploading photo"; } }

		public static string GettingAvatarInfo { get { return "Getting avatar info"; } }

		public static string GettingAvatarExportInfo { get { return "Getting avatar export info"; } }

		public static string RequestingHaircutInfo { get { return "Requesting haircut info"; } }

		public static string RequestingTextureInfo { get { return "Requesting texture info"; } }

		public static string RequestingModelInfo { get { return "Requesting model info"; } }

		public static string RequestingAvatarExports { get { return "Requesting avatar exports"; } }

		public static string DownloadingThumbnail { get { return "Downloading thumbnail"; } }

		public static string DownloadingHeadMesh { get { return "Downloading head mesh"; } }

		public static string DownloadingHeadTexture { get { return "Downloading head texture"; } }

		public static string DownloadingHaircut { get { return "Downloading haircut"; } }

		public static string DownloadingOutfit { get { return "Downloading outfit"; } }

		public static string DownloadingHaircutMesh { get { return "Downloading haircut mesh"; } }

		public static string DownloadingAllHaircuts { get { return "Downloading all available haircuts"; } }

		public static string DownloadingAllHaircutsTextures { get { return "Downloading all haircuts textures"; } }

		public static string DownloadingHaircutTexture { get { return "Downloading haircut texture"; } }

		public static string DownloadingHaircutPointCloud { get { return "Downloading haircut points"; } }

		public static string DownloadingHaircutPreview { get { return "Downloading haircut preview"; } }

		public static string DownloadingAllHaircutPointClouds { get { return DownloadingHaircutPointCloud; } }

		public static string DownloadingBlendshapes { get { return "Downloading blendshapes"; } }

		public static string DownloadingUmaBones { get { return "Downloading UMA bones data"; } }

		public static string DownloadingTextures { get { return "Downloading textures"; } }

		public static string GettingAvatarList { get { return "Getting list of avatars"; } }

		public static string EditingAvatar { get { return "Editing avatar"; } }

		public static string DeletingAvatarOnServer { get { return "Deleting avatar"; } }

		public static string RegisteringPlayerID { get { return "Registering player"; } }

		public static string PerformingRequest { get { return "Performing request"; } }

		#endregion

		#region save/load/delete files

		public static string SavingFiles { get { return "Saving files to disk"; } }

		public static string LoadingFiles { get { return "Loading files from disk"; } }

		public static string DeletingAvatarFiles { get { return "Deleting avatar files"; } }

		public static string DeletingAvatarFile { get { return "Deleting avatar file"; } }

		#endregion

		#region zip/unzip

		public static string UnzippingFile { get { return "Unzipping mesh data"; } }

		public static string CreatingZipArchive { get { return "Creating ZIP archive"; } }

		#endregion

		#region Ply/Obj/mesh utils

		public static string ParsingMeshData { get { return "Parsing mesh data"; } }

		public static string ParsingPoints { get { return "Parsing points"; } }

		public static string PreparingObjModel { get { return "Preparing OBJ model"; } }

		#endregion

		#region Higher-level API

		public static string RequestingAvatarInfo { get { return "Requesting avatar info"; } }

		public static string StartingCalculations { get { return "Starting calculations"; } }

		public static string InitializingAvatar { get { return "Initializing avatar"; } }

		public static string GeneratingAvatar { get { return "Generating avatar"; } }

		public static string GeneratingHaircuts { get { return "Generating haircuts"; } }

		public static string GeneratingAdditionalAssets { get { return "Generating additional assets"; } }

		public static string ComputingAvatar { get { return "Computing"; } }

		public static string ComputingExport { get { return "Computing export"; } }

		public static string DownloadingAvatar { get { return "Downloading"; } }

		public static string DownloadingZip { get { return "Downloading archive"; } }

		public static string LoadingAvatar { get { return "Loading avatar"; } }

		public static string LoadingTexture { get { return "Loading texture"; } }

		public static string GettingAvatar { get { return "Getting avatar"; } }

		public static string GettingHeadMesh { get { return "Getting head mesh"; } }

		public static string GettingTexture { get { return "Getting texture"; } }

		public static string GettingBlendshapes { get { return "Getting blendshapes"; } }

		public static string GettingHaircutsPointClouds { get { return "Gettings haircuts point clouds"; } }

		public static string GettingModelInfo { get { return "Gettings model info"; } }

		public static string LoadingHeadMesh { get { return "Loading head mesh"; } }

		public static string GettingHaircut { get { return "Getting haircut"; } }

		public static string GettingOutfit { get { return "Getting outfit"; } }

		public static string GettingAvailableHaircuts { get { return "Getting available haircuts"; } }

		public static string GettingHaircutPointCloud { get { return "Getting haircut points"; } }

		public static string GettingHaircutInfo { get { return "Getting haircut info"; } }

		public static string GettingHaircutMesh { get { return "Getting haircut mesh"; } }

		public static string GettingHaircutPreview { get { return "Getting haircut preview"; } }

		public static string LoadingHaircut { get { return "Loading haircut"; } }

		public static string LoadingOutfit { get { return "Loading outfit"; } }

		public static string ShowingHaircut { get { return "Showing haircut"; } }

		public static string ShowingOutfit { get { return "Showing outfit"; } }

		public static string GettingAvatarState { get { return "Getting avatar state"; } }

		public static string GettingAvailableParameters { get { return "Gettings available parameters"; } }

		public static string GettingParametersList { get { return "Getting parameters list"; } }

		public static string GettingExportParametersList { get { return "Getting export parameters list"; } }

		public static string VerifyingPipelineSupporting { get { return "Verifying for pipeline supporting"; } }

		#endregion

		#region Local Compute SDK

		public static string InitializingSession { get { return "Initializing session"; } }

		public static string LoadingResources { get { return "Loading resources"; } }

		public static string ExtractingHaircut { get { return "Extracting haircut"; } }

		public static string ExtractingHaircutPreview { get { return "Extracting haircut preview"; } }

		public static string LoadingAnimations { get { return "Loading animations"; } }

		public static string ParsingBlendshapes { get { return "Parsing blendshapes"; } }

		public static string GeneratingLODMesh { get { return "Generating LOD mesh"; } }

        public static string CreatingSelfie { get { return "Creating selfie request"; } }

        public static string RetrievingQrCode { get { return "Retrieving selfie QR code"; } }

		public static string RetrievingSelfieFile { get { return "Retrieving selfie file"; } }

		#endregion
	}

	/// <summary>
	/// SDK uses this interface to obtain strings that can potentially be displayed on the screen.
	/// You can override this interface and provide instance of your implementation in the AvatarSdkMgr.Init().
	/// This will allow you to replace certain strings with your own, or add translation, etc.
	/// </summary>
	public abstract class IStringManager
	{
		public abstract string GetString (string s);
	}

	/// <summary>
	/// This is an example of a user-defined IStringManager implementation that translates the strings displayed in
	/// UI. Feel free to derive from this class and implement your own Translate method.
	/// </summary>
	public abstract class TranslationStringManager : IStringManager
	{
		protected abstract string Translate (string s);

		public override string GetString (string s)
		{
			return Translate (s);
		}
	}

	/// <summary>
	/// Default string manager implementation (just returns the same string).
	/// </summary>
	public class DefaultStringManager : IStringManager
	{
		public override string GetString (string s)
		{
			return s;
		}
	}
}

