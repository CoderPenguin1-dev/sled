namespace sled;
using Extensions;
internal static class IO
{
    internal static void HandleAppendMode(string input)
    {
        if (input == ".") Buffer.AppendModeEnabled = false;
        else
        {
            Buffer.BufferLines.Add(input);
            if (Config.BackupEnabled)
                File.WriteAllLines($"{Config.BackupFilePath}sled.bak", Buffer.BufferLines);
        }
    }

    internal static void HandleScript(string[] script)
    {
        // Make sure AppendModeOnStart can never take effect.
        Buffer.AppendModeEnabled = false;
        foreach (string line in script)
        {
            if (Buffer.AppendModeEnabled)
                HandleAppendMode(line);
            else
                try
                {
                    HandleCommands(line);
                    if (Config.BackupEnabled)
                        File.WriteAllLines($"{Config.BackupFilePath}sled.bak", Buffer.BufferLines);
                }
                catch (Exception ex)
                {
                    Exceptions.HandleExceptions(ex);
                    break;
                }
        }
    }

    internal static void HandleCommands(string input)
    {
        string[] inputs = input.Split(" ");
        switch (inputs[0].ToLower())
        {
            default:
                throw Exceptions.InvalidCommand;

            case "":
                break;

            case "i":
                if (inputs.Length == 2)
                    Buffer.BufferLines.Insert(int.Parse(inputs[1]) - 1, "");
                else
                    Buffer.BufferLines.Insert(int.Parse(inputs[1]) - 1, inputs.JoinFrom(2));
                break;

            case "d":
                if (inputs.Length == 2)
                    Buffer.BufferLines.RemoveAt(int.Parse(inputs[1]) - 1);
                if (inputs.Length == 3)
                {
                    Buffer.BufferLines.RemoveRange(int.Parse(inputs[1]) - 1,
                        (int.Parse(inputs[2]) - int.Parse(inputs[1]) + 1));
                }

                break;

            case "w":
                Buffer.WriteToFile(inputs.JoinFrom(1).Replace("\"", null));
                break;

            case "l":
                if (inputs.Length == 2)
                {
                    if (inputs[1] == ".") inputs[1] = "1";
                    Buffer.ListLineFromIndex(int.Parse(inputs[1]) - 1);
                }
                else if (inputs.Length == 3)
                {
                    if (inputs[1] == ".") inputs[1] = "1";
                    if (inputs[2] == ".") inputs[2] = Buffer.BufferLines.Count.ToString();
                    for (int i = int.Parse(inputs[1]) - 1; i < int.Parse(inputs[2]); i++)
                        Buffer.ListLineFromIndex(i);
                }
                else
                    for (int i = 0; i < Buffer.BufferLines.Count; i++)
                    {
                        Buffer.ListLineFromIndex(i);
                    }

                break;

            case "q":
                if (inputs.Length == 2)
                    Buffer.WriteToFile(inputs.JoinFrom(1).Replace("\"", null));
                if (File.Exists($"{Config.BackupFilePath}sled.bak")) 
                    File.Delete($"{Config.BackupFilePath}sled.bak");
                Environment.Exit(0);
                break;

            case "a":
                if (inputs.Length >= 3)
                {
                    string line = Buffer.BufferLines[int.Parse(inputs[1]) - 1];
                    line += inputs.JoinFrom(2);
                    Buffer.BufferLines[int.Parse(inputs[1]) - 1] = line;
                }
                else Buffer.AppendModeEnabled = true;
                break;

            case "b":
                Config.BackupEnabled = !Config.BackupEnabled;
                if (Config.VerboseOutput)
                    Console.WriteLine($"Backup Buffer: {Config.BackupEnabled}");
                else Console.WriteLine(Config.BackupEnabled);
                break;

            case "c":
                Buffer.BufferLines = [.. File.ReadAllLines(inputs.JoinFrom(1).Replace("\"", null))];
                if (Config.ListBufferOnCopy)
                {
                    for (int i = 0; i < Buffer.BufferLines.Count; i++)
                        Buffer.ListLineFromIndex(i);
                }

                break;

            case "r":
                Buffer.BufferLines[int.Parse(inputs[1]) - 1] = inputs.JoinFrom(2); break;

            case "s":
                Buffer.BufferLines[int.Parse(inputs[1]) - 1] = Buffer.BufferLines[int.Parse(inputs[1]) - 1]
                    .Replace(oldValue: inputs[2], inputs.JoinFrom(3));
                break;

            case "f":
            {
                StringComparison sc;
                if (inputs[1] == "1") sc = StringComparison.CurrentCulture;
                else if (inputs[1] == "0") sc = StringComparison.CurrentCultureIgnoreCase;
                else throw Exceptions.InvalidParameter; // Return error to main loop.
                string content = inputs.JoinFrom(2);
                for (int i = 0; i < Buffer.BufferLines.Count; i++)
                {
                    if (Buffer.BufferLines[i].Contains(content, sc))
                        Console.WriteLine(i + 1);
                }

                break;
            }

            case "v":
                Config.VerboseOutput = !Config.VerboseOutput;
                if (Config.VerboseOutput)
                    Console.WriteLine($"Verbose Output: {Config.VerboseOutput}");
                else Console.WriteLine(Config.VerboseOutput);
                break;

            case "?":
                Console.WriteLine($"Sharp Line EDitor v{typeof(Program).Assembly.GetName().Version}\n");
                Console.WriteLine("Command Mode allows you to type in the below commands. Append Mode allows you to add to the BufferLines.");
                Console.WriteLine("Command Mode is the default mode and is indicated by a colon (:) in the input field.");
                Console.WriteLine("You can exit Append Mode by entering a single period/full-stop (.).");
                Console.WriteLine("q - Closes sled.");
                Console.WriteLine("q [absolute file path] - Equivalent to w and q.");
                Console.WriteLine("w [absolute file path] - Write BufferLines to specified file. Will create file if it doesn't exist.");
                Console.WriteLine("b - Toggle BufferLines backup. Default is off/false.");
                Console.WriteLine("a - Enter Append Mode.");
                Console.WriteLine("a [line] [content] - Append content to the end of the line.");
                Console.WriteLine("i [line] - Insert newline on specified line in the BufferLines.");
                Console.WriteLine("i [line] [content] Insert content on specified line in the BufferLines.");
                Console.WriteLine("d [line] - Delete line from BufferLines.");
                Console.WriteLine("d [from line] [to line] - Deletes the specified line range of lines.");
                Console.WriteLine("r [line] [content] - Replace line in BufferLines with specified content.");
                Console.WriteLine("s [line] [old content] [new content] - Replace all occurrences of the old content with the new content in the specified line in the BufferLines.");
                Console.WriteLine("c [absolute file path] - Overwrite BufferLines with specified file.");
                Console.WriteLine("l - List BufferLines.");
                Console.WriteLine("l [line or . for line 1] - Print specified line from the BufferLines.");
                Console.WriteLine("l [line or . for line 1] [line or . for all lines up to EOF] - Print specified range of lines from the BufferLines.");
                Console.WriteLine("f [0 for case-insensitive or 1 for case-sensitive] [content] - Find and print the line numbers that contain the content.");
                Console.WriteLine("v - Toggle verbose errors. Default is on/true.");
                break;
        }
    }
}