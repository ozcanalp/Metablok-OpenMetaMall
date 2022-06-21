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
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ItSeez3D.AvatarSdk.Core;
using System.IO;
using System.Text;

namespace ItSeez3D.AvatarSdk.Cloud
{
	using ParametersSubsetRequest = Dictionary<ComputationParametersSubset, AsyncRequest<string>>;

	/// <summary>
	/// Implementation of the IAvatarProvider for cloud version of the Avatar SDK.
	/// </summary>
	public class CloudAvatarProvider : IAvatarProvider
	{
		protected Connection connection = new Connection();

		// cached avatar data to reduce requests to the server
		protected Dictionary<string, AvatarData> avatarsDataCache = new Dictionary<string, AvatarData>();

		// cached haircuts data
		protected Dictionary<string, AvatarHaircutData[]> haircutsDataCache = new Dictionary<string, AvatarHaircutData[]>();

		// Cached resources
		protected Dictionary<PipelineType, ParametersSubsetRequest> avatarParametersCache = null;


		/// <summary>
		/// This is generally not for production use.
		/// Enabling this boolean variable enables you to skip the authentication and still use some of the "offline" features of this avatar provider
		/// (such as loading a model from disk).
		/// However, you won't be able to create new avatars or load data from server.
		/// </summary>
		private bool noInternetMode = false;

		#region Constructor
		public CloudAvatarProvider()
		{
			UseCache = true;
			avatarParametersCache = new Dictionary<PipelineType, ParametersSubsetRequest>();

			foreach (PipelineType type in Enum.GetValues(typeof(PipelineType)))
			{
				avatarParametersCache.Add(type, new ParametersSubsetRequest());
			}

		}
		#endregion

		#region IAvatarProvider

		public bool IsInitialized { get { return connection.IsAuthorized; } }

		/// <summary>
		/// Performs authorization on the server
		/// </summary>
		public AsyncRequest InitializeAsync()
		{
			if (noInternetMode)
				return new AsyncRequest { IsDone = true };

			// Obtain auth token asynchronously. This code will also create PlayerUID and
			// store it in persistent storage. Auth token and PlayerUID are required all further HTTP requests.
			return connection.AuthorizeAsync();
		}

		/// <summary>
		/// Waits while the avatar is being calulated. Calculations start automatically after the photo was loaded to the server.
		/// </summary>
		public AsyncRequest StartAndAwaitAvatarCalculationAsync(string avatarCode)
		{
			var request = new AsyncRequest<AvatarData>(AvatarSdkMgr.Str(Strings.GeneratingAvatar));
			AvatarSdkMgr.SpawnCoroutine(StartAndAwaitAvatarCalculationFunc(avatarCode, request));
			return request;
		}

		public AsyncRequest GenerateHaircutsAsync(string avatarCode, List<string> haircutsList)
		{
			throw new NotImplementedException("GenerateHaircutsAsync isn't implemented for Cloud version");
		}

		/// <summary>
		/// Downloads avatar files and stores them on disk.
		/// </summary>
		/// <param name="avatarCode">Avatar code</param>
		/// <param name="withHaircutPointClouds">If True, haircut point clouds will be downloaded.</param>
		/// <param name="withBlendshapes">If true, blendshapes will be downloaded.</param>
		/// <returns></returns>
		public AsyncRequest MoveAvatarModelToLocalStorageAsync(string avatarCode, bool withHaircutPointClouds, bool withBlendshapes, MeshFormat format = MeshFormat.PLY)
		{
			var request = new AsyncRequest<AvatarData>(AvatarSdkMgr.Str(Strings.DownloadingAvatar));
			AvatarSdkMgr.SpawnCoroutine(MoveAvatarModelToLocalStorage(avatarCode, withHaircutPointClouds, withBlendshapes, format, request));
			return request;
		}

		/// <summary>
		/// Creates TexturedMesh of the head for a given avatar.
		/// If required files (mesh and texture) don't exist on disk, it downloads them from the cloud.
		/// </summary>
		/// <param name="avatarCode">code of the loaded avatar</param>
		/// <param name="withBlendshapes">blendshapes will be added to mesh</param>
		/// <param name="additionalTextureName">Name of the texture that should be applied intead of default</param>
		public AsyncRequest<TexturedMesh> GetHeadMeshAsync(string avatarCode, bool withBlendshapes, int detailsLevel = 0, MeshFormat format = MeshFormat.PLY,
			string additionalTextureName = null)
		{
			var request = new AsyncRequest<TexturedMesh>(AvatarSdkMgr.Str(Strings.LoadingHeadMesh));
			AvatarSdkMgr.SpawnCoroutine(GetHeadMeshFunc(avatarCode, withBlendshapes, detailsLevel, format, additionalTextureName, request));
			return request;
		}

		/// <summary>
		/// Returns avatar texture by name or standard texture if name isn't specified. 
		/// If the textures doesn't exist, it will be downloaded from the cloud
		/// </summary>
		/// <param name="avatarCode">avatar code</param>
		/// <param name="textureName">Texture name or pass null for standard texture.</param>
		public AsyncRequest<Texture2D> GetTextureAsync(string avatarCode, string textureName)
		{
			var request = new AsyncRequest<Texture2D>(AvatarSdkMgr.Str(Strings.GettingTexture));
			AvatarSdkMgr.SpawnCoroutine(GetTextureFunc(avatarCode, textureName, request));
			return request;
		}

		/// <summary>
		/// Returns identities of all haircuts available for the avatar
		/// </summary>
		public AsyncRequest<string[]> GetHaircutsIdAsync(string avatarCode)
		{
			var request = new AsyncRequest<string[]>(AvatarSdkMgr.Str(Strings.GettingAvailableHaircuts));
			AvatarSdkMgr.SpawnCoroutine(GetHaircutsIdFunc(avatarCode, request));
			return request;
		}

		/// <summary>
		/// Creates TexturedMesh of the haircut.
		/// If any of the required files doesn't exist it downloads them from the cloud and saves on the disk.
		/// </summary>
		/// <param name="avatarCode">Avatar code</param>
		/// <param name="haircutName">Haircut identity</param>
		public AsyncRequest<TexturedMesh> GetHaircutMeshAsync(string avatarCode, string haircutId)
		{
			var request = new AsyncRequest<TexturedMesh>(AvatarSdkMgr.Str(Strings.GettingHaircutMesh));
			AvatarSdkMgr.SpawnCoroutine(GetHaircutMeshFunc(avatarCode, haircutId, request));
			return request;
		}

