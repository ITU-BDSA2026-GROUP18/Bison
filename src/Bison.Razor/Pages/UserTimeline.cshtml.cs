using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Bison.Razor.Pages;

public class UserTimelineModel : PageModel
{
	private readonly IObservationService _service;
	public List<ObservationViewModel> Observations { get; set; }
	public int PageNr { get; set; }
	public int NrOfObservations { get; set; }
	public string Author { get; set; } = "";

	public UserTimelineModel(IObservationService service)
	{
		_service = service;
	}

	public ActionResult OnGet(string author, [FromQuery] int page = 1)
	{
		if (page < 1)
			page = 1;
		Author = author ?? "";
		PageNr = page;
		Observations = _service.GetObservationsFromAuthor(Author, PageNr);
		NrOfObservations = Observations.Count;
		return Page();
	}
}
