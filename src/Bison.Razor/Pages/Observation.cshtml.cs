using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Bison.Razor.Pages;

public class ObservationModel : PageModel
{
	private readonly IObservationService _service;
	public List<ObservationViewModel> Observations { get; set; }
  public int PageNr { get; set; }
	public int NrOfObservations { get; set; }

	public ObservationModel(IObservationService service)
	{
		_service = service;
	}

	public ActionResult OnGet(string id, [FromQuery] int page = 1)
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
      var observation = _service.GetObservationById(observationId);
      if (observation == null)
      {
        return NotFound();
      }
      
      Observations = [observation];
      NrOfObservations = 1;
      return Page();
    }

    return NotFound();
  }
}
