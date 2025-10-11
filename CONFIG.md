# sled Configuration

## Environment Variables
All of these will override the config.
* SLED_CONFIG_FILE
   * The path to the file where the config file is. 
   * Read over all other config paths.
* SLED_BACKUP_FOLDER
   * The path to the folder where `sled.bak` will be saved to.
* SLED_BACKUP_ENABLED
  * Accepts True/False and true/false.
  * Sets if the backup is enabled on startup.

## Configuration File
Can use #'s to denote comments.

**Config Option Layout:** `KEY PARAMETER`

### Default Config File Locations
The local config will always be read over the user-wide configs.
* sled.conf
  * In the working directory.
  * Overides the user-wide config.
* ~/.config/sled.conf
  * Linux only user-wide config.
* %APPDATA%/sled.conf
  * Windows only user-wide config.

### Config Options
`KEY [ACCEPTED PARAMETERS] DEFAULT PARAMETER`
* BackupEnabled [True/False] False
  * Sets if the backup is enabled on startup.
* BackupFilePath [Absolute Path To Folder] Working Directory
  * Sets the path to the folder where `sled.bak` will be saved to.
  * Must end with a `/` oor `\`, depending on operating system.
* ListBufferOnCopy [True/False] False
  * Lists the buffer when using the copy (c) command.
* ListBufferOnLoad [True/False] False
  * Lists the buffer when loading a file using the load (l) mode.
* AppendModeOnStart [True/False] False
  * Starts sled in append mode instead of command mode.
  * Ignored when running a script with either script mode (s and x).
* ShowLineNumbersOnList [True/False] True
  * Show line numbers when using the list (l) command or when being listed by ListBufferOnLoad or ListBufferOnCopy.
* VerboseOutput [True/False] True
  * Show verbose output messages instead of basic information.
    * Full error messages instead of just `?`.
* ReportBytesWritten [True/False] True
  * Reports the amount of bytes written when writing the buffer to a file.