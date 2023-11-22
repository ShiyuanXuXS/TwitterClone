using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TwitterClone.Pages;

public class MainModel : PageModel
{
    private readonly ILogger<MainModel> _logger;

    public MainModel(ILogger<MainModel> logger)
    {
        _logger = logger;
    }


    public void OnGet()
    {
    }
}