		/// <summary>
		/// Downloads from the server haircut preview image and saves it locally.
		/// Note: this method isn't implemented yet.
		/// </summary>
		/// <param name="haircutId">Haircut identity</param>
		public AsyncRequest<byte[]> GetHaircutPreviewAsync(string avatarCode, string haircutId)
		{
			var request = new AsyncRequest<byte[]>(AvatarSdkMgr.Str(Strings.GettingHaircutPreview));
			AvatarSdkMgr.SpawnCoroutine(GetHaircutPreviewFunc(avatarCode, haircutId, request));
			return request;
		}

		/// <summary>
		/// Requests from the server identities of the latest "maxItems" avatars.
		/// </summary>
		public AsyncRequest<string[]> GetAllAvatarsAsync(int maxItems)
		{
			var request = new AsyncRequest<string[]>(AvatarSdkMgr.Str(Strings.GettingAvatarList));
			AvatarSdkMgr.SpawnCoroutine(GetAllAvatarsFunc(maxItems, request));
			return request;
		}

		/// <summary>
		/// Requests server to delete all data permanently and deletes local avatar files.
		/// </summary>
		public AsyncRequest DeleteAvatarAsync(string avatarCode)
		{
			var request = new AsyncRequest(AvatarSdkMgr.Str(Strings.DeletingAvatarFiles));
			AvatarSdkMgr.SpawnCoroutine(DeleteAvatarFunc(avatarCode, request));
			return request;
		}

		/// <summary>
		/// Requests server to check if the pipeline is available for user
		/// </summary>
		public AsyncRequest<bool> IsPipelineSupportedAsync(PipelineType pipelineType)
		{
			var request = new AsyncRequest<bool>(AvatarSdkMgr.Str(Strings.VerifyingPipelineSupporting));
			AvatarSdkMgr.SpawnCoroutine(IsPipelineSupportedFunc(pipelineType, request));
			return request;
		}

		/// <summary>
		/// Requests server for available parameters
		/// </summary>
		public AsyncRequest<ComputationParameters> GetParametersAsync(ComputationParametersSubset parametersSubset, PipelineType pipelineType)
		{
			var request = new AsyncRequest<ComputationParameters>(AvatarSdkMgr.Str(Strings.GettingParametersList));
			AvatarSdkMgr.SpawnCoroutine(GetParametersFunc(parametersSubset, pipelineType, request));
			return request;
		}
		#endregion

		#region IDisposable
		/// <summary>
		/// Empty method in Cloud version
		/// </summary>
		public virtual void Dispose() { }
		#endregion

		#region public methods
		/// <summary>
		/// Get the created connection instance.
		/// </summary>
		public Connection Connection { get { return connection; } }

		/// <summary>
		/// To avoid redundant requests to the server, some type of data may be cached.
		/// This property determinates whether the data cache is enabled. Default value in True.
		/// </summary>
		public bool UseCache { get; set; }

		/// <summary>
		/// Get avatar information by code. Firstly finds data in cache. If there is no data in cache, requests it from the server
		/// </summary>
		public AsyncRequest<AvatarData> GetAvatarAsync(string avatarCode)
		{
			var request = new AsyncRequest<AvatarData>(AvatarSdkMgr.Str(Strings.RequestingAvatarInfo));
			AvatarSdkMgr.SpawnCoroutine(GetAvatarFunc(avatarCode, request));
			return request;
		}

		/// <summary>
		/// Initializes avatar and uploads photo to the server.
		/// </summary>
		/// <param name="photoBytes">Photo bytes (jpg or png encoded).</param>
		/// <param name="name">Name of the avatar</param>
		/// <param name="description">Description of the avatar</param>
		/// <param name="pipeline">Calculation pipeline to use</param>
		/// <param name="computationParameters">Computation parameters</param>
		/// <returns>Avatar unique code</returns>
		public AsyncRequest<string> InitializeAvatarAsync(byte[] photoBytes, string name, string description, PipelineType pipeline = PipelineType.FACE,
			ComputationParameters computationParameters = null)
		{
			var request = new AsyncRequest<string>(AvatarSdkMgr.Str(Strings.InitializingAvatar));
			AvatarSdkMgr.SpawnCoroutine(InitializeAvatarFunc(photoBytes, name, description, pipeline, computationParameters, request));
			return request;
		}

		/// <summary>
		/// Download all avatar files, unzip and save to disk.
		/// </summary>
		/// <param name="connection">Connection session.</param>
		/// <param name="avatar">Avatar to download.</param>
		/// <param name="withHaircutPointClouds">If set to true, download all haircut point clouds too.</param>
		/// <param name="withBlendshapes">If set to true, download blendshapes too.</param>
		public AsyncRequest DownloadAndSaveAvatarModelAsync(AvatarData avatar, bool withHaircutPointClouds, bool withBlendshapes, int detailsLevel = 0, 
			MeshFormat format = MeshFormat.PLY)
		{
			var request = new AsyncRequest<AvatarData>(AvatarSdkMgr.Str(Strings.DownloadingAvatar));
			AvatarSdkMgr.SpawnCoroutine(DownloadAndSaveAvatarModel(avatar, withHaircutPointClouds, withBlendshapes, detailsLevel, format, request));
			return request;
		}

		/// <summary>
		/// Download avatar mesh, unzip and save to disk.
		/// </summary>
		public AsyncRequest DownloadAndSaveMeshAsync(AvatarData avatarData, int detailsLevel = 0, MeshFormat meshFormat = MeshFormat.PLY)
		{
			var request = new AsyncRequest<AvatarData>(AvatarSdkMgr.Str(Strings.GettingHeadMesh));
			AvatarSdkMgr.SpawnCoroutine(DownloadAndSaveMeshFunc(avatarData, detailsLevel, meshFormat, request));
			return request;
		}

		/// <summary>
		/// Download avatar texture and save to disk.
		/// <param name="textureName">Name of the additional texture you need to download.
		/// Use null if you need to download default texture.</param>
		/// </summary>
		public AsyncRequest DownloadAndSaveTextureAsync(AvatarData avatarData, string textureName = null)
		{
			var request = new AsyncRequest<AvatarData>(AvatarSdkMgr.Str(Strings.GettingTexture));
			AvatarSdkMgr.SpawnCoroutine(DownloadAndSaveTextureFunc(avatarData, textureName, null, request));
			return request;
		}

		/// <summary>
		/// Download blendshapes, unzip and save to disk.
		/// </summary>
		public AsyncRequest DownloadAndSaveBlendshapesAsync(AvatarData avatarData, int detailsLevel = 0)
		{
			var request = new AsyncRequest<AvatarData>(AvatarSdkMgr.Str(Strings.GettingBlendshapes));
			AvatarSdkMgr.SpawnCoroutine(DownloadAndSaveBlendshapesFunc(avatarData, detailsLevel, request));
			return request;
		}

