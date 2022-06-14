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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace ItSeez3D.AvatarSdk.Core.Communication
{
	public class ConnectionBase
	{
		/// <summary>
		/// Root URL, can be changed right after the creation of the object, before any requests are made (otherwise auth won't work).
		/// </summary>
		protected string rootServerUrl = NetworkUtils.rootUrl;
		public string RootServerUrl
		{
			get { return rootServerUrl; }
			set { rootServerUrl = value; }
		}

		/// <summary>
		/// Helper function to construct absolute url from relative url tokens.
		/// </summary>
		/// <returns>Absolute url.</returns>
		/// <param name="urlTokens">Relative url tokens.</param>
		public virtual string GetUrl(params string[] urlTokens)
		{
			return string.Format("{0}/{1}/", RootServerUrl, string.Join("/", urlTokens));
		}

		protected UnityWebRequest HttpPost(string url, Dictionary<string, string> form)
		{
			var r = UnityWebRequest.Post(url, form);
#if UNITY_2017 || UNITY_2018
			r.chunkedTransfer = false;  // chunked transfer causes problems with UWSGI
#endif
			return r;
		}

		/// <returns>Url used to obtain access token.</returns>
		public virtual string GetAuthUrl()
		{
			return GetUrl("o", "token");
		}

		/// <summary>
		/// Generate HTTP request object to obtain the access token.
		/// </summary>
		/// <param name="credentials">Auth credentials (id and secret)</param>
		/// <returns>UnityWebRequest object</returns>
		public virtual UnityWebRequest GenerateAuthRequest(AccessCredentials credentials)
		{
			var form = new Dictionary<string, string>() {
				{ "grant_type", "client_credentials" },
				{ "client_id", credentials.clientId },
				{ "client_secret", credentials.clientSecret },
			};
			return HttpPost(GetAuthUrl(), form);
		}
	}
}