using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Bison.Razor.Pages;

public class ObservationModel : PageModel
{
	private readonly IObservationService _service;
	public List<ObservationViewModel> Observations { get; set; }

	public ObservationModel(IObservationService service)
	{
		_service = service;
	}

	public ActionResult OnGet(string id)
  {
    if (string.IsNullOrEmpty(id))
    {
      Observations = _service.GetObservations(1);
      return Page();
    }

    if (int.TryParse(id, out int observationId))
    {
      var observation = _service.GetObservationById(observationId);
      Observations = new List<ObservationViewModel> { observation };
      return Page();
    }

    return NotFound();
  }
}
