using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using TwitterClone.Models;
using TwitterClone.Utils;
using TwitterClone.Data;



namespace TwitterClone.Pages.Settings
{
    [Authorize]
    public class ProfileModel : PageModel
    {
        private readonly UserManager<User> userManager;
        private readonly ILogger<ProfileModel> logger;
        private readonly TwitterCloneDbContext db;

        public User ModelUser { get; set; }
        public DateOnly UserDOB { get; set; }

        public ProfileModel(UserManager<User> userManager, ILogger<ProfileModel> logger)
        {
            this.userManager = userManager;
            this.logger = logger;
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

                ModelUser.NickName = Input.NickName;

                if (Input.Description != null)
                {
                    ModelUser.Description = Input.Description;
                }
                ModelUser.DateOfBirth = Input.DOB;

                IdentityResult result = await userManager.UpdateAsync(ModelUser);

                if (result.Succeeded)
                {
                    return RedirectToPage("../Home"); //TODO back to profile + flash msg
                }
                else
                {
                    return RedirectToPage("../NotFound"); //FIXME
                }
            }

            return Page();
        }

    }
}
