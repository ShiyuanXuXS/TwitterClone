using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TwitterClone.Data;
using TwitterClone.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;

namespace MidtermTodos.Pages
{
    public class _PartialFollowSuggestionModel : PageModel
    {

        // private readonly TwitterCloneDbContext _context;
        // private readonly UserManager<User> _userManager;

        // public _PartialFollowSuggestionModel(TwitterCloneDbContext context, UserManager<User> userManager)
        // {
        //     _context = context;
        //     _userManager = userManager;
        // }

        public void OnGet()
        {
        }
    }
}
