using Application.Dtos.AuthDtos;
using Domain.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace Application.Validations.AuthValidation;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    private readonly UserManager<User> _userManager;

    public RegisterRequestValidator(UserManager<User> userManager)
    {
        _userManager = userManager;

    }
    public RegisterRequestValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("UserName boş ola bilməz")
            .MinimumLength(3).WithMessage("UserName minimum 3 simvol olmalıdır")
            .MaximumLength(50).WithMessage("UserName maksimum 50 simvol ola bilər")
            .MustAsync(async (userName, ct) =>
             await _userManager.FindByNameAsync(userName) == null)
            .WithMessage("Bu UserName artıq istifadə olunur");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email boş ola bilməz")
            .EmailAddress().WithMessage("Email formatı yanlışdır")
            .MaximumLength(256).WithMessage("Email maksimum 256 simvol ola bilər")
             .MustAsync(async (email, ct) =>
             await _userManager.FindByEmailAsync(email) == null )
              .WithMessage("Bu Email artıq istifadə olunur");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password boş ola bilməz")
            .MinimumLength(8).WithMessage("Password minimum 8 simvol olmalıdır")
            .MaximumLength(100).WithMessage("Password maksimum 100 simvol ola bilər")
            .Matches("[0-9]").WithMessage("Password ən az 1 rəqəm içərməlidir")
            .Matches("[A-Z]").WithMessage("Password ən az 1 böyük hərf içərməlidir")
            .Matches("[a-z]").WithMessage("Password ən az 1 kiçik hərf içərməlidir")
            .Matches("[^a-zA-Z0-9]").WithMessage("Password ən az 1 xüsusi simvol içərməlidir");

        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("FullName boş ola bilməz")
            .MaximumLength(200).WithMessage("FullName maksimum 200 simvol ola bilər");
    }
}

