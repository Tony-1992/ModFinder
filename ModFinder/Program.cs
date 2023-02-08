using ModFinder;

// Get command line arguments
string gamePath = Environment.GetCommandLineArgs()[1];
string waitTime = Environment.GetCommandLineArgs()[2];

// Preset variables
DateTime startTime = DateTime.Now;

// Instatiate managers
Logger logger = new();
FileManager fileManager = new FileManager(logger);
ScriptManager scriptManager = new(logger, gamePath, fileManager, int.Parse(waitTime));




// Begin script
logger.LogMessage("\n============== LOGS ================");
try
{
    string failedModLocation = scriptManager.FindFailureInModFolder();
    scriptManager.CloseGame();

    string failedModName = scriptManager.FindFailedFile(failedModLocation);

    fileManager.CleanUp(
        scriptManager.GetTempModPath(), scriptManager.GetModFolderPath(), true);

    logger.LogSuccess($"Failed mod found ==> {failedModName}");

    logger.LogMessage("\n==============================");
    logger.LogMessage($"Start time: {startTime} ==> End time: {DateTime.Now}");
    logger.LogMessage("==============================");
}
catch (Exception ex)
{
    logger.LogMessage("\n==============================");
    logger.LogError($"Something went wrong - {ex.Message}");
    logger.LogMessage("==============================");
}
