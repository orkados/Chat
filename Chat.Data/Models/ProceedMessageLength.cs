namespace Chat.Data.Models;

public static class ProceedMessageLength
{
    public static int GetLength(byte[] arr)
    {
        return (arr[0] << 8) + arr[1];
    }
}