using System.ComponentModel.DataAnnotations;

namespace Bison.Razor.ViewModels;

public class CommentInput
{
    public int? ParentCommentId { get; set; }

    [Required, StringLength(2000, MinimumLength = 1)]
    public string Content { get; set; } = "";
}