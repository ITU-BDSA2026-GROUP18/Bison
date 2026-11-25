using System.Security.Claims;
using Bison.Razor.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Bison.Razor.Pages;

public class PublicModel : PageModel
{
	private readonly IObservationService _service;
	public List<ObservationViewModel> Observations { get; set; }
	public int PageNr { get; set; }
	public int NrOfObservations { get; set; }
	public ObservationInput Input { get; set; } = new();

	public PublicModel(IObservationService service)
	{
		_service = service;
	}

	public ActionResult OnGet([FromQuery] int page = 1, [FromQuery] string strTaxonId = "")
	{
		Load(page, strTaxonId);
		return Page();
	}

	[RequestSizeLimit(PostRepository.MaxImageBytes + 64 * 1024)]
	public async Task<IActionResult> OnPostObservationAsync(ObservationInput input)
	{
		var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
		if (userId is null)
			return Challenge();

		if (ModelState.IsValid)
		{
			try
			{
				await _service.AddObservationAsync(userId, input);
				return RedirectToPage();
			}
			catch (ArgumentException e)
			{
				ModelState.AddModelError(string.Empty, e.Message);
			}
		}

		Input = input;
		Load(1, "");
		return Page();
	}

	private void Load(int page, string strTaxonId)
	{
		if (page < 1)
			page = 1;
		PageNr = page;
		int taxonId = int.TryParse(strTaxonId, out int parsed) ? parsed : -1;
		Observations = _service.GetObservationsByTaxon(taxonId, PageNr);
		NrOfObservations = Observations.Count;
	}
}