		/// <summary>
		/// Download point clouds for all haircuts and save them to disk
		/// </summary>
		public AsyncRequest DownloadAndSaveHaircutsPointCloudsAsync(AvatarData avatarData)
		{
			var request = new AsyncRequest(AvatarSdkMgr.Str(Strings.GettingHaircutsPointClouds));
			AvatarSdkMgr.SpawnCoroutine(DownloadAndSaveHaircutsPointCloudsFunc(avatarData, request));
			return request;
		}

		/// <summary>
		/// Get haircut info
		/// </summary>
		/// <param name="avatarCode">Avatar code</param>
		/// <param name="haircutId">Haircut identity</param>
		public AsyncRequest<AvatarHaircutData> GetHaircutDataAsync(string avatarCode, string haircutId)
		{
			var request = new AsyncRequest<AvatarHaircutData>(AvatarSdkMgr.Str(Strings.GettingHaircutInfo));
			AvatarSdkMgr.SpawnCoroutine(GetHaircutDataFunc(avatarCode, haircutId, request));
			return request;
		}

		/// <summary>
		/// Download and save all required haircut data if it isn't exist in local storage.
		/// </summary>
		public AsyncRequest DownloadAndSaveHaircutDataAsync(string avatarCode, string haircutId)
		{
			var request = new AsyncRequest(AvatarSdkMgr.Str(string.Format("Downloading {0} haircut", haircutId)));
			AvatarSdkMgr.SpawnCoroutine(DownloadAndSaveHaircutDataFunc(avatarCode, haircutId, request));
			return request;
		}

		public AsyncRequest DownloadAndSaveAllHaircutsAsync(string avatarCode)
		{
			var request = new AsyncRequest(AvatarSdkMgr.Str(Strings.DownloadingAllHaircuts));
			AvatarSdkMgr.SpawnCoroutine(DownloadAndSaveAllHaircutsFunc(avatarCode, request));
			return request;
		}

		/// <summary>
		/// Download haircut mesh and texture and save them to disk
		/// </summary>
		public AsyncRequest DownloadAndSaveHaircutMeshAsync(AvatarHaircutData haircutData, string meshDirectoryPath, string textureFileNamePath)
		{
			var request = new AsyncRequest(AvatarSdkMgr.Str(Strings.GettingHaircutMesh));
			AvatarSdkMgr.SpawnCoroutine(DownloadAndSaveHaircutMeshFunc(haircutData, request, meshDirectoryPath, textureFileNamePath));
			return request;
		}

		/// <summary>
		/// Download haircut points and save them to disk
		/// </summary>
		public AsyncRequest DownloadAndSaveHaircutPointsAsync(string avatarCode, AvatarHaircutData haircutData)
		{
			var request = new AsyncRequest(AvatarSdkMgr.Str(Strings.GettingHaircutPointCloud));
			AvatarSdkMgr.SpawnCoroutine(DownloadAndSaveHaircutPointsFunc(avatarCode, haircutData, request));
			return request;
		}

		/// <summary>
		/// Download haircut preview and save it to disk
		/// </summary>
		public AsyncRequest DownloadAndSaveHaircutPreviewAsync(string haircutId, AvatarHaircutData haircutData)
		{
			var request = new AsyncRequest(AvatarSdkMgr.Str(Strings.GettingHaircutPreview));
			AvatarSdkMgr.SpawnCoroutine(DownloadAndSaveHaircutPreviewFunc(haircutId, haircutData, request));
			return request;
		}

		/// <summary>
		/// Process blendshapes slightly differently compared to other zips (for compatibility reasons).
		/// Blendshapes are unzipped not just in avatar directory, but in their own personal folder.
		/// </summary>
		/// <param name="blendshapesZip">Full path to blendshapes zip archive.</param>
		/// <param name="avatarCode">Avatar identifier to determine the correct unzip location.</param>
		/// <param name="levelOfDetails">Level of details</param>
		public AsyncRequest<string> UnzipBlendshapesAsync(string blendshapesZip, string avatarCode, int levelOfDetails = 0)
		{
			var blendshapesDir = AvatarSdkMgr.Storage().GetAvatarSubdirectory(avatarCode, AvatarSubdirectory.BLENDSHAPES, levelOfDetails);
			return CoreTools.UnzipFileAsync(blendshapesZip, blendshapesDir);
		}

		public virtual AsyncRequest DownloadModelInfoAsync(string avatarCode)
		{
			var request = new AsyncRequest(AvatarSdkMgr.Str(Strings.GettingModelInfo));
			AvatarSdkMgr.SpawnCoroutine(DownloadModelInfoFunc(avatarCode, request));
			return request;
		}
		#endregion

		#region private methods

		/// <summary>
		/// InitializeAvatarAsync implementation
		/// </summary>
		private IEnumerator InitializeAvatarFunc(byte[] photoBytes, string name, string description, PipelineType pipeline,
			ComputationParameters computationParameters, AsyncRequest<string> request)
		{
			if (pipeline.IsFullbodyPipeline())
			{
				request.SetError("To initialize Fullbody avatar, use CloudFullbodyAvatarProvider.InitializeFullbodyAvatarAsync method");
				yield break;
			}

			byte[] preprocessedBytes = photoBytes;
			if (AvatarSdkMgr.Settings.ForceRescaleLargeImages)
			{
				var downscaleImage = ImageUtils.DownscaleImageIfNeedAsync(photoBytes);
				yield return request.AwaitSubrequest(downscaleImage, finalProgress: 0.8f);
				preprocessedBytes = downscaleImage.Result;
			}

			// uploading photo and registering new avatar on the server
			var createAvatar = connection.CreateAvatarWithPhotoAsync(name, description, preprocessedBytes, pipeline, computationParameters);

			// Wait until async request is completed (without blocking the main thread).
			// Instead of using AwaitSubrequest we could just use `yield return createAvatar;`
			// AwaitSubrequest is a helper function that allows to track progress on composite
			// requests automatically. It also provides info for the caller about current subrequest
			// (and it's progress) and propagetes error from subrequest to the parent request.
			// finalProgress is a value between 0 and 1, a desired progress of parent request when given
			// subrequest is completed.
			yield return request.AwaitSubrequest(createAvatar, finalProgress: 0.99f);

			// must check whether request was successful before proceeding
			if (request.IsError)
				yield break;

			string avatarCode = createAvatar.Result.code;

			// save photo for later use
			var savePhoto = CoreTools.SaveAvatarFileAsync(photoBytes, avatarCode, AvatarFile.PHOTO);
			// save pipeline type
			CoreTools.SavePipelineType(pipeline, avatarCode);
			yield return request.AwaitSubrequests(1.0f, savePhoto);

			// again, must check for the error, there's no point in proceeding otherwise
			if (request.IsError)
				yield break;

			request.Result = avatarCode;
			request.IsDone = true;
		}

