﻿using TwitterClone.Data;
using TwitterClone.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace TwitterClone.Pages;

public class MainModel : PageModel
{
    private readonly ILogger<MainModel> _logger;
    private readonly TwitterCloneDbContext _context;
    private readonly UserManager<User> _userManager;

    public List<User> Users { get; set; } = null!;


    public MainModel(ILogger<MainModel> logger, TwitterCloneDbContext context, UserManager<User> userManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
    }


    public async Task<IActionResult> OnGetAsync()
    {
        // Get all users using UserManager
        Users = await _userManager.Users.ToListAsync();
        return Page();
    }
}

