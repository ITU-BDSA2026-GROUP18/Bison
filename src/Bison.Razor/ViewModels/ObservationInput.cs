namespace Bison.Razor.ViewModels;

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

public class ObservationInput
{
	[Required, StringLength(2000, MinimumLength = 1)]
	public string Text { get; set; } = "";

	[Required, Range(1, int.MaxValue, ErrorMessage = "Enter a valid taxon id.")]
	[Display(Name = "Taxon id")]
	public int? TaxonId { get; set; }

	public IFormFile? Image { get; set; }
}