namespace sled;

internal static class ConfigFileReader
{
    private static string[] _configFile = [];

    /// <summary>
    /// Finds and reads the config file.
    /// Must be called before any other operation with the config file is done.
    /// </summary>
    /// <returns>Returns true if a config could be found; false if none could be found.</returns>
    internal static bool InitConfigFile()
    {
        string configFilePath = string.Empty;

        // Goes in this order of priority in order of most to least:
        // SLED_CONFIG_FILE -> sled.conf -> AppData Folder (Windows) or ~/.config/sled.conf (Linux)
        #region Check Config Paths

        #region OS Specific Paths
        // Linux user config folder check.
        if (OperatingSystem.IsLinux())
        {
            string homeFolder = Environment.GetEnvironmentVariable("HOME");
            if (homeFolder != null)
                if (File.Exists($"{homeFolder}/.config/sled.conf"))
                    configFilePath = Environment.ExpandEnvironmentVariables($"{homeFolder}/.config/sled.conf");
        }
        
        // Windows user appdata folder check.
        // Currently untested.
        if (OperatingSystem.IsWindows())
        {
            string appdataFolder = Environment.GetEnvironmentVariable("APPDATA");
            if (File.Exists($"{appdataFolder}/sled.conf"))
                configFilePath = $"{appdataFolder}/sled.conf";
        }
        #endregion
        
        if (File.Exists("sled.conf"))
            configFilePath = "sled.conf";
        #endregion
        
        string sledConfigFilePath = Environment.GetEnvironmentVariable("SLED_CONFIG_FILE");
        if (sledConfigFilePath != null)
            if (File.Exists(sledConfigFilePath))
                configFilePath = sledConfigFilePath;
        
        
        if (configFilePath == string.Empty)
            return false;
        
        _configFile = File.ReadAllLines(configFilePath);
        return true;
    }
    

    /// <param name="key">Name of key in config file.</param>
    /// <returns>The value of the specified key.</returns>
    private static string GetKeyValue(string key)
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
    
    /// <param name="key">Key in config file.</param>
    /// <param name="configOption">Boolean-based option to set.</param>
    internal static void SetConfigOption(string key, ref bool configOption)
    {
        if (GetKeyValue(key) != string.Empty)
            configOption = bool.Parse(GetKeyValue(key));
    }
    
    /// <param name="key">Key in config file.</param>
    /// <param name="configOption">String-based option to set.</param>
    internal static void SetConfigOption(string key, ref string configOption)
    {
        if (GetKeyValue(key) != string.Empty)
            configOption = GetKeyValue(key);
    }
}