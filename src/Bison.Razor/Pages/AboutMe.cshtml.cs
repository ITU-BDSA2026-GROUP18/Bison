using System.Security.Claims;
using Bison.Razor.DBObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Bison.Razor.Pages;

public class AboutMeModel : PageModel
{
	private readonly IObservationService _service;

	public string Email { get; set; } = "";
	public string ViewType { get; set; } = "observations";
	public List<ObservationViewModel> Observations { get; set; } = [];
	public List<CommentViewModel> Comments { get; set; } = [];
	public List<ProposalViewModel> Proposals { get; set; } = [];
	private readonly UserManager<User> _userManager;
	private readonly SignInManager<User> _signInManager;

	[BindProperty]
	public string ForgetMeConfirmation { get; set; } = "";

	[TempData]
	public string? ForgetMeError { get; set; }

	public AboutMeModel(
    IObservationService service,
    UserManager<User> userManager,
    SignInManager<User> signInManager)
{
    _service = service;
    _userManager = userManager;
    _signInManager = signInManager;
}

	public IActionResult OnGet([FromQuery] string view = "observations", [FromQuery] int page = 1)
	{
		if (User.Identity?.IsAuthenticated != true)
			return Redirect("/obs");

		string username = User.Identity.Name ?? "";
		Email = User.FindFirstValue(ClaimTypes.Email) ?? "";
		ViewType = view is "comments" or "proposals" ? view : "observations";
		page = Math.Max(page, 1);

		Observations =
			ViewType == "observations" ? _service.GetObservationsFromAuthor(username, page) : [];
		Comments = ViewType == "comments" ? _service.GetCommentsFromAuthor(username, page) : [];
		Proposals = ViewType == "proposals" ? _service.GetProposalsFromAuthor(username, page) : [];
		return Page();
	}

	public async Task<IActionResult> OnPostForgetMeAsync()
	{
		var user = await _userManager.GetUserAsync(User);
		if (user is null)
			return Challenge();

		await _service.ReassignPostsFromUserId(user.Id);

		var result = await _userManager.DeleteAsync(user);
		if (!result.Succeeded)
		{
			foreach (var error in result.Errors)
				ModelState.AddModelError(string.Empty, error.Description);

			return Page();
		}

		await _signInManager.SignOutAsync();
		return Redirect("/obs");
	}
}
