namespace sled;

internal static class Config
{
    /// <summary>
    /// Enables the use of the buffer backup file.
    /// Can be changed during runtime via the backup (b) command.
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
    
    /// <summary>
    /// Changes the path where sled.bak is saved. Must be an absolute path.
    /// </summary>
    internal static string BackupFilePath = string.Empty;
    
    /// <summary>
    /// Show line numbers when using the list (l) command or when being listed by ListBufferOnLoad or ListBufferOnCopy.
    /// </summary>
    internal static bool ShowLineNumbersOnList = true;
    
    /// <summary>
    /// Show verbose error messages instead of just a ?.
    /// Can be changed during runtime via the verbose (v) command.
    /// </summary>
    internal static bool VerboseOutput = true;

    /// <summary>
    /// Reports the amount of bytes written on write commands.
    /// </summary>
    internal static bool ReportBytesWritten = true;
}