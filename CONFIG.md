# sled Configuration

## Environment Variables (Linux Only)
* SLED_CONFIG_FILE
   * The path to the file where the config file is. 
   * Overrides all other config paths.
* SLED_BACKUP_FILE_PATH
   * The path to the folder where `sled.bak` will be saved to.
   * Overrides the BackupFilePath config.

## Configuration File
### Config File Locations
* sled.conf
  * In the working directory.
* ~/.config/sled.conf
  * Linux only
* %APPDATA%/sled.conf
  * Windows Only

### Config Options
**Layout:** `KEY PARAMETER`

* BackupEnabled [True/False] False
  * Sets if the backup is enabled on startup.
* BackupFilePath [Absolute Path To Folder] Working Directory
  * Sets the path to the folder where `sled.bak` will be saved to.
* ListBufferOnCopy [True/False] False
  * Lists the buffer when using the copy (c) command.
* ListBufferOnLoad [True/False] False
  * Lists the buffer when loading a file using the load (l) mode.
* AppendModeOnStart [True/False] False
  * Starts sled in append mode instead of command mode.