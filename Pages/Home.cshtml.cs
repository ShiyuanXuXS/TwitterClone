using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using TwitterClone.Data;
using TwitterClone.Models;
using Microsoft.AspNetCore.Identity;


namespace TwitterClone.Pages
{
    [Authorize]
    public class HomeModel : PageModel
    {

    private readonly ILogger<HomeModel> logger;
    private readonly TwitterCloneDbContext context;
    private readonly UserManager<User> userManager;
    private readonly SignInManager<User> signInManager;


    public HomeModel(SignInManager<User> signInManager, ILogger<HomeModel> logger, TwitterCloneDbContext context, UserManager<User> userManager)
    {
        this.logger = logger;
        this.context = context;
        this.userManager = userManager;
        this.signInManager = signInManager;
    }
        public void OnGet()
        {
        }
    }
}

