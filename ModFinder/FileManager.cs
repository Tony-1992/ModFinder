using ModFinder.Interfaces;

namespace ModFinder
{
    internal class FileManager
    {
        private ILogger Logger { get; }
        public FileManager(ILogger logger)
        {
            Logger = logger;
        }



        public decimal GetTotalFolderSize(string folderPath)
        {
            DirectoryInfo modFolder = new DirectoryInfo(folderPath);
            IEnumerable<DirectoryInfo> subFolders = modFolder.EnumerateDirectories();

            long totalSize = 0;
            foreach (var subFolder in subFolders)
            {
                totalSize += subFolder.EnumerateFiles().Sum(f => f.Length);
            }

            return ConvertByteToGigabyte(totalSize);
        }

        public void CleanUp(string moveFromPath, string moveToPath, bool deleteMoveFromPath = false)
        {
            DirectoryInfo tempFolder = new DirectoryInfo(moveFromPath);
            DirectoryInfo modFolder = new DirectoryInfo(moveToPath);

            foreach (var folder in modFolder.EnumerateDirectories())
            {
                var pathToMods = Path.Combine(moveToPath, folder.Name);
                Directory.Delete(pathToMods, true);
            }

            foreach (var folder in tempFolder.EnumerateDirectories())
            {
                var pathToMods = Path.Combine(moveToPath, folder.Name);
                Directory.Move(folder.FullName, pathToMods);
            }

            // optional
            if (deleteMoveFromPath) Directory.Delete(moveFromPath, true);
        }


        public string MoveFolderTo(DirectoryInfo folderToMove, string moveToPath)
        {
            if (Directory.Exists(moveToPath) is false)
            {
                Directory.CreateDirectory(moveToPath);
            }

            Logger.LogMessage($"Moving: {folderToMove.Name} ====> {moveToPath}");

            string destinationFolder = Path.Combine(moveToPath, folderToMove.Name);
            Directory.Move(folderToMove.FullName, destinationFolder);
            return destinationFolder;
        }

        public string MoveFailedFolderToModFolder(string folderToMove, string moveToPath)
        {
            DirectoryInfo folder = new DirectoryInfo(folderToMove);
            if (Directory.Exists(moveToPath) is false)
            {
                Directory.CreateDirectory(moveToPath);
            }

            string destinationFolder = Path.Combine(moveToPath, folder.Name); // issue is here with the name
            Directory.Move(folder.FullName, destinationFolder);
            return destinationFolder;
        }

        public string CopyFailedFolderToModFolder(string folderToCopy, string moveToPath)
        {
            DirectoryInfo folder = new DirectoryInfo(folderToCopy);
            string destinationFolder = Path.Combine(moveToPath, folder.Name);
            if (Directory.Exists(destinationFolder) is false)
            {
                Directory.CreateDirectory(destinationFolder);
            }

           
            foreach (FileInfo fileName in folder.GetFiles())
            {
                fileName.CopyTo(Path.Combine(destinationFolder, fileName.Name));
            }

            return destinationFolder;
        }

        public void DeleteFolder(string folderName)
        {
            Directory.Delete(folderName, true);
        }

        public void DeleteFile(string fileName)
        {
            File.Delete(fileName);
        }


        public List<DirectoryInfo> GetFolders(string folderPath)
        {
            DirectoryInfo folder = new DirectoryInfo(folderPath);
            IEnumerable<DirectoryInfo> subFolders = folder.EnumerateDirectories();
            return subFolders.ToList();
        }

        private decimal ConvertByteToGigabyte(long totalFolderSize)
        {
            return totalFolderSize / 1024 / 1024 / 1024;
        }


    }
}
