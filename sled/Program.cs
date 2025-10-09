namespace sled;

public class Program
{
    private static List<string> _buffer = [];
    private static bool _appendModeEnabled = false;

    public static void Main(string[] args)
    {
        #region Config
        // Check if any config file exists. Use defaults otherwise.
        if (ConfigFileReader.InitConfigFile())
        {
            try
            {
                if (ConfigFileReader.GetKeyValue("BackupEnabled") != string.Empty)
                    Config.BackupEnabled = bool.Parse(ConfigFileReader.GetKeyValue("BackupEnabled"));
                if (ConfigFileReader.GetKeyValue("ListBufferOnCopy") != string.Empty)
                    Config.ListBufferOnCopy = bool.Parse(ConfigFileReader.GetKeyValue("ListBufferOnCopy"));
                if (ConfigFileReader.GetKeyValue("ListBufferOnLoad") != string.Empty)
                    Config.ListBufferOnLoad = bool.Parse(ConfigFileReader.GetKeyValue("ListBufferOnLoad"));
                if (ConfigFileReader.GetKeyValue("AppendModeOnStart") != string.Empty)
                    Config.AppendModeOnStart = bool.Parse(ConfigFileReader.GetKeyValue("AppendModeOnStart"));
                if (ConfigFileReader.GetKeyValue("BackupFilePath") != string.Empty)
                    Config.BackupFilePath = ConfigFileReader.GetKeyValue("BackupFilePath");
                if (ConfigFileReader.GetKeyValue("ShowLineNumbersOnList") != string.Empty)
                    Config.ShowLineNumbersOnList = bool.Parse(ConfigFileReader.GetKeyValue("ShowLineNumbersOnList"));
            }
            catch
            {
                Console.WriteLine("Error in config file.");
                Environment.Exit(1);
            }
        }

        if (Config.AppendModeOnStart)
            _appendModeEnabled = true;
        
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
        
        if (args.Length > 0)
        {
            try
            {
                if (args[0] == "s")
                {
                    string[] inputs = File.ReadAllLines(args[1]);
                    HandleScript(inputs);
                }
                else if (args[0] == "x")
                    HandleScript(args[1..]);
                else if (args[0] == "l")
                {
                    foreach (string line in File.ReadAllLines(args[1]))
                        _buffer.Add(line);
                    if (Config.ListBufferOnLoad)
                        for (int i = 0; i < _buffer.Count; i++)
                            ListLineFromIndex(_buffer.ToArray(), i);
                }
                
                else throw new Exception();
            }
            catch
            {
                // Reset the buffer if any error occurs.
                Console.WriteLine("?");
                _buffer = [];
            }
        }
        
        while (true)
        {
            if (!_appendModeEnabled) Console.Write(':');
            string input = Console.ReadLine();
            if (_appendModeEnabled)
                HandleAppendMode(input);
            else try
                {
                    HandleInput(input);
                    if (Config.BackupEnabled)
                        File.WriteAllLines($"{Config.BackupFilePath}sled.bak", _buffer);
                }
                catch { Console.WriteLine('?'); }
        }
    }

    private static string CombineFrom(string[] array, int fromIndex)
    {
        string output = string.Join(" ", array[fromIndex..array.Length]);
        return output;
    }
    
    private static void ListLineFromIndex(string[] buffer, int index)
    {
        if (Config.ShowLineNumbersOnList)
            Console.WriteLine($"[{index+1:D4}]~" + buffer[index]);
        else  Console.Write(buffer[index]);
    }

    #region  Handlers
    private static void HandleAppendMode(string input)
    {
        if (input == ".") _appendModeEnabled = false;
        else
        {
            _buffer.Add(input);
            if (Config.BackupEnabled)
                File.WriteAllLines($"{Config.BackupFilePath}sled.bak", _buffer);
        }
    }

    private static void HandleScript(string[] script)
    {
        // Make sure AppendModeOnStart can never take effect.
        _appendModeEnabled = false;
        foreach (string line in script)
        {
            if (_appendModeEnabled)
                HandleAppendMode(line);
            else try
                {
                    HandleInput(line);
                    if (Config.BackupEnabled)
                        File.WriteAllLines("sled.bak", _buffer);
                }
                catch { Console.WriteLine('?'); break; }
        }
    }

