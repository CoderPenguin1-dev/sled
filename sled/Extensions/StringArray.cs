namespace sled.Extensions;

public static class StringArray
{
    public static string JoinFrom(this string[] strArray, int fromIndex)
    {
        return string.Join(" ", strArray[fromIndex..]);
    }
}