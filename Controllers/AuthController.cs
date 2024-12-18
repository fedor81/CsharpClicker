using CsharpClicker.Web.UseCases.Captcha;
using CSharpClicker.Web.UseCases.Login;
using CSharpClicker.Web.UseCases.Logout;
using CSharpClicker.Web.UseCases.Register;
using CSharpClicker.Web.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CSharpClicker.Web.Controllers;

[Route("auth")]
public class AuthController : Controller
{
    private readonly IMediator mediator;
    private readonly RecaptchaService captcha;

    public AuthController(IMediator mediator, RecaptchaService captcha)
    {
        this.mediator = mediator;
        this.captcha = captcha;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterCommand command)
    {
        try
        {
            await mediator.Send(command);
        }
        catch (ValidationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);

            var viewModel = new RegisterViewModel
            {
                UserName = command.UserName,
                Password = command.Password,
                CaptchaKey = captcha.recaptchaKey
            };

            return View(viewModel);
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpGet("register")]
    public IActionResult Register()
    {
        return View(new RegisterViewModel { CaptchaKey = captcha.recaptchaKey });
    }

    [HttpGet("confirm")]
    public async Task<IActionResult> ConfirmEmail(string userId, string code)
    {
        return View();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginCommand command)
    {
        try
        {
            await mediator.Send(command);
        }
        catch (ValidationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);

            var viewModel = new AuthViewModel
            {
                UserName = command.UserName,
                Password = command.Password,
            };

            return View(viewModel);
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpGet("login")]
    public IActionResult Login()
    {
        return View(new AuthViewModel());
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(LogoutCommand command)
    {
        await mediator.Send(command);

        return RedirectToAction("Login");
    }
}
