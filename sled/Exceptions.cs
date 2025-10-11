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
            string errorMessage;
            switch (ex)
            {
                default:
                    errorMessage = ex.Message;
                    break;
                
                case FormatException:
                    errorMessage = "Invalid argument(s).";
                    break;
                case FileNotFoundException:
                    errorMessage = "File not found.";
                    break;

                // Equiv. to InvalidParameter.
                case IndexOutOfRangeException or ArgumentOutOfRangeException:
                    errorMessage = "Line number out of range.";
                    break;
            }

            Console.WriteLine($"?: {errorMessage}");
        }
        else Console.WriteLine("?");
    }
}