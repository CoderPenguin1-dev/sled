namespace sled;

internal static class Buffer
{
    internal static List<string> BufferLines = [];
    internal static bool AppendModeEnabled = false;
    
    internal static void ListLineFromIndex(int index)
    {
        if (Config.ShowLineNumbersOnList)
            Console.WriteLine($"[{index+1:D4}]~" + BufferLines[index]);
        else Console.Write(BufferLines[index]);
    }

    internal static void WriteToFile(string filepath)
    {
        File.WriteAllLines(filepath, BufferLines);
        if (!Config.ReportBytesWritten) return;
        var bytesWritten = File.ReadAllBytes(filepath).LongLength;
        if (Config.VerboseOutput)
            Console.WriteLine($"{bytesWritten} bytes written.");
        else Console.WriteLine(bytesWritten);
    }
}