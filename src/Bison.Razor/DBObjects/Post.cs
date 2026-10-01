using System.ComponentModel.DataAnnotations;

public class Post
{
    [Required]
    public required int Id { get; set;}

    [Required]
    public required string Text { get; set; }

    public double TimeStamp { get; set; }

    [Required]
    public required User Author { get; set; } 
}