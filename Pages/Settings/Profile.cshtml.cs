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
    public class ProfileModel : PageModel
    {
        private readonly UserManager<User> userManager;
        private readonly ILogger<ProfileModel> logger;
        private readonly BlobServiceClient blobClient;

        public User ModelUser { get; set; }
        public DateOnly UserDOB { get; set; }

        public ProfileModel(UserManager<User> userManager, ILogger<ProfileModel> logger, BlobServiceClient blobClient)
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
            [Display(Name = "Name")]
            public string NickName { get; set; }

            [Display(Name = "Bio")]
            [Required(AllowEmptyStrings = true)]
            [DisplayFormat(ConvertEmptyStringToNull = false)]
            //[StringLength(160, ErrorMessage = "Bio must not exceed 160 characters.", MinimumLength = 0)]

            public string? Description { get; set; }

            [Display(Name = "Date of birth")]
            [DataType(DataType.Date)]
            [DOB]
            public DateTime? DOB { get; set; }

            [BindProperty]
            public IFormFile? Avatar { get; set; }
        }

        public async Task OnGetAsync()
        {
            ModelUser = await userManager.GetUserAsync(HttpContext.User);
            if (ModelUser != null)
            {
                DateTime dob = ModelUser.DateOfBirth.Value;
                UserDOB = DateOnly.FromDateTime(dob);


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

                if (Input.Avatar != null)
                {

                    string[] validExt = { ".jpg", ".jpeg", ".png" };
                    if (validExt.Contains(Path.GetExtension(Input.Avatar.FileName).ToLower()))
                    {
                        var containerClient = blobClient.GetBlobContainerClient(ModelUser.UserName);

                        await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

                        string blob = $"{Guid.NewGuid().ToString()}{Path.GetExtension(Input.Avatar.FileName)}";

                        await containerClient.UploadBlobAsync(
                           blob, Input.Avatar.OpenReadStream()
                       );
                        ModelUser.Avatar = $"{containerClient.Uri.AbsoluteUri}/{blob}"; 
                        //FIXME handle fail
                    }

                }



                ModelUser.NickName = Input.NickName;

                if (Input.Description != null)
                {
                    ModelUser.Description = Input.Description;
                }
                ModelUser.DateOfBirth = Input.DOB;

                IdentityResult result = await userManager.UpdateAsync(ModelUser);

                if (result.Succeeded)
                {
                    return RedirectToPage("../User", new { username = ModelUser.UserName}); //TODO flash msg
                }
                else
                {
                    return RedirectToPage("../NotFound");
                }
            }

            return Page();
        }

        // private async Task LoadFiles(InputFileChangeEventArgs e)
        // {

        // }

    }
}
