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
using UnityEngine;

namespace ItSeez3D.AvatarSdk.Cloud
{
	public class ConnectionForExportApi : Connection
	{
		public AsyncWebRequest<ExportData> GetAvatarExportAsync(string avatarCode, string exportCode)
		{
			var r = AvatarJsonRequest<ExportData>(GetUrl("avatars", avatarCode, "exports", exportCode));
			r.State = AvatarSdkMgr.Str(Strings.GettingAvatarExportInfo);
			return r;
		}

		public AsyncWebRequest<ExportData[]> GetAvatarExportsAsync(AvatarData avatar)
		{
			var r = AvatarJsonArrayRequest<ExportData>(avatar.exports);
			r.State = AvatarSdkMgr.Str(Strings.RequestingAvatarExports);
			return r;
		}

		public AsyncRequest<ExportData> AwaitExportCalculationsAsync(ExportData exportData)
		{
			var request = new AsyncRequest<ExportData>(AvatarSdkMgr.Str(Strings.ComputingExport));
			AvatarSdkMgr.SpawnCoroutine(AwaitExportCalculationsFunc(exportData, request));
			return request;
		}

		public AsyncWebRequest<string> GetExportParametersAsync(PipelineType pipelineType)
		{
			var tratis = pipelineType.Traits();
			var url = GetUrl("export_parameters", "available", tratis.PipelineTypeName);
			url = UrlWithParams(url, "pipeline_subtype", tratis.PipelineSubtypeName);
			var request = new AsyncWebRequest<string>(Strings.GettingExportParametersList);
			AvatarSdkMgr.SpawnCoroutine(AwaitStringDataAsync(() => HttpGet(url), request));
			return request;
		}

		protected override IEnumerator AwaitAvatarCalculationsFunc(AvatarData avatar, AsyncRequest<AvatarData> request)
		{
			bool exportsExist = !string.IsNullOrEmpty(avatar.exports);
			if (exportsExist)
			{
				CoroutineOutVar<AvatarData> avatarDataOut = new CoroutineOutVar<AvatarData>(avatar);
				yield return AwaitAvatarCalculationsLoop(avatarDataOut, 0.9f, request);

				avatar = avatarDataOut.value;
				if (Strings.GoodFinalStates.Contains(avatar.status))
				{
					var exportsRequest = GetAvatarExportsAsync(avatar);
					yield return request.AwaitSubrequest(exportsRequest, 0.9f);
					if (request.IsError)
						yield break;

					if (exportsRequest.Result.Length > 0)
					{
						var awaitExportCalculationsRequest = AwaitExportCalculationsAsync(exportsRequest.Result[0]);
						yield return request.AwaitSubrequest(awaitExportCalculationsRequest, 1.0f);
						if (request.IsError)
							yield break;

						avatar.exportData = awaitExportCalculationsRequest.Result;
					}
					
					request.Result = avatar;
					request.IsDone = true;
				}
				else
				{
					request.SetError(string.Format("Avatar calculations failed, status: {0}", avatar.status));
				}
			}
			else
			{
				yield return base.AwaitAvatarCalculationsFunc(avatar, request);
			}
		}

		protected IEnumerator AwaitExportCalculationsFunc(ExportData exportData, AsyncRequest<ExportData> request)
		{
			while (!Strings.FinalStates.Contains(exportData.status))
			{
				yield return new WaitForSecondsRealtime(0.2f);
				var exportStatusRequest = GetAvatarExportAsync(exportData.avatar_code, exportData.code);
				yield return exportStatusRequest;

				if (exportStatusRequest.Status.Value == (long)StatusCode.Code.TOO_MANY_REQUESTS_THROTTLING)
				{
					Debug.LogWarning("Too many requests for avatar export!");
					yield return new WaitForSecondsRealtime(exportStatusRequest.RetryPeriod.Value);
				}

				
				exportData = exportStatusRequest.Result;
			}

			if (Strings.GoodFinalStates.Contains(exportData.status))
			{
				request.Result = exportData;
				request.IsDone = true;
			}
			else
			{
				request.SetError(string.Format("Avatar export calculations failed, status: {0}", exportData.status));
			}
		}
	}
}