		/// <summary>
		/// StartAndAwaitAvatarCalculationAsync implementation
		/// </summary>
		protected virtual IEnumerator StartAndAwaitAvatarCalculationFunc(string avatarCode, AsyncRequest<AvatarData> request)
		{
			var avatarRequest = GetAvatarAsync(avatarCode);
			yield return avatarRequest.Await();
			if (avatarRequest.IsError)
			{
				request.SetError(avatarRequest.ErrorMessage);
				yield break;
			}

			var awaitCalculations = connection.AwaitAvatarCalculationsAsync(avatarRequest.Result);
			yield return request.AwaitSubrequest(awaitCalculations, finalProgress: 1.0f);
			if (request.IsError)
				yield break;

			if (Strings.BadFinalStates.Contains(awaitCalculations.Result.status))
			{
				request.SetError(string.Format("Avatar {0} calculation finished with status: {1}", awaitCalculations.Result.code, awaitCalculations.Result.status));
				yield break;
			}
			else
			{
				request.Result = awaitCalculations.Result;
				if (UseCache)
					avatarsDataCache[avatarCode] = awaitCalculations.Result;
			}

			request.IsDone = true;
		}

		/// <summary>
		/// MoveToLocalStorageAvatarModelAsync implementation
		/// </summary>
		private IEnumerator MoveAvatarModelToLocalStorage(string avatarCode, bool withHaircutPointClouds, bool withBlendshapes, MeshFormat format, AsyncRequest request)
		{
			var avatarRequest = GetAvatarAsync(avatarCode);
			yield return avatarRequest.Await();
			if (avatarRequest.IsError)
			{
				request.SetError(avatarRequest.ErrorMessage);
				yield break;
			}

			yield return DownloadAndSaveAvatarModel(avatarRequest.Result, withHaircutPointClouds, withBlendshapes, 0, format, request);
		}

		private IEnumerator DownloadModelInfoFunc(string avatarCode, AsyncRequest request)
		{
			var modelInfoRequest = Connection.GetModelInfoAsync(avatarCode);
			yield return modelInfoRequest.Await();
			if (modelInfoRequest.IsError)
			{
				request.SetError(modelInfoRequest.ErrorMessage);
				yield break;
			}

			var fileName = AvatarSdkMgr.Storage().GetAvatarFilename(avatarCode, AvatarFile.MODEL_JSON);
			File.WriteAllText(fileName, modelInfoRequest.Result);
			request.IsDone = true;
		}

		/// <summary>
		/// DownloadAndSaveAvatarModelAsync implementation.
		/// </summary>
		private IEnumerator DownloadAndSaveAvatarModel(AvatarData avatar, bool withHaircutPointClouds, bool withBlendshapes, int detailsLevel, 
			MeshFormat meshFormat, AsyncRequest request)
		{
			List<AsyncRequest> requests = new List<AsyncRequest>();
			requests.Add(DownloadAndSaveMeshAsync(avatar, detailsLevel, meshFormat));
			requests.Add(DownloadAndSaveTextureAsync(avatar));
			requests.Add(DownloadModelInfoAsync(avatar.code));
			if (withHaircutPointClouds)
				requests.Add(DownloadAndSaveHaircutsPointCloudsAsync(avatar));
			if (withBlendshapes)
				requests.Add(DownloadAndSaveBlendshapesAsync(avatar, detailsLevel));

			for (int i = 0; i < requests.Count; i++)
			{
				yield return request.AwaitSubrequest(requests[i], ((float)(i + 1)) / requests.Count);
				if (request.IsError)
					yield break;
			}

			request.IsDone = true;
		}

		/// <summary>
		/// GetHaircutDataAsync implementation
		/// </summary>
		private IEnumerator GetHaircutDataFunc(string avatarCode, string haircutId, AsyncRequest<AvatarHaircutData> request)
		{
			haircutId = CoreTools.GetShortHaircutId(haircutId);

			bool takeFromCache = UseCache && haircutsDataCache.ContainsKey(avatarCode);
			if (takeFromCache)
				request.Result = haircutsDataCache[avatarCode].FirstOrDefault(h => string.Compare(h.identity, haircutId) == 0);
			else
			{
				// get AvatarData firstly.
				// If you would like to make multiple requests for getting haircut data, it is better to get AvatarData only once and store it somewhere
				var avatarRequest = GetAvatarAsync(avatarCode);
				yield return avatarRequest.Await();
				if (avatarRequest.IsError)
				{
					request.SetError(avatarRequest.ErrorMessage);
					yield break;
				}

				var haircutInfoRequest = connection.GetHaircutsAsync(avatarRequest.Result);
				yield return request.AwaitSubrequest(haircutInfoRequest, 0.9f);
				if (request.IsError)
					yield break;

				if (UseCache && !haircutsDataCache.ContainsKey(avatarCode))
					haircutsDataCache.Add(avatarCode, haircutInfoRequest.Result);

				AvatarHaircutData haircutData = haircutInfoRequest.Result.FirstOrDefault(h => string.Compare(h.identity, haircutId) == 0);
				if (haircutData == null)
				{
					request.SetError(string.Format("There is no {0} haircut for avatar with code: {1}", haircutId, avatarCode));
					yield break;
				}
				request.Result = haircutData;
			}

			request.IsDone = true;
		}

		/// <summary>
		/// GetHaircutsIdAsync implementation
		/// </summary>
		private IEnumerator GetHaircutsIdFunc(string avatarCode, AsyncRequest<string[]> request)
		{
			var traits = PipelineTraitsFactory.Instance.GetTraitsFromAvatarCode(avatarCode);
			bool takeFromCache = UseCache && haircutsDataCache.ContainsKey(avatarCode);
			if (takeFromCache)
			{
				List<string> shortHaircutsId = haircutsDataCache[avatarCode].Select(h => h.identity).ToList();
				var convertIdRequest = ConvertToFullHaircutsId(shortHaircutsId, traits.Type);
				yield return convertIdRequest.Await();
				if (convertIdRequest.IsError)
					yield break;

				request.Result = convertIdRequest.Result.ToArray();
			}
			else
			{
				var avatarRequest = GetAvatarAsync(avatarCode);
				yield return avatarRequest.Await();
				if (avatarRequest.IsError)
				{
					request.SetError(avatarRequest.ErrorMessage);
					yield break;
				}

				if (traits.HaircutsSupported)
				{
					var haircutInfoRequest = connection.GetHaircutsAsync(avatarRequest.Result);
					yield return request.AwaitSubrequest(haircutInfoRequest, 0.9f);
					if (request.IsError)
						yield break;

					List<string> shortHaircutsId = haircutInfoRequest.Result.Select(h => h.identity).ToList();
					var convertIdRequest = ConvertToFullHaircutsId(shortHaircutsId, traits.Type);
					yield return convertIdRequest.Await();
					if (convertIdRequest.IsError)
						yield break;

					request.Result = convertIdRequest.Result.ToArray();

					if (UseCache && !haircutsDataCache.ContainsKey(avatarCode))
						haircutsDataCache.Add(avatarCode, haircutInfoRequest.Result);
				}
				else
				{
					Debug.LogFormat("{0} doesn't support haircuts", avatarRequest.Result.pipeline);
				}
			}

			request.IsDone = true;
		}

