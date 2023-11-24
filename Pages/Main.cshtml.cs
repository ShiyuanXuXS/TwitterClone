using TwitterClone.Data;
using TwitterClone.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace TwitterClone.Pages;

[Authorize] //FIXME: Not working as expected, logged out user still accessing page
public class MainModel : PageModel
{
    private readonly ILogger<MainModel> _logger;
    private readonly TwitterCloneDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> signInManager;


    public MainModel(SignInManager<User> signInManager, ILogger<MainModel> logger, TwitterCloneDbContext context, UserManager<User> userManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
        this.signInManager = signInManager;

    }

   //ALSO NOT WORKING AS EXPECTED 
//Temporary solution
    // public async Task<IActionResult> OnGetAsync()
    // {
    //     if (!signInManager.IsSignedIn(User))
    //     {
    //         return RedirectToPage("Index");
    //     }
    //     else
    //     {
    //         return Page();
    //     }
    // }
}


