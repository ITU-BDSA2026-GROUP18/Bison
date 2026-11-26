using System.Security.Claims;
using Bison.Razor.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Bison.Razor.Pages;

public class UserTimelineModel : PageModel
{
	private readonly IObservationService _service;
	public List<ObservationViewModel> Observations { get; set; } = [];
	public List<CommentViewModel> Comments { get; set; } = [];
	public List<ProposalViewModel> Proposals { get; set; } = [];
	public string CurrentViewType { get; set; } = "observations";
	public int PageNr { get; set; }
	public int NrOfObservations { get; set; }
	public bool IsOwnTimeline { get; set; }
	public ObservationInput Input { get; set; } = new();

	public UserTimelineModel(IObservationService service)
	{
		_service = service;
	}

	public ActionResult OnGet(
		string author,
		[FromQuery] int page = 1,
		[FromQuery] string view = "observations"
	)
	{
		Load(author, page, view);
		return Page();
	}

	[RequestSizeLimit(PostRepository.MaxImageBytes + 64 * 1024)]
	public async Task<IActionResult> OnPostObservationAsync(string author, ObservationInput input)
	{
		var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
		if (userId is null)
			return Challenge();
		if (User.Identity?.Name != author)
			return Forbid();

		if (ModelState.IsValid)
		{
			try
			{
				await _service.AddObservationAsync(userId, input);
				return RedirectToPage(new { author });
			}
			catch (ArgumentException e)
			{
				ModelState.AddModelError(string.Empty, e.Message);
			}
		}

		Input = input;
		Load(author, 1, "observations");
		return Page();
	}

	private void Load(string author, int page, string requestedView)
	{
		if (page < 1)
			page = 1;
		PageNr = page;
		IsOwnTimeline = User.Identity?.IsAuthenticated == true && User.Identity.Name == author;

		CurrentViewType = IsOwnTimeline && (requestedView is "comments" or "proposals")
			? requestedView
			: "observations";
		Observations = CurrentViewType == "observations"
			? _service.GetObservationsFromAuthor(author, PageNr)
			: [];
		Comments = CurrentViewType == "comments"
			? _service.GetCommentsFromAuthor(author, PageNr)
			: [];
		Proposals = CurrentViewType == "proposals"
			? _service.GetProposalsFromAuthor(author, PageNr)
			: [];
		NrOfObservations = CurrentViewType switch
		{
			"comments" => Comments.Count,
			"proposals" => Proposals.Count,
			_ => Observations.Count,
		};
	}
}