		/// <summary>
		/// Downloads and saves haircut data that are missed in local storage
		/// </summary>
		private IEnumerator DownloadAndSaveHaircutDataFunc(string avatarCode, string haircutId, AsyncRequest request)
		{
			// In order to display the haircut in a scene correctly we need three things: mesh, texture, and coordinates of
			// vertices adjusted specifically for our avatar (this is called "haircut point cloud"). We need this because
			// algorithms automatically adjust haircuts for each model to provide better fitness.
			// Haircut texture and mesh (number of points and mesh topology) are equal for all avatars, but "point cloud"
			// should be downloaded separately for each model. 
			// If mesh and texture are not cached yet, lets download and save them.

			var haircutMetadata = HaircutsPersistentStorage.Instance.GetHaircutMetadata(haircutId, avatarCode);

			string haircutMeshFilename = haircutMetadata.MeshPly;
			string haircutTextureFilename = haircutMetadata.Texture;
			string haircutPointCloudFilename = haircutMetadata.PathToPointCloud;

			bool needMeshFiles = !File.Exists(haircutMeshFilename) || !File.Exists(haircutTextureFilename);
			bool needPointcloud = !string.IsNullOrEmpty(haircutPointCloudFilename) && !File.Exists(haircutPointCloudFilename);

			if (needMeshFiles || needPointcloud)
			{
				var haircutDataRequest = GetHaircutDataAsync(avatarCode, haircutId);
				yield return request.AwaitSubrequest(haircutDataRequest, 0.05f);
				if (request.IsError)
					yield break;

				List<AsyncRequest> downloadRequests = new List<AsyncRequest>();
				if (needMeshFiles)
					downloadRequests.Add(DownloadAndSaveHaircutMeshAsync(haircutDataRequest.Result, haircutMeshFilename, haircutTextureFilename));
				if (needPointcloud)
					downloadRequests.Add(DownloadAndSaveHaircutPointsAsync(avatarCode, haircutDataRequest.Result));

				yield return request.AwaitSubrequests(0.9f, downloadRequests.ToArray());
				if (request.IsError)
					yield break;
			}

			request.IsDone = true;
		}

		private IEnumerator DownloadAndSaveAllHaircutsFunc(string avatarCode, AsyncRequest request)
		{
			var haircutsListRequest = GetHaircutsIdAsync(avatarCode);
			yield return request.AwaitSubrequest(haircutsListRequest, 0.0f);
			if (request.IsError)
				yield break;

			string[] availableHaircuts = haircutsListRequest.Result;
			if (availableHaircuts != null)
			{
				for (int i=0; i<availableHaircuts.Length; i++)
				{
					var downloadRequest = DownloadAndSaveHaircutDataAsync(avatarCode, availableHaircuts[i]);
					yield return request.AwaitSubrequest(downloadRequest, (i + 1.0f) / availableHaircuts.Length);
					if (request.IsError)
						yield break;
				}
			}

			request.IsDone = true;
		}

		/// <summary>
		/// DownloadAndSaveHaircutMeshAsync implementation
		/// </summary>
		private IEnumerator DownloadAndSaveHaircutMeshFunc(AvatarHaircutData haircutData, AsyncRequest request, string meshDirectoryPath, string textureFilePath)
		{
			string pathToZip = Path.Combine(Path.GetDirectoryName(meshDirectoryPath), "haircut.zip");

			Debug.LogFormat("Downloading haircut mesh, texture and points simultaneously...");
			var haircutMeshRequest = connection.DownloadHaircutMeshZipAsync(haircutData);
			var haircutTextureRequest = connection.DownloadHaircutTextureBytesAsync(haircutData);
			yield return request.AwaitSubrequests(0.8f, haircutMeshRequest, haircutTextureRequest);
			if (request.IsError)
				yield break;

			Debug.LogFormat("Saving haircut mesh and texture to disk...");
			var saveHaircutMeshRequest = CoreTools.SaveHaircutFileAsync(haircutMeshRequest.Result, pathToZip);
			var saveHaircutTextureRequest = CoreTools.SaveHaircutFileAsync(haircutTextureRequest.Result, textureFilePath);
			yield return request.AwaitSubrequests(0.9f, saveHaircutMeshRequest, saveHaircutTextureRequest);
			if (request.IsError)
				yield break;

			Debug.LogFormat("Unzip haircut mesh...");
			var unzipMeshRequest = CoreTools.UnzipFileAsync(saveHaircutMeshRequest.Result);
			yield return request.AwaitSubrequest(unzipMeshRequest, 1.0f);
			if (request.IsError)
				yield break;

			request.IsDone = true;
		}

		/// <summary>
		/// DownloadAndSaveHaircutPreviewAsync implementation
		/// </summary>
		private IEnumerator DownloadAndSaveHaircutPreviewFunc(string haircutId, AvatarHaircutData haircutData, AsyncRequest request)
		{
			Debug.LogFormat("Downloading haircut preview...");
			var haircutPreviewRequest = connection.DownloadHaircutPreviewBytesAsync(haircutData);
			yield return request.AwaitSubrequest(haircutPreviewRequest, 0.8f);
			if (request.IsError)
				yield break;

			Debug.LogFormat("Saving haircut preview to disk...");
			string pathToPreview = HaircutsPersistentStorage.Instance.GetHaircutMetadata(haircutId, null).Preview;
			var saveHaircutPreviewRequest = CoreTools.SaveFileAsync(haircutPreviewRequest.Result, pathToPreview);
			yield return request.AwaitSubrequest(saveHaircutPreviewRequest, 0.9f);
			if (request.IsError)
				yield break;

			request.IsDone = true;
		}

