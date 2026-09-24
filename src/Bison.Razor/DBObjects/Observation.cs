using System.ComponentModel.DataAnnotations;
using SQLitePCL;

public class Observation
{
    [Required]
    public required int observationId;
    public int authorId;
    
    [Required]
    public required string text;
    public int pubDate;

    [Required]
    public required string location;
    public User author;
}