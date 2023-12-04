using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using TwitterClone.Data;
using TwitterClone.Models;

namespace TwitterClone.Pages
{
    public class ForgotPassword : PageModel
    {
        private readonly ILogger<ForgotPassword> _logger;
        private readonly UserManager<User> _userManager;
        private readonly TwitterCloneDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        public ForgotPassword(ILogger<ForgotPassword> logger,UserManager<User> userManager,TwitterCloneDbContext context,IHttpContextAccessor httpContextAccessor,IConfiguration configuration)
        {
            _logger = logger;
            _userManager=userManager;
            _context=context;
            _httpContextAccessor = httpContextAccessor;
            _configuration=configuration;
        }

        [Required]
        [BindProperty]
        public string Email { get; set; }
        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Email);
                if (user != null)
                {
                    // var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var resetToken = await _userManager.GenerateUserTokenAsync(user, TokenOptions.DefaultProvider, "ResetPassword");
                    var tokenResult = await _userManager.SetAuthenticationTokenAsync(user, "Default", "ResetPassword", resetToken);

                    var resetLink = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}/ResetPassword?token={WebUtility.UrlEncode(resetToken)}";

                    
                    var smtpUserName = _configuration["EmailService:SmtpUserName"];
                    var smtpPassword = _configuration["EmailService:SmtpPassword"];
                    var smtpHost = _configuration["EmailService:SmtpHost"];
                    var smtpPort = _configuration.GetValue<int>("EmailService:SmtpPort");
                    var enableSsl = _configuration.GetValue<bool>("EmailService:EnableSsl");

                    using(var smtp = new SmtpClient())
                    {
                        var credential = new NetworkCredential
                        {
                            UserName = smtpUserName,  
                            Password = smtpPassword 
                        };
                        smtp.Credentials = credential;
                        smtp.Host =smtpHost;
                        smtp.Port = smtpPort;
                        smtp.EnableSsl = enableSsl;
                        var message = new MailMessage();
                        message.To.Add(Email);
                        message.Subject = "Y - Reset Password";
                        message.Body = $"Dear {user.UserName} Click <a href=\"{resetLink}\">here</a> to reset your password.";
                        message.IsBodyHtml = true;
                        message.From = new MailAddress("jac2340575@gmail.com");
                        await smtp.SendMailAsync(message);
                    }
                    TempData["ResetEmailSentMessage"] = "Password reset email has been sent.";

                    return Page();
                }
            }
            
            ModelState.AddModelError(string.Empty, "Invalid email or user not found.");
            return Page();
        }

    }
}