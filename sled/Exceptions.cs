namespace sled;

public static class Exceptions
{
    public static readonly Exception InvalidCommand = new("Invalid command.");
    public static readonly Exception InvalidMode = new("Invalid mode.");
    public static readonly Exception InvalidParameter = new("Invalid argument(s).");
    
    internal static void HandleExceptions(Exception ex)
    {
        if (Config.VerboseOutput)
        {
            string errorMessage = ex switch
            {
                FormatException => "Invalid argument(s).", // Equiv. to InvalidParameter.
                FileNotFoundException => "File not found.",
                IndexOutOfRangeException or ArgumentOutOfRangeException => "Index out of range.",
                _ => ex.Message
            };

            Console.WriteLine($"?: {errorMessage}");
        }
        else Console.WriteLine("?");
    }
}