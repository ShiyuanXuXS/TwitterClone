using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace TwitterClone.Pages.UserPortal
{
    public class AddTweet : PageModel
    {
        private readonly ILogger<AddTweet> _logger;

        public AddTweet(ILogger<AddTweet> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}