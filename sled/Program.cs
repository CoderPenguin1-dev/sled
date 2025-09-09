class Program
{
    static List<string> buffer = [];
    static bool appendMode = false;
    static bool backupEnabled = false;

    public static void Main(string[] args)
    {
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
                    HandleScript(args[1..args.Length]);
                else if (args[0] == "l")
                    foreach (string line in File.ReadAllLines(args[1]))
                        buffer.Add(line);
                else throw new Exception();
            }
            catch
            {
                // Reset the buffer if any error occurs.
                Console.WriteLine("?");
                buffer = [];
            }
        }
        while (true)
        {
            if (!appendMode) Console.Write(':');
            string input = Console.ReadLine();
            if (appendMode)
                HandleAppendMode(input);
            else try
            {
                HandleInput(input);
                if (backupEnabled)
                    File.WriteAllLines("sled.bak", buffer);
            }
            catch { Console.WriteLine('?'); }
        }
    }

    static string CombineFrom(string[] array, int fromIndex)
    {
        string output = string.Join(" ", array[fromIndex..array.Length]);
        return output;
    }

    static void HandleAppendMode(string input)
    {
        if (input == ".") appendMode = false;
        else
        {
            buffer.Add(input);
            if (backupEnabled)
                File.WriteAllLines("sled.bak", buffer);
        }
    }

    static void HandleScript(string[] script)
    {
        foreach (string line in script)
        {
            if (appendMode)
                HandleAppendMode(line);
            else try
            {
                HandleInput(line);
                if (backupEnabled)
                    File.WriteAllLines("sled.bak", buffer);
            }
            catch { Console.WriteLine('?'); break; }
        }
    }

    static void HandleInput(string input)
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
                    buffer.Insert(int.Parse(inputs[1]) - 1, "");
                else
                    buffer.Insert(int.Parse(inputs[1]) - 1, CombineFrom(inputs, 2));
                break;

            case "d":
                if (inputs.Length == 2)
                    buffer.RemoveAt(int.Parse(inputs[1]) - 1);
                if (inputs.Length == 3)
                {
                    buffer.RemoveRange(int.Parse(inputs[1]) - 1, (int.Parse(inputs[2]) - int.Parse(inputs[1]) + 1));
                }
                break;

            case "w":
                File.WriteAllLines(CombineFrom(inputs, 1), buffer); break;

            case "l":
                if (inputs.Length == 2)
                {
                    if (inputs[1] == ".") inputs[1] = "1";
                    Console.WriteLine($"[{int.Parse(inputs[1]):D4}]~" + buffer[int.Parse(inputs[1]) - 1]);
                }
                else if (inputs.Length == 3)
                {
                    if (inputs[1] == ".") inputs[1] = "1";
                    if (inputs[2] == ".") inputs[2] = buffer.Count.ToString();
                    for (int i = int.Parse(inputs[1]) - 1; i < int.Parse(inputs[2]); i++)
                        Console.WriteLine($"[{i + 1:D4}]~" + buffer[i]);
                }
                else
                    for (int i = 0; i < buffer.Count; i++)
                        Console.WriteLine($"[{i + 1:D4}]~" + buffer[i]);
                break;

            case "q":
                if (File.Exists("sled.bak")) File.Delete("sled.bak");
                Environment.Exit(0); break;

            case "a":
                if (inputs.Length >= 3)
                {
                    string line = buffer[int.Parse(inputs[1]) - 1];
                    line += CombineFrom(inputs, 2);
                    buffer[int.Parse(inputs[1]) - 1] = line;
                }
                else
                    appendMode = true;
                break;

            case "b":
                backupEnabled = !backupEnabled;
                Console.WriteLine($"Backup Buffer: {backupEnabled}");
                break;

            case "wq":
                File.WriteAllLines(CombineFrom(inputs, 1), buffer);
                Environment.Exit(0);
                break;

            case "c":
                buffer = [.. File.ReadAllLines(inputs[1])]; break;

            case "r":
                buffer[int.Parse(inputs[1]) - 1] = CombineFrom(inputs, 2); break;

            case "s":
                buffer[int.Parse(inputs[1]) - 1] = buffer[int.Parse(inputs[1]) - 1].Replace(oldValue: inputs[2], CombineFrom(inputs, 3));
                break;

            case "f":
                StringComparison sc;
                if (inputs[1] == "1") sc = StringComparison.CurrentCulture;
                else if (inputs[1] == "0") sc = StringComparison.CurrentCultureIgnoreCase;
                else throw new Exception(); // Return error to main loop.
                string content = CombineFrom(inputs, 2);
                for (int i = 0; i < buffer.Count; i++)
                {
                    if (buffer[i].Contains(content, sc))
                        Console.WriteLine(i + 1);
                }
                break;

            case "?":
                Console.WriteLine($"Sharp Line-based EDitor v{typeof(Program).Assembly.GetName().Version}\n");
                Console.WriteLine("Command Mode allows you to type in the below commands. Append Mode allows you to add to the buffer.");
                Console.WriteLine("Command Mode is the default mode and is indicated by a colon (:) in the input field.");
                Console.WriteLine("You can exit Append Mode by entering a single period/full-stop (.).");
                Console.WriteLine("q - Closes SLED.");
                Console.WriteLine("w [filepath] - Write buffer to specifced file. Will create file if it doesn't exist.");
                Console.WriteLine("wq [filepath] - Equivalent to w and q.");
                Console.WriteLine("b - Toggle Backup. Default is off/false.");
                Console.WriteLine("a - Enter Append Mode.");
                Console.WriteLine("a [line] [content] - Append content to the end of the line.");
                Console.WriteLine("i [line] - Insert newline on specificed line in the buffer.");
                Console.WriteLine("i [line] [content] Insert content on specified line in the buffer.");
                Console.WriteLine("d [line] - Delete line from buffer.");
                Console.WriteLine("d [from line] [to line] - Deletes the specifed line range of lines");
                Console.WriteLine("r [line] [content] - Replace line in buffer with specified content.");
                Console.WriteLine("s [line] [old content] [new content] - Replace all occurances of the old content with the new content in the specified line in the buffer");
                Console.WriteLine("c [filepath] - Overwrite buffer with specifed file.");
                Console.WriteLine("l - List buffer");
                Console.WriteLine("l [line or . for line 1] - Print specifed line from the buffer.");
                Console.WriteLine("l [line or . for line 1] [line or . for all lines up to EOF] - Print specified range of lines from the buffer.");
                Console.WriteLine("f [0 for case-insensitive or 1 for case-sensitive] [content] - Find and print the line numbers that contain the content.");
                break;

        }
    }
}