		/// <summary>
		/// DownloadAndSaveHaircutPointsAsync implementation
		/// </summary>
		private IEnumerator DownloadAndSaveHaircutPointsFunc(string avatarCode, AvatarHaircutData haircutData, AsyncRequest request)
		{
			var haircutPointsRequest = connection.DownloadHaircutPointCloudZipAsync(haircutData);
			yield return request.AwaitSubrequest(haircutPointsRequest, 0.9f);
			if (request.IsError)
				yield break;

			var saveHaircutPointsRequest = CoreTools.SaveAvatarHaircutPointCloudZipFileAsync(haircutPointsRequest.Result, avatarCode, haircutData.identity);
			yield return request.AwaitSubrequest(saveHaircutPointsRequest, 0.95f);
			if (request.IsError)
				yield break;

			var unzipPointsRequest = CoreTools.UnzipFileAsync(saveHaircutPointsRequest.Result);
			yield return request.AwaitSubrequest(unzipPointsRequest, 1.0f);
			if (request.IsError)
				yield break;

			request.IsDone = true;
		}

		/// <summary>
		/// GetHaircutMeshAsync implementation
		/// </summary>
		private IEnumerator GetHaircutMeshFunc(string avatarCode, string haircutId, AsyncRequest<TexturedMesh> request)
		{
			DateTime startTime = DateTime.Now;

			var downloadRequest = DownloadAndSaveHaircutDataAsync(avatarCode, haircutId);
			yield return request.AwaitSubrequest(downloadRequest, 0.8f);
			if (request.IsError)
				yield break;

			var loadHaircutRequest = CoreTools.LoadHaircutFromDiskAsync(avatarCode, haircutId);
			yield return request.AwaitSubrequest(loadHaircutRequest, 1.0f);
			if (request.IsError)
				yield break;

			request.IsDone = true;
			request.Result = loadHaircutRequest.Result;
		}

		/// <summary>
		/// GetHaircutPreviewAsync implementation
		/// </summary>
		private IEnumerator GetHaircutPreviewFunc(string avatarCode, string haircutId, AsyncRequest<byte[]> request)
		{
			string haircutPreviewFilename = HaircutsPersistentStorage.Instance.GetHaircutMetadata(haircutId, avatarCode).Preview;

			if (!File.Exists(haircutPreviewFilename))
			{
				var haircutDataRequest = GetHaircutDataAsync(avatarCode, haircutId);
				yield return request.AwaitSubrequest(haircutDataRequest, 0.05f);
				if (request.IsError)
					yield break;

				var downloadRequest = DownloadAndSaveHaircutPreviewAsync(haircutId, haircutDataRequest.Result);

				yield return request.AwaitSubrequest(downloadRequest, 0.9f);
				if (request.IsError)
					yield break;
			}

			byte[] previewBytes = File.ReadAllBytes(haircutPreviewFilename);

			request.IsDone = true;
			request.Result = previewBytes;
		}

		/// <summary>
		/// GetHeadMeshAsync implementation
		/// </summary>
		private IEnumerator GetHeadMeshFunc(string avatarCode, bool withBlendshapes, int detailsLevel, MeshFormat meshFormat, 
			string additionalTextureName, AsyncRequest<TexturedMesh> request)
		{
			string meshFilename = AvatarSdkMgr.Storage().GetAvatarFilename(avatarCode, AvatarFile.MESH_PLY, detailsLevel);
			bool meshFileExists = File.Exists(meshFilename);

			string textureFilename = AvatarSdkMgr.Storage().GetAvatarTextureFilename(avatarCode, additionalTextureName);
			bool textureFileExists = File.Exists(textureFilename);

			List<string> blendshapesDirs = AvatarSdkMgr.Storage().GetAvatarBlendshapesDirs(avatarCode, detailsLevel);
			bool blendshapesExist = true;

			string modelInfoFilename = AvatarSdkMgr.Storage().GetAvatarFilename(avatarCode, AvatarFile.MODEL_JSON);
			bool modelInfoFileExists = File.Exists(modelInfoFilename);
			foreach (string dir in blendshapesDirs)
			{
				if (!Directory.Exists(dir) || Directory.GetFiles(dir).Length == 0)
					blendshapesExist = false;
			}

			if (!meshFileExists || !textureFileExists || !blendshapesExist || !modelInfoFileExists)
			{
				var avatarRequest = connection.GetAvatarAsync(avatarCode);
				yield return avatarRequest;
				if (avatarRequest.IsError)
					yield break;
				AvatarData avatarData = avatarRequest.Result;

				if (!meshFileExists)
				{
					var meshRequest = DownloadAndSaveMeshAsync(avatarData, detailsLevel, meshFormat);
					yield return request.AwaitSubrequest(meshRequest, 0.3f);
					if (request.IsError)
						yield break;
				}

				if (!textureFileExists)
				{
					var textureRequest = DownloadAndSaveTextureAsync(avatarData, additionalTextureName);
					yield return request.AwaitSubrequest(textureRequest, 0.6f);
					if (request.IsError)
						yield break;
				}

				if (!blendshapesExist)
				{
					var blendshapesRequest = DownloadAndSaveBlendshapesAsync(avatarData, detailsLevel);
					yield return request.AwaitSubrequest(blendshapesRequest, 0.9f);
					if (request.IsError)
						yield break;
				}

				if (!modelInfoFileExists)
				{
					var modelInfoRequest = DownloadModelInfoAsync(avatarCode);
					yield return request.AwaitSubrequest(modelInfoRequest, 0.95f);
					if (request.IsError)
						yield break;
				}
			}

			// At this point all avatar files are already saved to disk. Let's load the files to Unity.
			var loadAvatarHeadRequest = CoreTools.LoadAvatarHeadFromDiskAsync(avatarCode, withBlendshapes, detailsLevel, additionalTextureName);
			yield return request.AwaitSubrequest(loadAvatarHeadRequest, 1.0f);
			if (request.IsError)
				yield break;

			request.Result = loadAvatarHeadRequest.Result;
			request.IsDone = true;
		}

		/// <summary>
		/// GetTextureAsync implementation
		/// </summary>
		private IEnumerator GetTextureFunc(string avatarCode, string textureName, AsyncRequest<Texture2D> request)
		{
			string textureFilename = AvatarSdkMgr.Storage().GetAvatarTextureFilename(avatarCode, textureName);
			if (!File.Exists(textureFilename))
			{
				var avatarRequest = connection.GetAvatarAsync(avatarCode);
				yield return avatarRequest;
				if (avatarRequest.IsError)
					yield break;

				var textureRequest = DownloadAndSaveTextureAsync(avatarRequest.Result, textureName);
				yield return request.AwaitSubrequest(textureRequest, 0.9f);
				if (request.IsError)
					yield break;
			}

			var loadTextureRequest = CoreTools.LoadAvatarTextureFromDiskAsync(avatarCode, textureName);
			yield return request.AwaitSubrequest(loadTextureRequest, 1.0f);
			if (request.IsError)
				yield break;

			request.Result = loadTextureRequest.Result;
			request.IsDone = true;
		}

