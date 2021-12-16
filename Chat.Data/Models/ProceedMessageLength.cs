namespace Chat.Data.Models;

public static class ProceedMessageLength
{
    public static int GetMessageLength(byte[] arr)
    {
        return (arr[0] << 8) + arr[1];
    }

    public static int GetDbMessagesLength(byte[] arr)
    {
        return (arr[0] << 24) + (arr[1] << 16) + (arr[2] << 8) + arr[3];
    }
}