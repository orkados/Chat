using System.ComponentModel.DataAnnotations;

namespace Chat.Data.Models;

public class Message
{
    [Key] public int Id { get; set; }
    public string Username { get; set; }
    public string Content { get; set; }
    public string Date { get; set; }
}