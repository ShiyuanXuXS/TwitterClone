using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TwitterClone.Models;

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
namespace TwitterClone.Pages

{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<User> signInManager;
        private readonly ILogger<LoginModel> logger;

        public LoginModel(SignInManager<User> signInManager, ILogger<LoginModel> logger)
        {
            this.signInManager = signInManager;
            this.logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            public string UserName { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {

                var result = await signInManager.PasswordSignInAsync(Input.UserName, Input.Password, false, true);
                if (result.Succeeded)
                {
                    logger.LogInformation($"User {Input.UserName} logged in");
                    return RedirectToPage("/Index"); //TODO redirect user to home or attemped page
                }
                else
                { // user does not exist, password invalid, account locked out
                    ModelState.AddModelError(string.Empty, "Login failed");
                }

            }
            return Page();
        }

    }
}
