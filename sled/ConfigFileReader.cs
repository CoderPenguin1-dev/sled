namespace sled;

internal static class ConfigFileReader
{
    private static string[] _configFile = [];

    internal static bool InitConfigFile()
    {
        string configFilePath = string.Empty;

        // Goes in this order of priority in order of most to least:
        // SLED_CONFIG_FILE (Linux) -> sled.conf -> AppData Folder (Windows) or ~/.config/sled.conf (Linux)
        #region Check Config Paths
        
        // Linux user config folder check.
        if (OperatingSystem.IsLinux())
        {
            string homeFolder = Environment.GetEnvironmentVariable("HOME");
            if (homeFolder != null)
                if (File.Exists($"{homeFolder}/.config/sled.conf"))
                    configFilePath = Environment.ExpandEnvironmentVariables($"{homeFolder}/.config/sled.conf");
            
        }
        
        string sledConfigFilePath = Environment.GetEnvironmentVariable("SLED_CONFIG_FILE");
        if (sledConfigFilePath != null)
            if (File.Exists(sledConfigFilePath))
                configFilePath = sledConfigFilePath;
        
        // Windows appdata check.
        if (OperatingSystem.IsWindows())
        {
            if (File.Exists(Environment.ExpandEnvironmentVariables("%APPDATA%/sled.conf")))
                configFilePath = Environment.ExpandEnvironmentVariables("%APPDATA%/sled.conf");
        }
        
        if (File.Exists("sled.conf"))
            configFilePath = "sled.conf";
        #endregion
        
        if (configFilePath == string.Empty)
            return false;
        
        _configFile = File.ReadAllLines(configFilePath);
        return true;
    }
    
    internal static string GetKeyValue(string key)
    {
        foreach (string line in _configFile)
        {
            // Comment Line
            if (line.StartsWith('#'))
                continue;
            if (line.StartsWith(key))
                return line.Split(" ")[1];
        }
        return string.Empty;
    }
}