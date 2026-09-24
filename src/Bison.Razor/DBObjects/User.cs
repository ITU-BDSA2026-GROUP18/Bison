using System.ComponentModel.DataAnnotations;

public class User
{
    [Required]
    public required int userId;

    [Required]
    public required string username;

    [Required]
    public required string email;
}