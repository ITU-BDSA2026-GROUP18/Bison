namespace Bison.Razor.Pages;

using System.Security.Claims;
using Bison.Razor.DBObjects;
using Bison.Razor.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class ObservationModel : PageModel
{
	private readonly IObservationService _service;
	public List<ObservationViewModel> Observations { get; set; }
	public List<CommentViewModel> Comments { get; set; }
	public List<ProposalViewModel> Proposals { get; set; }
	public int PageNr { get; set; }
	public int NrOfObservations { get; set; }

	public ObservationModel(IObservationService service)
	{
		_service = service;
	}

	public async Task<ActionResult> OnGetAsync(string id, [FromQuery] int page = 1)
	{
		if (page < 1)
			page = 1;

		PageNr = page;

		if (string.IsNullOrEmpty(id))
		{
			Observations = _service.GetObservations(PageNr);
			NrOfObservations = Observations.Count;
			return Page();
		}

		if (int.TryParse(id, out int observationId))
		{
			// TODO: This is a waterfall of calls to the service, which is not ideal. We should refactor this to be more efficient.

			var observation = _service.GetObservationById(observationId);
			if (observation == null)
			{
				return NotFound();
			}

			Observations = [observation];
			NrOfObservations = 1;

			Comments = _service.GetCommentsForObservation(observationId, true);
			Proposals = _service.GetProposalsForObservation(observationId);

			return Page();
		}

		return NotFound();
	}

	public async Task<IActionResult> OnPostCommentAsync(string id, CommentInput input)
	{
		var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
		if (userId is null)
			return Challenge();

		if (!ModelState.IsValid)
			return await OnGetAsync(id);

		var AddedCommentId = _service.AddCommentToObservation(int.Parse(id), userId, input);

		// Redirect back and jump to the new comment
		return RedirectToPage(null, null, new { id }, $"comment-{AddedCommentId}");
	}
}
