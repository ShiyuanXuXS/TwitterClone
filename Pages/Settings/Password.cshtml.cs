using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using TwitterClone.Models;
using TwitterClone.Utils;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;



namespace TwitterClone.Pages.Settings
{
    [Authorize]
    public class PasswordModel : PageModel
    {
        private readonly UserManager<User> userManager;
        private readonly ILogger<ProfileModel> logger;
        private readonly BlobServiceClient blobClient;

        public User ModelUser { get; set; }
        public DateOnly UserDOB { get; set; }

        public PasswordModel(UserManager<User> userManager, ILogger<ProfileModel> logger, BlobServiceClient blobClient)
        {
            this.userManager = userManager;
            this.logger = logger;
            this.blobClient = blobClient;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]  
            [DataType(DataType.Password)]
            [Display(Name = "Current password")]
            public string? Password { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [Display(Name = "New password")]
            public string? NewPassword { get; set; }

        }

        public async Task OnGetAsync()
        {
            ModelUser = await userManager.GetUserAsync(HttpContext.User);
            if (ModelUser != null)
            {
                

            }
            else
            {
                RedirectToPage("../NotFound");
            }

        }

        public async Task<IActionResult> OnPostAsync()
        {
            ModelUser = await userManager.GetUserAsync(HttpContext.User);

            if (ModelState.IsValid & ModelUser != null)
            {
               
                IdentityResult result = await userManager.ChangePasswordAsync(ModelUser, Input.Password, Input.NewPassword);

                if (result.Succeeded)
                {
                    return RedirectToPage("../User", new { username = ModelUser.UserName}); //TODO flash msg
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return Page();
        }

        // private async Task LoadFiles(InputFileChangeEventArgs e)
        // {

        // }

    }
}
