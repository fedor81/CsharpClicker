using CSharpClicker.Web.Domain;
using CsharpClicker.Web.UseCases.Captcha;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CSharpClicker.Web.UseCases.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Unit>
{
    private readonly UserManager<ApplicationUser> userManager;
    private readonly RecaptchaService captcha;

    public RegisterCommandHandler(UserManager<ApplicationUser> userManager, RecaptchaService captcha)
    {
        this.userManager = userManager;
        this.captcha = captcha;
    }

    public async Task<Unit> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        const string action = "SIGNUP";

        /*  Капча не работает  
        if (request.CaptchaToken == null)
        {
            throw new ValidationException("reCAPTCHA не пройдена.");
        }
        else
        {
            var isRecaptchaValid = await captcha.VerifyRecaptchaAsync(request.CaptchaToken, action);

            if (!isRecaptchaValid)
            {
                throw new ValidationException("reCAPTCHA failed to verify.");
            }
        }
        */

        if (request.UserName == null || request.Password == null)
        {
            throw new ValidationException("Username or password cannot be empty.");
        }

        const int minPassLength = 8;

        if (request.Password.Length < minPassLength)
        {
            throw new ValidationException($"Password must be at least {minPassLength} characters long");
        }

        if (userManager.Users.Any(u => u.UserName == request.UserName))
        {
            throw new ValidationException("Such user already exists.");
        }

        var user = new ApplicationUser
        {
            UserName = request.UserName,
        };

        var result = await userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var errorString = string.Join(Environment.NewLine, result.Errors);
            throw new ValidationException(errorString);
        }

        return Unit.Value;
    }
}