		/// <summary>
		/// DownloadAndSaveMeshAsync implementation
		/// </summary>
		private IEnumerator DownloadAndSaveMeshFunc(AvatarData avatarData, int detailsLevel, MeshFormat format, AsyncRequest request)
		{
			var meshZip = connection.DownloadMeshZipAsync(avatarData, detailsLevel, format);
			yield return request.AwaitSubrequest(meshZip, 0.9f);
			if (request.IsError)
				yield break;

			var saveMeshZip = CoreTools.SaveAvatarFileAsync(meshZip.Result, avatarData.code, AvatarFile.MESH_ZIP, detailsLevel);
			yield return request.AwaitSubrequest(saveMeshZip, 0.95f);
			if (request.IsError)
				yield break;

			var unzipMesh = CoreTools.UnzipFileAsync(saveMeshZip.Result);
			yield return request.AwaitSubrequest(unzipMesh, 0.99f);
			if (request.IsError)
				yield break;

			try
			{
				File.Delete(saveMeshZip.Result);
			}
			catch (Exception ex)
			{
				// error here is not critical, we can just ignore it
				Debug.LogException(ex);
			}

			request.IsDone = true;
		}

		/// <summary>
		/// DownloadAndSaveTextureAsync implementation
		/// </summary>
		protected IEnumerator DownloadAndSaveTextureFunc(AvatarData avatarData, string textureName, string expectedTextureFilename, AsyncRequest request)
		{
			string textureFilename = null;
			byte[] textureBytes = null;

			if (string.IsNullOrEmpty(textureName))
			{
				var textureRequest = connection.DownloadTextureBytesAsync(avatarData);
				yield return request.AwaitSubrequest(textureRequest, 0.9f);
				if (request.IsError)
					yield break;

				textureBytes = textureRequest.Result;
				textureFilename = AvatarSdkMgr.Storage().GetAvatarTextureFilename(avatarData.code);
			}
			else
			{
				var textureRequest = connection.DownloadAdditionalTextureBytesAsync(avatarData, textureName);
				yield return request.AwaitSubrequest(textureRequest, 0.9f);
				if (request.IsError)
					yield break;

				textureBytes = textureRequest.Result.bytes;
				if (!string.IsNullOrEmpty(expectedTextureFilename))
					textureFilename = Path.Combine(AvatarSdkMgr.Storage().GetAvatarDirectory(avatarData.code), expectedTextureFilename);
				else
					textureFilename = Path.Combine(AvatarSdkMgr.Storage().GetAvatarDirectory(avatarData.code), textureRequest.Result.fileName);
			}

			var saveTextureRequest = CoreTools.SaveFileAsync(textureBytes, textureFilename);
			yield return request.AwaitSubrequest(saveTextureRequest, 1.0f);
			if (request.IsError)
				yield break;

			request.IsDone = true;
		}

		/// <summary>
		/// DownloadAndSaveBlendshapesAsync implementation
		/// </summary>
		private IEnumerator DownloadAndSaveBlendshapesFunc(AvatarData avatarData, int detailsLevel, AsyncRequest request)
		{
			var download = new List<AsyncRequest>();

			var blendshapesZip = connection.DownloadBlendshapesZipAsync(avatarData, levelOfDetails: detailsLevel);
			download.Add(blendshapesZip);

#if BLENDSHAPES_IN_PLY_OR_FBX
			// just a sample of how to get blendshapes in a different format
			var blendshapesZipFbx = connection.DownloadBlendshapesZipAsync(avatarData, BlendshapesFormat.FBX, detailsLevel);
			download.Add(blendshapesZipFbx);

			var blendshapesZipPly = connection.DownloadBlendshapesZipAsync(avatar, BlendshapesFormat.PLY, detailsLevel);
			download.Add(blendshapesZipPly);
#endif

			// continue execution when all requests finish
			yield return request.AwaitSubrequests(0.9f, download.ToArray());
			if (request.IsError)
				yield break;

			if (blendshapesZip.Result.IsNullOrEmpty())
			{
				request.IsDone = true;
				yield break;
			}

			var save = new List<AsyncRequest>();
			var saveBlendshapesZip = CoreTools.SaveAvatarFileAsync(blendshapesZip.Result, avatarData.code, AvatarFile.BLENDSHAPES_ZIP, detailsLevel);
			save.Add(saveBlendshapesZip);

#if BLENDSHAPES_IN_PLY_OR_FBX
			// just a sample of how to get blendshapes in a different format
			var saveBlendshapesZipFbx = CoreTools.SaveAvatarFileAsync (blendshapesZipFbx.Result, avatarData.code, AvatarFile.BLENDSHAPES_FBX_ZIP, detialsLevel);
			save.Add (saveBlendshapesZipFbx);

			var saveBlendshapesZipPly = CoreTools.SaveAvatarFileAsync (blendshapesZipPly.Result, avatarData.code, AvatarFile.BLENDSHAPES_PLY_ZIP, detailsLevel);
			save.Add (saveBlendshapesZipPly);
#endif

			yield return request.AwaitSubrequests(0.99f, save.ToArray());
			if (request.IsError)
				yield break;

			var unzip = new List<AsyncRequest>();
			var unzipBlendshapes = UnzipBlendshapesAsync(saveBlendshapesZip.Result, avatarData.code, detailsLevel);
			unzip.Add(unzipBlendshapes);

#if BLENDSHAPES_IN_PLY_OR_FBX
			// just a sample of how to get blendshapes in a different format
			var unzipBlendshapesFbx = UnzipBlendshapesAsync(saveBlendshapesZipFbx.Result, avatarData.code, detailsLevel);
			unzip.Add(unzipBlendshapes);

			var unzipBlendshapesPly = UnzipBlendshapesAsync(saveBlendshapesZipPly.Result, avatarData.code, detailsLevel);
			unzip.Add(unzipBlendshapes);
#endif

			yield return request.AwaitSubrequests(0.99f, unzip.ToArray());
			if (request.IsError)
				yield break;

			try
			{
				File.Delete(saveBlendshapesZip.Result);
			}
			catch (Exception ex)
			{
				// error here is not critical, we can just ignore it
				Debug.LogException(ex);
			}

			request.IsDone = true;
		}

