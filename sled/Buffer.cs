namespace sled;

internal static class Buffer
{
    internal static List<string> buffer = [];
    internal static bool appendModeEnabled = false;
    
    internal static void ListLineFromIndex(int index)
    {
        if (Config.ShowLineNumbersOnList)
            Console.WriteLine($"[{index+1:D4}]~" + buffer[index]);
        else Console.Write(buffer[index]);
    }

    internal static void WriteToFile(string filepath)
    {
        File.WriteAllLines(filepath, buffer);
        if (!Config.ReportBytesWritten) return;
        var bytesWritten = File.ReadAllBytes(filepath).LongLength;
        if (Config.VerboseOutput)
            Console.WriteLine($"{bytesWritten} bytes written.");
        else Console.WriteLine(bytesWritten);
    }
}