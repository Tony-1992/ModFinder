using ModFinder.Interfaces;
using System.Diagnostics;

namespace ModFinder
{
    internal class ScriptManager
    {
        private ILogger Logger { get; }
        private FileManager FileManager { get; }
        private string ModFolderPath { get; }
        private string GameExePath { get; }
        private string GameProcessName { get; }
        private string TempModPath { get; }
        private int WaitTime { get; }

        public ScriptManager(ILogger logger, string gameExePath, FileManager fileManager, int waitTime)
        {
            Logger = logger;
            FileManager = fileManager;
            ModFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Electronic Arts\The Sims 4\Mods";
            GameExePath = gameExePath ?? throw new ArgumentNullException(nameof(gameExePath));
            GameProcessName = Path.GetFileNameWithoutExtension(gameExePath);
            TempModPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\TempMods";
            WaitTime = waitTime;
        }

        public string GetModFolderPath()
        {
            return ModFolderPath;
        }

        public string GetTempModPath()
        {
            return TempModPath;
        }

        public void OpenGame()
        {
            Process.Start(GameExePath);
        }

        public void CloseGame()
        {
            Process[] procs = Process.GetProcessesByName(GameProcessName);
            foreach (var proc in procs)
                proc.Kill(true);
        }

        private bool IsGameProcessRunning()
        {
            foreach (Process clsProcess in Process.GetProcesses())
            {
                if (clsProcess.ProcessName.Contains(GameProcessName)) return true;
            }
            return false;
        }

        public string FindFailedModFolder()
        {
            string failureFolderPath = string.Empty;
            foreach (DirectoryInfo folder in FileManager.GetFolders(ModFolderPath))
            {
                Logger.LogMessage($"{FileManager.GetTotalFolderSize(ModFolderPath)}GB of data remaining...");

                DateTime gameExpirationTime = DateTime.Now.AddMinutes(WaitTime);

                // Move batch of mods
                failureFolderPath = FileManager.MoveFolderTo(folder, TempModPath);

                // Load game & wait
                OpenGame();
                Delay();

                // Game process should be running once OpenGame is called
                while (IsGameProcessRunning())
                {
                    // If current time is greater than expiry then more than likely it's running ok so return previous folder path
                    if (DateTime.Now > gameExpirationTime)
                    {
                        return failureFolderPath;
                    }
                }
            }
            return failureFolderPath;
        }

        public string FindFailedFile(string folderPathToLoop)
        {
            Logger.LogInformation("\nFailure folder found, beginner deeper search...");
            string failedModName = string.Empty;

            Delay();

            Logger.LogInformation("Moving remaining mods to TempMod location...");
            foreach (DirectoryInfo folder in FileManager.GetFolders(ModFolderPath))
            {
                FileManager.MoveFolderTo(folder, TempModPath);
            }


            Logger.LogInformation("\nRebuilding mods folder to begin individual file search...");
            Logger.LogMessage($"Copying failed folder to mods\n");
            
            var updatedPath = FileManager.CopyFailedFolderToModFolder(folderPathToLoop, ModFolderPath);
            string[] files = Directory.GetFiles(updatedPath);

            // Loop files
            foreach (string fileName in files)
            {
                DateTime gameExpirationTime = DateTime.Now.AddMinutes(WaitTime);

                Logger.LogMessage($"File removed => {fileName}");
                FileManager.DeleteFile(fileName);

                // Load game & wait
                OpenGame();
                Delay();

                while (IsGameProcessRunning())
                {
                    // If current time is greater than expiry then more than likely it's running ok so return file name
                    if (DateTime.Now > gameExpirationTime)
                    {
                        return fileName;
                    }
                }
            }

            return failedModName;
        }

        public void Delay(int delay = 10000)
        {
            Thread.Sleep(delay);
        }

    }
}
