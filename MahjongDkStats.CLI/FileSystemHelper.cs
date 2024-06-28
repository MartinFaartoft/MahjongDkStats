namespace MahjongDkStats.CLI
{
	internal class FileSystemHelper
	{
		public const string OutputFolderName = "dist";
		public const string ImagesPath = OutputFolderName + "/img";

		public static void PrepareFoldersAndAssets()
		{
			if (Directory.Exists(OutputFolderName))
			{
				Directory.Delete(OutputFolderName, true);
			}
			Directory.CreateDirectory(OutputFolderName);
			if (Directory.Exists("assets"))
			{
				Copy("assets", OutputFolderName);
			}
			Directory.CreateDirectory(ImagesPath);
		}

		private static void Copy(string sourceDirectory, string targetDirectory)
		{
			DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
			DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);

			CopyAll(diSource, diTarget);
		}

		private static void CopyAll(DirectoryInfo source, DirectoryInfo target)
		{
			Directory.CreateDirectory(target.FullName);

			foreach (FileInfo fi in source.GetFiles())
			{
				fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
			}

			foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
			{
				DirectoryInfo nextTargetSubDir =
					target.CreateSubdirectory(diSourceSubDir.Name);
				CopyAll(diSourceSubDir, nextTargetSubDir);
			}
		}

	}
}
