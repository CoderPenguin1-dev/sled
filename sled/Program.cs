namespace sled;

public class Program
{
    public static void Main(string[] args)
    {
        #region Config
        // Check if any config file exists. Use defaults otherwise.
        if (ConfigFileReader.InitConfigFile())
        {
            try
            {
                ConfigFileReader.SetConfigOption("BackupEnabled", ref Config.BackupEnabled);
                ConfigFileReader.SetConfigOption("ListBufferOnCopy", ref Config.ListBufferOnCopy);
                ConfigFileReader.SetConfigOption("ListBufferOnLoad", ref Config.ListBufferOnLoad);
                ConfigFileReader.SetConfigOption("AppendModeOnStart", ref Config.AppendModeOnStart);
                ConfigFileReader.SetConfigOption("BackupFilePath", ref Config.BackupFilePath);
                ConfigFileReader.SetConfigOption("ShowLineNumbersOnList", ref Config.ShowLineNumbersOnList);
                ConfigFileReader.SetConfigOption("VerboseOutput", ref Config.VerboseOutput);
                ConfigFileReader.SetConfigOption("ReportBytesWritten", ref Config.ReportBytesWritten);
            }
            catch
            {
                Console.WriteLine("Error in config file.");
                Environment.Exit(1);
            }
        }

        if (Config.AppendModeOnStart)
            Buffer.appendModeEnabled = true;
        
        // Environment variable overrides.
        string backupFilePathEnv = Environment.GetEnvironmentVariable("SLED_BACKUP_FOLDER");
        if (backupFilePathEnv != null)
            if (Path.Exists(backupFilePathEnv))
                Config.BackupFilePath = backupFilePathEnv;
            else
            {
                Console.WriteLine("SLED_BACKUP_FILE_PATH: Path not found.");
                Environment.Exit(1);
            }
        string backupEnabledEnv = Environment.GetEnvironmentVariable("SLED_BACKUP_ENABLED");
        if (backupEnabledEnv != null)
            if (!bool.TryParse(backupEnabledEnv, out Config.BackupEnabled))
            {
                Console.WriteLine("SLED_BACKUP_ENABLED: Invalid value given.");
                Environment.Exit(1);
            }
        #endregion
        
        // Check what mode sled started in.
        if (args.Length > 0)
        {
            try
            {
                if (args[0] == "s")
                {
                    string[] inputs = File.ReadAllLines(args[1]);
                    IO.HandleScript(inputs);
                }
                else if (args[0] == "x")
                    IO.HandleScript(args[1..]);
                else if (args[0] == "l")
                {
                    foreach (string line in File.ReadAllLines(args[1]))
                        Buffer.buffer.Add(line);
                    if (Config.ListBufferOnLoad)
                        for (int i = 0; i < Buffer.buffer.Count; i++)
                            Buffer.ListLineFromIndex(i);
                }

                else throw Exceptions.InvalidMode;
            }
            catch (Exception ex)
            {
                // Reset the buffer if any error occurs.
                Exceptions.HandleExceptions(ex);
                Buffer.buffer.Clear();
            }
        }
        
        // Main loop.
        while (true)
        {
            if (!Buffer.appendModeEnabled) Console.Write(':');
            string input = Console.ReadLine();
            if (Buffer.appendModeEnabled)
                IO.HandleAppendMode(input);
            else
                try
                {
                    IO.HandleCommands(input);
                    if (Config.BackupEnabled)
                        File.WriteAllLines($"{Config.BackupFilePath}sled.bak", Buffer.buffer);
                }
                catch (Exception ex)
                {
                    Exceptions.HandleExceptions(ex);
                }
        }
    }
}