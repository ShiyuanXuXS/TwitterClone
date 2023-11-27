using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using TwitterClone.Models;
using System.ComponentModel.DataAnnotations;
using TwitterClone.Utils;

namespace TwitterClone.Pages
{
    public class SignupModel : PageModel
    {

        private readonly UserManager<User> userManager;
        private readonly ILogger<SignupModel> logger;

        [BindProperty]
        public InputModel Input { get; set; }

        public SignupModel(UserManager<User> userManager,
            ILogger<SignupModel> logger)
        {
            this.userManager = userManager;
            this.logger = logger;
        }

        public class InputModel
        {
            [Required]
            [Display(Name = "Username")]
            public string UserName { get; set; }

            [Required]
            [Display(Name = "Name")]

            public string NickName { get; set; }

            [Required]
            [Display(Name = "Date of birth")]
            [DataType(DataType.Date)]
            [DOB]
            public DateTime DOB { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email address")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }


        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = new User { UserName = Input.UserName, Email = Input.Email, NickName = Input.NickName, DateOfBirth = Input.DOB, Avatar = "https://storagetwitterclonev2.blob.core.windows.net/web/default-avatar.png", EmailConfirmed = false };
                var result = await userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    var result2 = await userManager.AddToRoleAsync(user, "User");
                    if (result2.Succeeded)
                    {
                        logger.LogInformation($"User {Input.Email} create a new account with password");


                        return RedirectToPage("RegisterSuccess"); //TODO email verification


                    }
                    else
                    {
                        // FIXME: delete the user since role assignment failed, log the event, show error to the user
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return Page();
        }

    }
}
