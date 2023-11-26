using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TwitterClone.Data;
using TwitterClone.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace TwitterClone.Pages
{
    public class SearchResultModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        private readonly ILogger<SearchResultModel> logger;
        private readonly TwitterCloneDbContext context;
        private readonly UserManager<User> userManager;




        public void OnGet(string term)
        {
            SearchTerm = term;
        }
    }
}

