using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Bison.Razor.Pages;

public class LandingPageModel : PageModel
{
	public LandingPageModel() { }

	public ActionResult OnGet()
	{
		return LocalRedirect($"/obs");
	}
}
