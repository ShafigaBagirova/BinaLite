using Application.Abstracts.Repositories;
using Application.Abstracts.Services;
using Application.Options;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Net;

namespace Persistence.Services;

public class PropertyModerationService : IPropertyModerationService
{
    private readonly IPropertyAdRepository _repo;
    private readonly IEmailService _email;
    private readonly EmailOptions _emailOptions;
    private readonly UserManager<User> _userManager;

    public PropertyModerationService(IPropertyAdRepository repo,IEmailService email, 
        IOptions<EmailOptions> emailOptions,UserManager<User> userManager)
    {
        _repo = repo;
        _email = email;
        _emailOptions = emailOptions.Value;
        _userManager = userManager;

    }

    public async Task ApproveAsync(int propertyId, CancellationToken ct)
    {
        var p = await _repo.GetByIdWithOwnerAsync(propertyId, ct);
        if (p is null) throw new InvalidOperationException("Property not found.");

        p.Status = PropertyStatus.Approved;
        p.RejectionReason = null;

        await _repo.SaveChangesAsync(ct);

        if (!_emailOptions.EnableEmailSending) return;

        var link = $"{_emailOptions.PropertyBaseUrl.TrimEnd('/')}/{p.Id}";
        var subject = "Your listing has been approved";
        var html =
            $"""
            <p>Your listing <b>{WebUtility.HtmlEncode(p.Title)}</b> has been approved ✅</p>
            <p>Open it here: <a href="{link}">View listing</a></p>
            """;
        if (p.User is null)
            throw new InvalidOperationException("User navigation is NULL. Repo Include(User) etmir.");

        if (string.IsNullOrWhiteSpace(p.User.Email))
            throw new InvalidOperationException("User.Email is empty.");

        await _email.SendAsync(p.User.Email, subject, html, cancellationToken: ct);
    }

    public async Task RejectAsync(int propertyId, string reason, CancellationToken ct)
    {
        var p = await _repo.GetByIdWithOwnerAsync(propertyId, ct);
        if (p is null) throw new InvalidOperationException("Property not found.");

        p.Status = PropertyStatus.Rejected;
        p.RejectionReason = reason;

        await _repo.SaveChangesAsync(ct);

        if (!_emailOptions.EnableEmailSending) return;

        var subject = "Your listing was rejected";
        var html =
            $"""
            <p>Your listing <b>{WebUtility.HtmlEncode(p.Title)}</b> was rejected ❌</p>
            <p>Reason: {WebUtility.HtmlEncode(reason)}</p>
            """;

        await _email.SendAsync(p.User.Email, subject, html, cancellationToken: ct);
    }
}
