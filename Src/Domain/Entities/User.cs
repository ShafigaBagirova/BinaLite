using System.Security.Principal;
using Microsoft.AspNetCore.Identity;
using Minio.DataModel.Notification;

namespace Domain.Entities;

public class User:IdentityUser
{
    public string FullName { get; set; }
}