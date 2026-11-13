namespace Bison.Razor.ViewModels;

using System.ComponentModel.DataAnnotations;

public class CommentInput
{
	public int? ParentCommentId { get; set; }

	[Required, StringLength(2000, MinimumLength = 1)]
	public string Content { get; set; } = "";
}
