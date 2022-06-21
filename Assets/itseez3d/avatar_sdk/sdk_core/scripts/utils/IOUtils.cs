/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
* You may not use this file except in compliance with an authorized license
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
* See the License for the specific language governing permissions and limitations under the License.
* Written by Itseez3D, Inc. <support@avatarsdk.com>, April 2017
*/

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace ItSeez3D.AvatarSdk.Core
{
	public static class IOUtils
	{
		/// <summary>
		/// C# should have this overload out of the box, honestly.
		/// </summary>
		public static string CombinePaths(params string[] tokens)
		{
			return tokens.Aggregate(Path.Combine);
		}

		/// <summary>
		/// Return file name independent of the slash type in path (backslash or forward slash)
		/// </summary>
		public static string GetFileName(string filePath)
		{
			string[] parts = filePath.Split('\\', '/');
			return parts[parts.Length - 1];
		}

		/// <summary>
		/// Delete directory content
		/// </summary>
		public static void CleanDirectory(string dir)
		{
			if (Directory.Exists(dir))
				Directory.Delete(dir, true);
			Directory.CreateDirectory(dir);
		}

		/// <summary>
		/// Copy content of the directory
		/// </summary>
		public static void CopyDirectory(string fromDir, string toDir)
		{
			CleanDirectory(toDir);

			DirectoryInfo dirInfo = new DirectoryInfo(fromDir);
			foreach (FileInfo fileInfo in dirInfo.GetFiles())
				fileInfo.CopyTo(Path.Combine(toDir, fileInfo.Name), true);

			foreach (DirectoryInfo subDirInfo in dirInfo.GetDirectories())
				CopyDirectory(subDirInfo.FullName, Path.Combine(toDir, subDirInfo.FullName));
		}

		/// <summary>
		/// Copy file to a specified directory
		/// </summary>
		public static void CopyFile(string srcfilePath, string dstDirPath)
		{
			byte[] data = File.ReadAllBytes(srcfilePath);
			string dstFilePath = Path.Combine(dstDirPath, Path.GetFileName(srcfilePath));
			File.WriteAllBytes(dstFilePath, data);
		}

		public static void CreateDirectoryIfNotExist(string dir)
		{
			if (!Directory.Exists(dir))
				Directory.CreateDirectory(dir);
		}

		public static void SaveFile(string filePath, byte[] fileContent)
		{
			Directory.CreateDirectory(Path.GetDirectoryName(filePath));
			File.WriteAllBytes(filePath, fileContent);
		}
	}
}
