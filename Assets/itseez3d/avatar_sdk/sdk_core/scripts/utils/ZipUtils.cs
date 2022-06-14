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
using System.IO;
using System.Collections.Generic;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Core;
using UnityEngine;

namespace ItSeez3D.AvatarSdk.Core
{
	/// <summary>
	/// Tiny utility class that wraps SharpZipLib functionality.
	/// </summary>
	public static class ZipUtils
	{
		static ZipUtils()
		{
			ICSharpCode.SharpZipLib.Zip.ZipConstants.DefaultCodePage = 0;
		}

		public static void Unzip(string zipFilePath, string location, List<string> extractedFiles = null, bool placeAllFilesInRoot = false)
		{
			using (var s = new ZipInputStream(File.OpenRead(zipFilePath)))
				Unzip(s, location, extractedFiles, placeAllFilesInRoot);
		}

		public static void Unzip(byte[] bytes, string location, List<string> extarctedFiles = null, bool placeAllFilesInRoot = false)
		{
			using (var s = new ZipInputStream(new MemoryStream(bytes)))
				Unzip(s, location, extarctedFiles, placeAllFilesInRoot);
		}

		public static void Unzip(ZipInputStream s, string location, List<string> extarctedFiles = null, bool placeAllFilesInRoot = false)
		{
			ZipEntry theEntry;
			while ((theEntry = s.GetNextEntry()) != null)
			{
				string directoryName = Path.GetDirectoryName(theEntry.Name);
				if (directoryName.Length > 0 && !placeAllFilesInRoot)
					Directory.CreateDirectory(Path.Combine(location, directoryName));

				string fileName = Path.GetFileName(theEntry.Name);
				if (string.IsNullOrEmpty(fileName))
					continue;

				if (extarctedFiles != null)
					extarctedFiles.Add(fileName);

				byte[] data = new byte[s.Length];
				int size = s.Read(data, 0, data.Length);
				if (size != data.Length)
				{
					Debug.LogErrorFormat("Extracted unexpected data size. Extracted: {0}, expected: {1}", size, data.Length);
					return;
				}
				File.WriteAllBytes(Path.Combine(location, placeAllFilesInRoot ? fileName : theEntry.Name), data);
			}
		}

		public static void CreateZipArchive(string archivePath, string folderToZip)
		{
			using (FileStream outStream = File.Create(archivePath))
			{
				using (var zipStream = new ZipOutputStream(outStream))
				{
					zipStream.SetLevel(9);
					CompressFolder(folderToZip, "", zipStream);
				}
			}
		}

		public static byte[] CreateZipArchive(string folderToZip)
		{
			using (MemoryStream stream = new MemoryStream())
			{
				using (var zipStream = new ZipOutputStream(stream))
				{
					zipStream.SetLevel(9);
					CompressFolder(folderToZip, "", zipStream);
				}
				return stream.ToArray();
			}
		}

		private static void CompressFolder(string folderPath, string folderPathInArchive, ZipOutputStream zipStream)
		{
			var files = Directory.GetFiles(folderPath);
			foreach (var filename in files)
			{
				var fileInfo = new FileInfo(filename);
				string fileNameInArchive = Path.Combine(folderPathInArchive, fileInfo.Name);

				var newEntry = new ZipEntry(fileNameInArchive);
				newEntry.DateTime = fileInfo.LastWriteTime;
				newEntry.Size = fileInfo.Length;

				zipStream.PutNextEntry(newEntry);

				var buffer = new byte[4096];
				using (FileStream fsInput = File.OpenRead(filename))
				{
					StreamUtils.Copy(fsInput, zipStream, buffer);
				}
				zipStream.CloseEntry();
			}

			var folders = Directory.GetDirectories(folderPath);
			foreach (var folder in folders)
			{
				string folderName = Path.GetFileName(folder);
				CompressFolder(folder, Path.Combine(folderPathInArchive, folderName), zipStream);
			}
		}
	}
}