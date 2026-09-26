using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Bison.Razor.Pages;

public class PublicModel : PageModel
{
	private readonly IObservationService _service;
	public List<ObservationViewModel> Observations { get; set; }
	public int PageNr { get; set; }
	public int NrOfObservations { get; set; }

	public PublicModel(IObservationService service)
	{
		_service = service;
	}

	public ActionResult OnGet([FromQuery] int page = 1)
	{
		if (page < 1) page = 1;
		PageNr = page;
		Observations = _service.GetObservations(PageNr);
		NrOfObservations = Observations.Count;
		return Page();
	}
}
