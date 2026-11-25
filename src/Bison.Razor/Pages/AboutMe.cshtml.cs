using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Bison.Razor.Pages;

public class AboutMeModel : PageModel
{
	private readonly IObservationService _service;

	public string Email { get; private set; } = "";
	public string ViewType { get; private set; } = "observations";
	public List<ObservationViewModel> Observations { get; private set; } = [];
	public List<CommentViewModel> Comments { get; private set; } = [];
	public List<ProposalViewModel> Proposals { get; private set; } = [];

	public AboutMeModel(IObservationService service)
	{
		_service = service;
	}

	public IActionResult OnGet([FromQuery] string view = "observations", [FromQuery] int page = 1)
	{
		if (User.Identity?.IsAuthenticated != true)
			return Redirect("/obs");

		string username = User.Identity.Name ?? "";
		Email = User.FindFirstValue(ClaimTypes.Email) ?? "";
		ViewType = view is "comments" or "proposals" ? view : "observations";
		page = Math.Max(page, 1);

		Observations = ViewType == "observations" ? _service.GetObservationsFromAuthor(username, page) : [];
		Comments = ViewType == "comments" ? _service.GetCommentsFromAuthor(username, page) : [];
		Proposals = ViewType == "proposals" ? _service.GetProposalsFromAuthor(username, page) : [];
		return Page();
	}
}
