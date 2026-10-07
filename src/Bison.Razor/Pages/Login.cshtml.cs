using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Bison.Razor.Pages;

public class LoginModel : PageModel
{
	private readonly IUserRepository _userRepository;
	private readonly IPasswordHasher<User> _passwordHasher;

	[BindProperty]
	[Required(ErrorMessage = "Please enter your username or email.")]
	[StringLength(256, ErrorMessage = "Input is too long.")]
	public string Identifier { get; set; } = "";

	[BindProperty]
	[Required(ErrorMessage = "Please enter your password.")]
	[StringLength(128, ErrorMessage = "Password is too long.")]
	[DataType(DataType.Password)]
	public string Password { get; set; } = "";

	[BindProperty(SupportsGet = true)]
	public string? ReturnUrl { get; set; }

	public string? ErrorMessage { get; set; }

	private static readonly User DummyUser = new()
	{
		UserId = 0,
		Username = "",
		Email = "",
	};
	private static readonly string DummyHash = new PasswordHasher<User>().HashPassword(
		DummyUser,
		"dummy-password-for-timing"
	);

	public LoginModel(IUserRepository userRepository, IPasswordHasher<User> passwordHasher)
	{
		_userRepository = userRepository;
		_passwordHasher = passwordHasher;
	}

	public IActionResult OnGet()
	{
		return (User.Identity?.IsAuthenticated == true) ? RedirectToPage("/Public") : Page();
	}

	public async Task<IActionResult> OnPostAsync()
	{
		if (!ModelState.IsValid)
			return Page();

		var user = _userRepository.GetByUsernameOrEmail(Identifier);
		if (user == null)
		{
			_passwordHasher.VerifyHashedPassword(DummyUser, DummyHash, Password);
			ErrorMessage = "Invalid username/email or password.";
			return Page();
		}

		if (!VerifyPassword(user, Password))
		{
			ErrorMessage = "Invalid username/email or password.";
			return Page();
		}

		await SignInUserAsync(user);

		if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
			return Redirect(ReturnUrl);

		return RedirectToPage("/Public");
	}

	private bool VerifyPassword(User user, string password)
	{
		var result = _passwordHasher.VerifyHashedPassword(user, user.PwHash, password);
		return result != PasswordVerificationResult.Failed;
	}

	private async Task SignInUserAsync(User user)
	{
		var claims = new List<Claim>
		{
			new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
			new(ClaimTypes.Name, user.Username),
			new(ClaimTypes.Email, user.Email),
		};

		var identity = new ClaimsIdentity(
			claims,
			CookieAuthenticationDefaults.AuthenticationScheme
		);
		var principal = new ClaimsPrincipal(identity);

		var authProperties = new AuthenticationProperties
		{
			IsPersistent = true,
			AllowRefresh = true,
			Items = { ["AbsoluteExpiresUtc"] = DateTimeOffset.UtcNow.AddDays(30).ToString("o") },
		};

		await HttpContext.SignInAsync(
			CookieAuthenticationDefaults.AuthenticationScheme,
			principal,
			authProperties
		);
	}
}
