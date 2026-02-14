using Application.Options;
using Domain.Constants;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Persistence.Data;

public sealed class AdminSeeder
{
    private readonly UserManager<User> _userManager;
    private readonly SeedOptions _seedOptions;

    public AdminSeeder(UserManager<User> userManager, IOptions<SeedOptions> seedOptions)
    {
        _userManager = userManager;
        _seedOptions = seedOptions.Value;
    }

    public async Task SeedAsync()
    {
        var email = _seedOptions.AdminEmail;
        var password = _seedOptions.AdminPassword;
        var fullName = _seedOptions.AdminFullName;

        if (string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password) ||
            string.IsNullOrWhiteSpace(fullName))
            return;

        var existing = await _userManager.FindByEmailAsync(email);
        if (existing != null) return;

        var admin = new User
        {
            UserName = email, 
            Email = email,
            FullName = fullName,
            EmailConfirmed = true
        };

        var createResult = await _userManager.CreateAsync(admin, password);
        if (!createResult.Succeeded) return;

        await _userManager.AddToRoleAsync(admin, RoleNames.Admin);
    }
}
