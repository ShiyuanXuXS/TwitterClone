using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using TwitterClone.Models;
using Microsoft.AspNetCore.Identity;


namespace TwitterClone.Pages.Shared.Components.AccountMenu
{
    public class AccountMenuViewComponent : ViewComponent
    {
        private readonly UserManager<User> userManager;

        public AccountMenuViewComponent(UserManager<User> userManager) {
            this.userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            User? currentUser = await userManager.GetUserAsync(HttpContext.User);
            return View(currentUser);
        }
    }
}
