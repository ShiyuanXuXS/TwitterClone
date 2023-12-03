using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using TwitterClone.Models;

namespace TwitterClone.Pages
{
    public class ResetPassword : PageModel
    {
        private readonly ILogger<ResetPassword> _logger;
        private readonly UserManager<User> _userManager;

        public ResetPassword(ILogger<ResetPassword> logger,UserManager<User> userManager)
        {
            _logger = logger;
            _userManager=userManager;
        }
        [BindProperty]
        public string Token { get; set; }
        [Required]
        [BindProperty]
        public string Email { get; set; }

        [Required]
        [BindProperty]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        [BindProperty]
        public string ConfirmPassword { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(string token)
        {
            if (token == null)
            {
                return BadRequest("A token is required for password reset.");
            }

            Token = token;
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            // if (!ModelState.IsValid)
            // {
            //     return Page();
            // }

            var user = await _userManager.FindByEmailAsync(Email);
            if (user == null)
            {
                return BadRequest("User not found.");
            }

            var result = await _userManager.ResetPasswordAsync(user, Token, Password);
            if (result.Succeeded)
            {
                // StatusMessage = "Password has been reset successfully, you can log in with your new password now.";
                TempData["ResetPasswordMessage"] = "Password has been reset successfully, you can log in with your new password now.";
                return Page();
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }
        }
    }
}