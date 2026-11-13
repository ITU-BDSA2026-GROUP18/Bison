using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Bison.Razor.DBObjects;

public class ExternalLoginModel : PageModel
{
	private readonly UserManager<User> _userManager;
	private readonly SignInManager<User> _signInManager;

	public ExternalLoginModel(UserManager<User> userManager, SignInManager<User> signInManager)
	{
		_userManager = userManager;
		_signInManager = signInManager;
	}

	public async Task<ChallengeResult> OnPost(string provider, string? returnUrl)
	{
		var redirectUrl = Url.Page("./ExternalLogin", "Callback", new { returnUrl });
		var props = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
		return Challenge(props, provider);
	}

	public async Task<IActionResult> OnGetCallbackAsync(string? returnUrl)
	{
		var info = await _signInManager.GetExternalLoginInfoAsync();
		if (info == null)
		{
			return RedirectToPage("./Login", new { returnUrl });
		}
		var res = await _signInManager.ExternalLoginSignInAsync(
			info.LoginProvider,
			info.ProviderKey,
			isPersistent: false
		);
		if (res.Succeeded) // existing user
		{
			return LocalRedirect(returnUrl ?? "/");
		}

		// new user
		var githubName = info.Principal.FindFirstValue(ClaimTypes.Name);
		var email = info.Principal.FindFirstValue(ClaimTypes.Email);
		if (githubName is null || email is null)
		{
			return RedirectToPage("./Login", new { returnUrl });
		}

		var user = new User
		{
			UserName = githubName,
			Name = githubName,
			Email = email,
		};
		var createRes = await _userManager.CreateAsync(user);
		if (!createRes.Succeeded)
		{
			return GetErrors(createRes);
		}
		var linkRes = await _userManager.AddLoginAsync(user, info);
		if (!linkRes.Succeeded)
		{
			await _userManager.DeleteAsync(user); // undo the add
			return GetErrors(linkRes);
		}
		await _signInManager.SignInAsync(user, isPersistent: false);
		return LocalRedirect(returnUrl ?? "/");
	}

	private IActionResult GetErrors(IdentityResult res)
	{
		foreach (var error in res.Errors)
		{
			ModelState.AddModelError(string.Empty, error.Description);
		}
		return Page();
	}
}
