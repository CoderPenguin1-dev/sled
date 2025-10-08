namespace sled;

internal static class Config
{
    /// <summary>
    /// Enables the use of the buffer backup file. Can be changed during runtime.
    /// </summary>
    internal static bool BackupEnabled = false;
    /// <summary>
    /// Lists the buffer when using the copy (c) command.
    /// </summary>
    internal static bool ListBufferOnCopy = false;
    /// <summary>
    /// Lists the buffer when loading a file with the load (l) mode.
    /// </summary>
    internal static bool ListBufferOnLoad = false;
    /// <summary>
    /// Enables Append Mode when starting sled. Ignored when running from a script.
    /// </summary>
    internal static bool AppendModeOnStart = false;

    internal static string BackupFilePath = string.Empty;
}