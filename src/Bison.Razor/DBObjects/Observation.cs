using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SQLitePCL;

public class Observation
{
    [Key]
    [Column("observation_id")]
    [Required]
    public required int ObservationId { get; set; }
    
    [Column("author_id")]
    public int AuthorId { get; set; }
    
    [Column("text")]
    [Required]
    public required string Text { get; set; } = "";

    [Column("pub_date")]
    public double PubDate { get; set; }
    public User Author { get; set; } = null;
}