		private IEnumerator DownloadAndSaveHaircutsPointCloudsFunc(AvatarData avatarData, AsyncRequest request)
		{
			if (!PipelineTraitsFactory.Instance.IsHaircutSupported(avatarData.pipeline))
			{
				Debug.LogWarningFormat("{0} doesn't support haircuts", avatarData.pipeline);
				request.IsDone = true;
			}

			var allHaircutPointCloudsZip = connection.DownloadAllHaircutPointCloudsZipAsync(avatarData);
			yield return request.AwaitSubrequest(allHaircutPointCloudsZip, 0.9f);
			if (request.IsError)
				yield break;

			var saveHaircutPointsZip = CoreTools.SaveAvatarFileAsync(allHaircutPointCloudsZip.Result, avatarData.code, AvatarFile.ALL_HAIRCUT_POINTS_ZIP);
			yield return request.AwaitSubrequest(saveHaircutPointsZip, 0.95f);
			if (request.IsError)
				yield break;

			var unzipHaircutPoints = CoreTools.UnzipFileAsync(saveHaircutPointsZip.Result);
			yield return request.AwaitSubrequest(unzipHaircutPoints, 0.99f);
			if (request.IsError)
				yield break;

			try
			{
				File.Delete(saveHaircutPointsZip.Result);

			}
			catch (Exception ex)
			{
				// error here is not critical, we can just ignore it
				Debug.LogException(ex);
			}

			request.IsDone = true;
		}

		/// <summary>
		/// GetAllAvatarsAsync implementation
		/// </summary>
		private IEnumerator GetAllAvatarsFunc(int maxItems, AsyncRequest<string[]> request)
		{
			var avatarsRequest = connection.GetAvatarsAsync(maxItems);
			yield return avatarsRequest;
			if (avatarsRequest.IsError)
			{
				request.SetError(avatarsRequest.ErrorMessage);
				yield break;
			}

			var avatarsData = avatarsRequest.Result.OrderBy(av => DateTime.Parse(av.created_on)).Reverse().ToArray();
			request.Result = avatarsData.Select(a => a.code).ToArray();
			request.IsDone = true;
		}

		/// <summary>
		/// DeleteAvatarAsync implementation
		/// </summary>
		private IEnumerator DeleteAvatarFunc(string avatarCode, AsyncRequest request)
		{
			var avatarRequest = GetAvatarAsync(avatarCode);
			yield return avatarRequest;
			if (avatarRequest.IsError)
			{
				request.SetError(avatarRequest.ErrorMessage);
				yield break;
			}

			var deleteRequest = connection.DeleteAvatarAsync(avatarRequest.Result);
			yield return request.AwaitSubrequest(deleteRequest, 0.5f);
			if (request.IsError)
				yield break;

			CoreTools.DeleteAvatarFiles(avatarCode);

			request.IsDone = true;
		}

		/// <summary>
		/// IsPipelineSupportedAsync implementation
		/// </summary>
		private IEnumerator IsPipelineSupportedFunc(PipelineType pipelineType, AsyncRequest<bool> request)
		{
			var availableParametersRequest = connection.CheckIfPipelineSupportedAsync(pipelineType);
			yield return availableParametersRequest;
			if (availableParametersRequest.IsError)
			{
				Debug.LogErrorFormat("{0} Pipeline isn't supported due to network error.", pipelineType);
				request.Result = false;
			}
			else
			{
				if (!availableParametersRequest.Result)
					Debug.LogFormat("Looks like you don't not have an access to the {0} pipeline", pipelineType);
			}

			request.Result = availableParametersRequest.Result;
			request.IsDone = true;
		}

		/// <summary>
		/// GetParametersAsync implementation
		/// </summary>
		private IEnumerator GetParametersFunc(ComputationParametersSubset parametersSubset, PipelineType pipelineType, AsyncRequest<ComputationParameters> request)
		{
			if (UseCache && avatarParametersCache[pipelineType].ContainsKey(parametersSubset))
			{
				if (!avatarParametersCache[pipelineType][parametersSubset].IsDone)
				{
					yield return avatarParametersCache[pipelineType][parametersSubset];
				}
				request.Result = CloudComputationParametersController.GetParametersFromJson(avatarParametersCache[pipelineType][parametersSubset].Result);
				request.IsDone = true;

			}
			else
			{
				var parametersWebRequest = connection.GetParametersAsync(pipelineType, parametersSubset);
				avatarParametersCache[pipelineType].Add(parametersSubset, parametersWebRequest);
				yield return parametersWebRequest;
				if (parametersWebRequest.IsError)
				{
					Debug.LogError(parametersWebRequest.ErrorMessage);
					request.SetError(parametersWebRequest.ErrorMessage);
					yield break;
				}
				ComputationParameters avatarParameters = CloudComputationParametersController.GetParametersFromJson(parametersWebRequest.Result);
				request.IsDone = true;
				request.Result = avatarParameters;
			}
		}

		/// <summary>
		/// GetAvatarAsync implementation
		/// </summary>
		private IEnumerator GetAvatarFunc(string avatarCode, AsyncRequest<AvatarData> request)
		{
			if (UseCache && avatarsDataCache.ContainsKey(avatarCode))
			{
				request.Result = avatarsDataCache[avatarCode];
				request.IsDone = true;
				yield break;
			}

			var avatarRequest = connection.GetAvatarAsync(avatarCode);
			yield return avatarRequest;
			if (avatarRequest.IsError)
			{
				request.SetError(avatarRequest.ErrorMessage);
				yield break;
			}

			if (UseCache && string.Compare(avatarRequest.Result.status.ToLower(), "Completed") == 0)
			{
				avatarsDataCache[avatarCode] = avatarRequest.Result;
			}

			request.Result = avatarRequest.Result;
			request.IsDone = true;
		}

		/// <summary>
		/// Add prefix (for example "base\") to the haircut ID to make full haircut id
		/// </summary>
		private AsyncRequest<List<string>> ConvertToFullHaircutsId(List<string> shortHaircutsId, PipelineType pipeline)
		{
			var request = new AsyncRequest<List<string>>();
			AvatarSdkMgr.SpawnCoroutine(ConvertToFullHaircutsIdFunc(shortHaircutsId, pipeline, request));
			return request;
		}

		/// <summary>
		/// ConvertToFullHaircutsIdFunc implementation
		/// </summary>
		private IEnumerator ConvertToFullHaircutsIdFunc(List<string> shortHaircutsId, PipelineType pipeline, AsyncRequest<List<string>> request)
		{
			var parametersRequest = GetParametersAsync(ComputationParametersSubset.ALL, pipeline);
			yield return request.AwaitSubrequest(parametersRequest, 0.9f);
			if (request.IsError)
				yield break;

			List<string> allHaircuts = parametersRequest.Result.haircuts.FullNames;
			List<string> fullHaircutsId = new List<string>();
			foreach (string shortId in shortHaircutsId)
			{
				string fullId = allHaircuts.FirstOrDefault(id => id.EndsWith(shortId));
				if (!string.IsNullOrEmpty(fullId))
					fullHaircutsId.Add(fullId);
				else
				{
					Debug.LogErrorFormat("Unable to find full name for haircut: {0}", shortId);
					fullHaircutsId.Add(shortId);
				}
			}
			request.Result = fullHaircutsId;
			request.IsDone = true;
		}
		#endregion
	}
}
