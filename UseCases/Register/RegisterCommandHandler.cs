using CSharpClicker.Web.Domain;
using CsharpClicker.Web.UseCases.Captcha;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using CSharpClicker.Web.UseCases.Email;

namespace CSharpClicker.Web.UseCases.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Unit>
{
    private readonly UserManager<ApplicationUser> userManager;
    private readonly RecaptchaService captcha;
    private readonly EmailService emailService;

    public RegisterCommandHandler(UserManager<ApplicationUser> userManager, RecaptchaService captcha, EmailService emailService)
    {
        this.userManager = userManager;
        this.captcha = captcha;
        this.emailService = emailService;
    }

    public async Task<Unit> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        /*  Капча не работает  
        const string action = "SIGNUP";

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

        var user = await userManager.FindByNameAsync(request.UserName);

        if (user == null)
        {
            user = new ApplicationUser
            {
                UserName = request.UserName,
            };
        }
        else // if (user.EmailConfirmed)
            throw new ValidationException("Such user already exists.");

        var result = await userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var errorString = string.Join(Environment.NewLine, result.Errors);
            throw new ValidationException(errorString);
        }

        // Отправка письма для подтверждения регистрации на email
        //emailService.SendEmailAsync();

        return Unit.Value;
    }
}
