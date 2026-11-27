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

	// "latest" (all observations) or "following" (only from users the current user follows).
	public string Feed { get; set; } = "latest";
	public string TaxonFilter { get; set; } = "";
	public ObservationInput Input { get; set; } = new();

	public PublicModel(IObservationService service)
	{
		_service = service;
	}

	public ActionResult OnGet(
		[FromQuery] int page = 1,
		[FromQuery] string strTaxonId = "",
		[FromQuery] string feed = "latest"
	)
	{
		if (feed == "following" && User.FindFirstValue(ClaimTypes.NameIdentifier) is null)
			return Challenge();

		Load(page, strTaxonId, feed);
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
		Load(1, "", "latest");
		return Page();
	}

	private void Load(int page, string strTaxonId, string feed)
	{
		if (page < 1)
			page = 1;
		PageNr = page;
		TaxonFilter = strTaxonId ?? "";
		int taxonId = int.TryParse(strTaxonId, out int parsed) ? parsed : -1;
		var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

		Feed = feed == "following" && userId is not null ? "following" : "latest";
		Observations =
			Feed == "following"
				? _service.GetFollowingObservationsByTaxon(userId!, taxonId, PageNr)
				: _service.GetObservationsByTaxon(taxonId, PageNr, currentUserId: userId);
		NrOfObservations = Observations.Count;
	}
}
