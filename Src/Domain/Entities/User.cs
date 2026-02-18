using System.Security.Principal;
using Microsoft.AspNetCore.Identity;
using Minio.DataModel.Notification;

namespace Domain.Entities;

public class User:IdentityUser
{
    public string FullName { get; set; }
    public ICollection<RefreshToken> RefreshTokens { get; set; }
    public ICollection<PropertyAd> PropertyAds { get; set; }
}