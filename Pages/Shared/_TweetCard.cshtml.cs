using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace TwitterClone.Pages.Shared
{
    public class _TweetCard : PageModel
    {
        private readonly ILogger<_TweetCard> _logger;

        public _TweetCard(ILogger<_TweetCard> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}