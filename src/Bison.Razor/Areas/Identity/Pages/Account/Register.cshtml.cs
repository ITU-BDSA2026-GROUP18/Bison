using System.ComponentModel.DataAnnotations;
using Bison.Razor.DBObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class RegisterModel : PageModel
{
	private readonly UserManager<User> _userManager;
	private readonly SignInManager<User> _signInManager;
	private readonly ILogger<RegisterModel> _logger;

	public RegisterModel(
		UserManager<User> userManager,
		SignInManager<User> signInManager,
		ILogger<RegisterModel> logger
	)
	{
		_userManager = userManager;
		_signInManager = signInManager;
		_logger = logger;
	}

	[BindProperty]
	public InputModel Input { get; set; } = new();

	public string? ReturnUrl { get; set; }

	public class InputModel
	{
		[Required]
		[StringLength(50, MinimumLength = 2)]
		[Display(Name = "Username")]
		public string UserName { get; set; } = string.Empty;

		[Required]
		[EmailAddress]
		[Display(Name = "Email")]
		public string Email { get; set; } = string.Empty;

		[Required]
		[StringLength(
			100,
			ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.",
			MinimumLength = 6
		)]
		[DataType(DataType.Password)]
		public string Password { get; set; } = string.Empty;

		[DataType(DataType.Password)]
		[Display(Name = "Confirm password")]
		[Compare(
			nameof(Password),
			ErrorMessage = "The password and confirmation password do not match."
		)]
		public string ConfirmPassword { get; set; } = string.Empty;
	}

	public void OnGet(string? returnUrl = null)
	{
		ReturnUrl = returnUrl;
	}

	public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
	{
		ReturnUrl = returnUrl;
		if (!ModelState.IsValid)
		{
			return Page();
		}

		var user = new User
		{
			UserName = Input.UserName,
			Name = Input.UserName,
			Email = Input.Email,
		};

		var result = await _userManager.CreateAsync(user, Input.Password);
		if (result.Succeeded)
		{
			_logger.LogInformation("A new user created an account with a password.");
			await _signInManager.SignInAsync(user, isPersistent: false);
			return LocalRedirect(ReturnUrl ?? "/");
		}

		foreach (var error in result.Errors)
		{
			ModelState.AddModelError(string.Empty, error.Description);
		}

		return Page();
	}
}
