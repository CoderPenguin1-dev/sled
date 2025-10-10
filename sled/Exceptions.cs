namespace sled;

public static class Exceptions
{
    public static readonly Exception InvalidCommand = new("Invalid command.");
    public static readonly Exception InvalidMode = new("Invalid mode.");

    internal static void HandleExceptions(Exception ex)
    {
        if (Config.VerboseErrors)
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

                case IndexOutOfRangeException or ArgumentOutOfRangeException:
                    errorMessage = "Line number out of range.";
                    break;
            }

            Console.WriteLine($"?: {errorMessage}");
        }
        else Console.WriteLine("?");
    }
}