    private static void HandleInput(string input)
    {
        string[] inputs = input.Split(" ");
        switch (inputs[0].ToLower())
        {
            default:
                Console.WriteLine("?");
                break;

            case "":
                break;

            case "i":
                if (inputs.Length == 2)
                    _buffer.Insert(int.Parse(inputs[1]) - 1, "");
                else
                    _buffer.Insert(int.Parse(inputs[1]) - 1, CombineFrom(inputs, 2));
                break;

            case "d":
                if (inputs.Length == 2)
                    _buffer.RemoveAt(int.Parse(inputs[1]) - 1);
                if (inputs.Length == 3)
                {
                    _buffer.RemoveRange(int.Parse(inputs[1]) - 1, (int.Parse(inputs[2]) - int.Parse(inputs[1]) + 1));
                }
                break;

            case "w":
                File.WriteAllLines(CombineFrom(inputs, 1).Replace("\"", null), _buffer); break;

            case "l":
                if (inputs.Length == 2)
                {
                    if (inputs[1] == ".") inputs[1] = "1";
                    ListLineFromIndex(_buffer.ToArray(), int.Parse(inputs[1]) - 1);
                }
                else if (inputs.Length == 3)
                {
                    if (inputs[1] == ".") inputs[1] = "1";
                    if (inputs[2] == ".") inputs[2] = _buffer.Count.ToString();
                    for (int i = int.Parse(inputs[1]) - 1; i < int.Parse(inputs[2]); i++)
                        ListLineFromIndex(_buffer.ToArray(), i);
                }
                else
                    for (int i = 0; i < _buffer.Count; i++)
                    {
                        ListLineFromIndex(_buffer.ToArray(), i);
                    }
                break;

            case "q":
                if (File.Exists($"{Config.BackupFilePath}sled.bak")) File.Delete($"{Config.BackupFilePath}sled.bak");
                Environment.Exit(0); break;

            case "a":
                if (inputs.Length >= 3)
                {
                    string line = _buffer[int.Parse(inputs[1]) - 1];
                    line += CombineFrom(inputs, 2);
                    _buffer[int.Parse(inputs[1]) - 1] = line;
                }
                else
                    _appendModeEnabled = true;
                break;

            case "b":
                Config.BackupEnabled = !Config.BackupEnabled;
                Console.WriteLine($"Backup Buffer: {Config.BackupEnabled}");
                break;

            case "wq":
                if (File.Exists($"{Config.BackupFilePath}sled.bak")) File.Delete($"{Config.BackupFilePath}sled.bak");
                File.WriteAllLines(CombineFrom(inputs, 1).Replace("\"", null), _buffer);
                Environment.Exit(0);
                break;

            case "c":
                _buffer = [.. File.ReadAllLines(CombineFrom(inputs, 1).Replace("\"", null))];
                if (Config.ListBufferOnCopy)
                {
                    for (int i = 0; i < _buffer.Count; i++)
                        ListLineFromIndex(_buffer.ToArray(), i);
                }
                break;

            case "r":
                _buffer[int.Parse(inputs[1]) - 1] = CombineFrom(inputs, 2); break;

            case "s":
                _buffer[int.Parse(inputs[1]) - 1] = _buffer[int.Parse(inputs[1]) - 1].Replace(oldValue: inputs[2], CombineFrom(inputs, 3));
                break;

            case "f":
                StringComparison sc;
                if (inputs[1] == "1") sc = StringComparison.CurrentCulture;
                else if (inputs[1] == "0") sc = StringComparison.CurrentCultureIgnoreCase;
                else throw new Exception(); // Return error to main loop.
                string content = CombineFrom(inputs, 2);
                for (int i = 0; i < _buffer.Count; i++)
                {
                    if (_buffer[i].Contains(content, sc))
                        Console.WriteLine(i + 1);
                }
                break;

            case "?":
                Console.WriteLine($"Sharp Line-based EDitor v{typeof(Program).Assembly.GetName().Version}\n");
                Console.WriteLine("Command Mode allows you to type in the below commands. Append Mode allows you to add to the buffer.");
                Console.WriteLine("Command Mode is the default mode and is indicated by a colon (:) in the input field.");
                Console.WriteLine("You can exit Append Mode by entering a single period/full-stop (.).");
                Console.WriteLine("q - Closes sled.");
                Console.WriteLine("w [absolute file path] - Write buffer to specified file. Will create file if it doesn't exist.");
                Console.WriteLine("wq [absolute file path] - Equivalent to w and q.");
                Console.WriteLine("b - Toggle Backup. Default is off/false.");
                Console.WriteLine("a - Enter Append Mode.");
                Console.WriteLine("a [line] [content] - Append content to the end of the line.");
                Console.WriteLine("i [line] - Insert newline on specified line in the buffer.");
                Console.WriteLine("i [line] [content] Insert content on specified line in the buffer.");
                Console.WriteLine("d [line] - Delete line from buffer.");
                Console.WriteLine("d [from line] [to line] - Deletes the specified line range of lines.");
                Console.WriteLine("r [line] [content] - Replace line in buffer with specified content.");
                Console.WriteLine("s [line] [old content] [new content] - Replace all occurrences of the old content with the new content in the specified line in the buffer.");
                Console.WriteLine("c [absolute file path] - Overwrite buffer with specified file.");
                Console.WriteLine("l - List buffer.");
                Console.WriteLine("l [line or . for line 1] - Print specified line from the buffer.");
                Console.WriteLine("l [line or . for line 1] [line or . for all lines up to EOF] - Print specified range of lines from the buffer.");
                Console.WriteLine("f [0 for case-insensitive or 1 for case-sensitive] [content] - Find and print the line numbers that contain the content.");
                break;
        }
    }
    #endregion
}