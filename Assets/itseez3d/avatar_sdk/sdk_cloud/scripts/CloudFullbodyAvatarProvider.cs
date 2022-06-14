/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
* You may not use this file except in compliance with an authorized license
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
* See the License for the specific language governing permissions and limitations under the License.
* Written by Itseez3D, Inc. <support@avatarsdk.com>, November 2020
*/

using ItSeez3D.AvatarSdk.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace ItSeez3D.AvatarSdk.Cloud
{
	public class CloudFullbodyAvatarProvider : CloudAvatarProvider, IFullbodyAvatarProvider
	{
		protected IFullbodyPersistentStorage fullbodyStorage = null;

		protected Dictionary<PipelineType, AsyncRequest<FullbodyAvatarComputationParameters>> computationParametersCache = new Dictionary<PipelineType, AsyncRequest<FullbodyAvatarComputationParameters>>();

		public CloudFullbodyAvatarProvider()
		{
			connection = new ConnectionForExportApi();
			fullbodyStorage = AvatarSdkMgr.FullbodyStorage();
		}

		#region IFullbodyAvatarProvider
		public AsyncRequest<string> InitializeFullbodyAvatarAsync(byte[] photoBytes, FullbodyAvatarComputationParameters computationParameters, PipelineType pipelineType,
			string name = "name", string description = "")
		{
			var request = new AsyncRequest<string>(AvatarSdkMgr.Str(Strings.InitializingAvatar));
			AvatarSdkMgr.SpawnCoroutine(InitializeFullbodyAvatarFunc(photoBytes, null, computationParameters, pipelineType, name, description, request));
			return request;
		}

		public AsyncRequest<string> InitializeFullbodyAvatarAsync(string selfieCode, byte[] selfiePhotoBytes, FullbodyAvatarComputationParameters computationParameters, PipelineType pipelineType,
			string name = "name", string description = "")
		{
			var request = new AsyncRequest<string>(AvatarSdkMgr.Str(Strings.InitializingAvatar));
			AvatarSdkMgr.SpawnCoroutine(InitializeFullbodyAvatarFunc(selfiePhotoBytes, selfieCode, computationParameters, pipelineType, name, description, request));
			return request;
		}

		public List<string> GetHaircutsInDiscreteFiles(string avatarCode)
		{
			return FindExportIdentitiesForCategory(avatarCode, "haircuts");
		}

		public List<string> GetOutfitsInDiscreteFiles(string avatarCode)
		{
			return FindExportIdentitiesForCategory(avatarCode, "outfits");
		}

		public AsyncRequest RetrieveAllAvatarDataFromCloudAsync(string avatarCode)
		{
			return DownloadAndSaveAllFullbodyAvatarDataAsync(avatarCode);
		}

		public AsyncRequest RetrieveBodyModelFromCloudAsync(string avatarCode)
		{
			var request = new AsyncRequest(AvatarSdkMgr.Str(Strings.GettingAvatar));
			AvatarSdkMgr.SpawnCoroutine(RetrieveBodyModelFromCloudFunc(avatarCode, request));
			return request;
		}

		public AsyncRequest RetrieveHaircutModelFromCloudAsync(string avatarCode, string haircutName)
		{
			var request = new AsyncRequest(AvatarSdkMgr.Str(Strings.GettingHaircut));
			AvatarSdkMgr.SpawnCoroutine(RetrieveHaircutModelFromCloudFunc(avatarCode, haircutName, request));
			return request;
		}

		public AsyncRequest RetrieveOutfitModelFromCloudAsync(string avatarCode, string outfitName)
		{
			var request = new AsyncRequest(AvatarSdkMgr.Str(Strings.GettingOutfit));
			AvatarSdkMgr.SpawnCoroutine(RetrieveOutfitModelFromCloudFunc(avatarCode, outfitName, request));
			return request;
		}

		public AsyncRequest<FullbodyAvatarComputationParameters> GetAvailableComputationParametersAsync(PipelineType pipelineType)
		{
			if (UseCache && computationParametersCache.ContainsKey(pipelineType))
				return computationParametersCache[pipelineType];
			
			var request = new AsyncRequest<FullbodyAvatarComputationParameters>(AvatarSdkMgr.Str(Strings.GettingAvailableParameters));
			if (UseCache)
				computationParametersCache[pipelineType] = request;
			AvatarSdkMgr.SpawnCoroutine(GetAvailableComputationParametersFunc(pipelineType, request));
			return request;
		}

		#endregion IFullbodyAvatarProvider

		public AsyncRequest DownloadAndSaveFullbodyAvatarAsync(string avatarCode)
		{
			var request = new AsyncRequest(AvatarSdkMgr.Str(Strings.DownloadingAvatar));
			AvatarSdkMgr.SpawnCoroutine(DownloadAndSaveFullbodyAvatarFunc(avatarCode, request));
			return request;
		}

		public AsyncRequest DownloadAndSaveFullbodyHaircutAsync(string avatarCode, string haircutName)
		{
			var request = new AsyncRequest(AvatarSdkMgr.Str(Strings.DownloadingHaircut));
			AvatarSdkMgr.SpawnCoroutine(DownloadExportItemFiles(avatarCode, haircutName, request));
			return request;
		}

		public AsyncRequest DownloadAndSaveFullbodyOutfitAsync(string avatarCode, string outfitName)
		{
			var request = new AsyncRequest(AvatarSdkMgr.Str(Strings.DownloadingOutfit));
			AvatarSdkMgr.SpawnCoroutine(DownloadExportItemFiles(avatarCode, outfitName, request));
			return request;
		}

		public AsyncRequest DownloadAndSaveAllFullbodyAvatarDataAsync(string avatarCode)
		{
			var request = new AsyncRequest(AvatarSdkMgr.Str(Strings.DownloadingAvatar));
			AvatarSdkMgr.SpawnCoroutine(DownloadAndSaveAllFullbodyAvatarDataFunc(avatarCode, request));
			return request;
		}

		protected override IEnumerator StartAndAwaitAvatarCalculationFunc(string avatarCode, AsyncRequest<AvatarData> request)
		{
			yield return base.StartAndAwaitAvatarCalculationFunc(avatarCode, request);
		
			if (!request.IsError)
				SaveExportData(request.Result);
		}

		private IEnumerator RetrieveBodyModelFromCloudFunc(string avatarCode, AsyncRequest request)
		{
			string avatarModelFilename = fullbodyStorage.GetAvatarFile(avatarCode, FullbodyAvatarFileType.Model);

			if (!File.Exists(avatarModelFilename))
			{
				var downloadRequest = DownloadAndSaveFullbodyAvatarAsync(avatarCode);
				yield return request.AwaitSubrequest(downloadRequest, 1.0f);
				if (request.IsError)
					yield break;
			}

			request.IsDone = true;
		}

		private IEnumerator RetrieveHaircutModelFromCloudFunc(string avatarCode, string haircutName, AsyncRequest request)
		{
			string modelFilename = fullbodyStorage.GetHaircutFile(avatarCode, haircutName, FullbodyHaircutFileType.Model);

			if (!File.Exists(modelFilename))
			{
				var downloadRequest = DownloadAndSaveFullbodyHaircutAsync(avatarCode, haircutName);
				yield return request.AwaitSubrequest(downloadRequest, 1.0f);
				if (request.IsError)
					yield break;
			}

			request.IsDone = true;
		}

		private IEnumerator RetrieveOutfitModelFromCloudFunc(string avatarCode, string outfitName, AsyncRequest request)
		{
			string modelFilename = fullbodyStorage.GetOutfitFile(avatarCode, outfitName, OutfitFileType.Model);

			if (!File.Exists(modelFilename))
			{
				var downloadRequest = DownloadAndSaveFullbodyOutfitAsync(avatarCode, outfitName);
				yield return request.AwaitSubrequest(downloadRequest, 1.0f);
				if (request.IsError)
					yield break;
			}

			request.IsDone = true;
		}

		private AsyncRequest<ExportData> GetAvatarFirstExportAsync(string avatarCode)
		{
			var request = new AsyncRequest<ExportData>(AvatarSdkMgr.Str(Strings.GettingAvatarExportInfo));
			AvatarSdkMgr.SpawnCoroutine(GetAvatarFirstExportFunc(avatarCode, request));
			return request;
		}

		private IEnumerator InitializeFullbodyAvatarFunc(byte[] photoBytes, string selfieCode, FullbodyAvatarComputationParameters computationParameters, PipelineType pipeline,
			string name, string description, AsyncRequest<string> request)
		{
			AsyncWebRequest<AvatarData> createAvatarRequest = null;
			if (string.IsNullOrEmpty(selfieCode))
			{
				byte[] preprocessedBytes = photoBytes;
				if (AvatarSdkMgr.Settings.ForceRescaleLargeImages)
				{
					var downscaleImage = ImageUtils.DownscaleImageIfNeedAsync(photoBytes);
					yield return request.AwaitSubrequest(downscaleImage, finalProgress: 0.8f);
					if (request.IsError)
						yield break;
					preprocessedBytes = downscaleImage.Result;
				}

				// uploading photo and registering new avatar on the server
				createAvatarRequest = connection.CreateAvatarAsync(preprocessedBytes, pipeline, computationParameters, name, description);
			}
			else
			{
				createAvatarRequest = connection.CreateAvatarAsync(selfieCode, pipeline, computationParameters, name, description);
			}

			// Wait until async request is completed (without blocking the main thread).
			// Instead of using AwaitSubrequest we could just use `yield return createAvatar;`
			// AwaitSubrequest is a helper function that allows to track progress on composite
			// requests automatically. It also provides info for the caller about current subrequest
			// (and it's progress) and propagetes error from subrequest to the parent request.
			// finalProgress is a value between 0 and 1, a desired progress of parent request when given
			// subrequest is completed.
			yield return request.AwaitSubrequest(createAvatarRequest, finalProgress: 0.99f);

			// must check whether request was successful before proceeding
			if (request.IsError)
				yield break;

			string avatarCode = createAvatarRequest.Result.code;

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

		private IEnumerator DownloadAndSaveFullbodyAvatarFunc(string avatarCode, AsyncRequest request)
		{
			var avatarExportRequest = GetAvatarFirstExportAsync(avatarCode);
			yield return request.AwaitSubrequest(avatarExportRequest, 0.0f);
			if (request.IsError)
				yield break;

			ExportData avatarExport = avatarExportRequest.Result;
			ExportItem exportItem = avatarExport.files.FirstOrDefault(f => f.identity == "avatar");
			if (exportItem == null)
			{
				request.SetError(string.Format("There is no avatar export file for avatar: {0}", avatarCode));
				yield break;
			}

			yield return DownloadExportItemFiles(avatarCode, exportItem, 1.0f, request);
			if (request.IsError)
				yield break;

			request.IsDone = true;
		}

		private IEnumerator DownloadExportItemFiles(string avatarCode, string exportItemIdentity, AsyncRequest request)
		{
			var avatarExportRequest = GetAvatarFirstExportAsync(avatarCode);
			yield return request.AwaitSubrequest(avatarExportRequest, 0.0f);
			if (request.IsError)
				yield break;

			ExportData avatarExport = avatarExportRequest.Result;
			ExportItem haircutExportItem = avatarExport.files.FirstOrDefault(f => f.identity == exportItemIdentity);
			if (haircutExportItem == null)
			{
				request.SetError(string.Format("There is no {0} export item for avatar: {1}", exportItemIdentity, avatarCode));
				yield break;
			}

			yield return DownloadExportItemFiles(avatarCode, haircutExportItem, 1.0f, request);
			if (request.IsError)
				yield break;

			request.IsDone = true;
		}

		private IEnumerator DownloadAndSaveAllFullbodyAvatarDataFunc(string avatarCode, AsyncRequest request)
		{
			var avatarExportRequest = GetAvatarFirstExportAsync(avatarCode);
			yield return request.AwaitSubrequest(avatarExportRequest, 0.0f);
			if (request.IsError)
				yield break;

			ExportData avatarExport = avatarExportRequest.Result;
			if (avatarExport.files != null && avatarExport.files.Length > 0)
			{
				float progressIncrement = 1.0f / avatarExport.files.Length;
				foreach (ExportItem exportItem in avatarExport.files)
					yield return DownloadExportItemFiles(avatarCode, exportItem, progressIncrement, request);
			}

			request.IsDone = true;
		}

		private IEnumerator DownloadExportItemFiles(string avatarCode, ExportItem exportItem, float progressIncrement, AsyncRequest request)
		{
			int countFilesInExportItem = 1;
			if (exportItem.static_files != null)
				countFilesInExportItem += exportItem.static_files.Length;
			float progressSubIncrement = progressIncrement / countFilesInExportItem;

			string zipFilename = Path.Combine(fullbodyStorage.GetAvatarDirectory(avatarCode), exportItem.identity + ".zip");
			yield return DownloadAndExtractArchive(exportItem.file, zipFilename, progressSubIncrement, request);
			if (request.IsError)
				yield break;

			if (exportItem.static_files != null)
			{
				string extractedArchiveDir = Path.Combine(fullbodyStorage.GetAvatarDirectory(avatarCode), exportItem.identity);
				foreach (string commonFileUrl in exportItem.static_files)
				{
					yield return DownloadCommonFileIfRequired(exportItem, commonFileUrl, extractedArchiveDir, progressSubIncrement, request);
					if (request.IsError)
						yield break;
				}
			}
		}

		private IEnumerator DownloadAndExtractArchive(string archiveUrl, string zipFilename, float progressIncrement, AsyncRequest parentRequest)
		{
			float startProgress = parentRequest.Progress;

			var downloadZipRequest = connection.AvatarDataRequestAsync(archiveUrl, Strings.DownloadingZip);
			yield return parentRequest.AwaitSubrequest(downloadZipRequest, startProgress + 0.9f * progressIncrement);
			if (parentRequest.IsError)
				yield break;

			var saveZip = CoreTools.SaveFileAsync(downloadZipRequest.Result, zipFilename);
			yield return parentRequest.AwaitSubrequest(saveZip, startProgress + 0.95f * progressIncrement);
			if (parentRequest.IsError)
				yield break;

			var unzipMesh = CoreTools.UnzipFileAsync(saveZip.Result, null);
			yield return parentRequest.AwaitSubrequest(unzipMesh, startProgress + progressIncrement);
			if (parentRequest.IsError)
				yield break;

			try
			{
				File.Delete(saveZip.Result);
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		private IEnumerator DownloadCommonFileIfRequired(ExportItem exportItem, string commonFileUrl, string copyToDirectory, float progressIncrement, AsyncRequest parentRequest)
		{
			string commonFileRelativePath = GetCommonFileRelativePathFromUrl(exportItem.category, exportItem.identity, commonFileUrl);
			string commonFilePath = fullbodyStorage.GetStaticFilePath(commonFileRelativePath);

			if (!File.Exists(commonFilePath))
			{
				var downloadRequest = connection.AvatarDataRequestAsync(commonFileUrl, string.Format("Downloading {0}", Path.GetFileName(commonFilePath)));
				yield return parentRequest.AwaitSubrequest(downloadRequest, parentRequest.Progress + progressIncrement);
				if (parentRequest.IsError)
					yield break;

				IOUtils.SaveFile(commonFilePath, downloadRequest.Result);
			}
			else
				parentRequest.Progress = progressIncrement;

			IOUtils.CopyFile(commonFilePath, copyToDirectory);
		}

		private IEnumerator GetAvatarFirstExportFunc(string avatarCode, AsyncRequest<ExportData> request)
		{
			var avatarDataRequest = GetAvatarAsync(avatarCode);
			yield return request.AwaitSubrequest(avatarDataRequest, 0.0f);
			if (request.IsError)
				yield break;

			AvatarData avatarData = avatarDataRequest.Result;
			if (string.IsNullOrEmpty(avatarData.exports))
			{
				string errorMsg = string.Format("There are no exports for avatar {0}", avatarCode);
				Debug.LogError(errorMsg);
				request.SetError(errorMsg);
				yield break;
			}

			if (avatarData.exportData == null || avatarData.exportData.IsEmpty())
			{
				var exportsRequest = (connection as ConnectionForExportApi).GetAvatarExportsAsync(avatarData);
				yield return request.AwaitSubrequest(exportsRequest, 0.0f);
				if (request.IsError)
					yield break;

				avatarData.exportData = exportsRequest.Result[0];
				if (UseCache)
					avatarsDataCache[avatarCode] = avatarData;
			}

			request.Result = avatarData.exportData;
			request.IsDone = true;
		}

		private IEnumerator GetAvailableComputationParametersFunc(PipelineType pipelineType, AsyncRequest<FullbodyAvatarComputationParameters> request)
		{
			var parametersRequest = connection.GetParametersAsync(pipelineType, ComputationParametersSubset.ALL);
			yield return request.AwaitSubrequest(parametersRequest, 0.5f);
			if (request.IsError)
			{
				if (UseCache)
					computationParametersCache.Remove(pipelineType);
				yield break;
			}

			var exportParametersRequest = (connection as ConnectionForExportApi).GetExportParametersAsync(pipelineType);
			yield return request.AwaitSubrequest(exportParametersRequest, 0.99f);
			if (request.IsError)
			{
				if (UseCache)
					computationParametersCache.Remove(pipelineType);
				yield break;
			}

			FullbodyAvatarComputationParameters computationParameters = new FullbodyAvatarComputationParameters();
			computationParameters.ParseParametersJson(parametersRequest.Result);
			computationParameters.ParseExportJson(exportParametersRequest.Result);

			request.Result = computationParameters;
			request.IsDone = true;
		}

		private void SaveExportData(AvatarData avatarData)
		{
			List<string> haircutsList = new List<string>();
			if (avatarData.exportData != null)
			{
				string exportJson = JsonUtility.ToJson(avatarData.exportData);
				File.WriteAllText(fullbodyStorage.GetCommonFile(avatarData.code, FullbodyCommonFileType.ExportDataJson), exportJson);
			}
		}

		private string GetCommonFileRelativePathFromUrl(string category, string identity, string staticFileUrl)
		{
			if (string.IsNullOrEmpty(category))
				return Path.Combine(identity, Path.GetFileName(staticFileUrl));
			else
				return Path.Combine(category, identity, Path.GetFileName(staticFileUrl));
		}

		private List<string> FindExportIdentitiesForCategory(string avatarCode, string category)
		{
			List<string> identitiesNames = new List<string>();
			string exportDataFile = fullbodyStorage.GetCommonFile(avatarCode, FullbodyCommonFileType.ExportDataJson);
			if (!File.Exists(exportDataFile))
			{
				Debug.LogErrorFormat("Unable to get identities for category {0}. Export data file not found: {1}", category, exportDataFile);
				return identitiesNames;
			}

			ExportData exportData = JsonUtility.FromJson<ExportData>(File.ReadAllText(exportDataFile));
			if (exportData.files != null)
			{
				foreach (ExportItem exportItem in exportData.files)
				{
					if (exportItem.category == category)
						identitiesNames.Add(exportItem.identity);
				}
			}

			return identitiesNames;
		}
	}
}
