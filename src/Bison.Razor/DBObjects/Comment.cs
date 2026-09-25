using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Comment
{
    [Key]
    [Required]
    public required int AuthorId { get; set; }

    [Required]
    public required int ObservationId { get; set; }

    [Required]
    public required string comment { get; set; }

    public User Author { get; set; }

    public Observation observation { get; set; }
}