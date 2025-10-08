namespace sled;

internal static class ConfigFileReader
{
    private static string[] _configFile = [];

    internal static bool InitConfigFile()
    {
        string configFilePath = string.Empty;

        // Goes in this order of "power"
        // AppData Folder (Windows) or SLED_CONFIG_FILE (Linux) -> ~/.config/sled.conf (Linux) -> sled.conf
        #region Check Config Paths
        if (File.Exists("sled.conf"))
            configFilePath = "sled.conf";

        #region OS Specific Checks
        if (OperatingSystem.IsOSPlatform("Linux"))
        {
            string homeFolder = Environment.GetEnvironmentVariable("HOME");
            string sledConfigFilePath = Environment.GetEnvironmentVariable("SLED_CONFIG_FILE");
            if (homeFolder != null)
                if (File.Exists($"{homeFolder}/.config/sled.conf"))
                    configFilePath = Environment.ExpandEnvironmentVariables($"{homeFolder}/.config/sled.conf");
            if (sledConfigFilePath != null)
                if (File.Exists(sledConfigFilePath))
                    configFilePath = sledConfigFilePath;
        }

        if (OperatingSystem.IsOSPlatform("Windows"))
        {
            if (File.Exists(Environment.ExpandEnvironmentVariables("%APPDATA%/sled.conf")))
                configFilePath = Environment.ExpandEnvironmentVariables("%APPDATA%/sled.conf");
        }
        #endregion
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