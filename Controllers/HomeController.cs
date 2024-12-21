using CSharpClicker.Web.UseCases.AddPoints;
using CSharpClicker.Web.UseCases.Common;
using CSharpClicker.Web.UseCases.GetBoosts;
using CSharpClicker.Web.UseCases.GetCurrentUser;
using CSharpClicker.Web.UseCases.Roulette;
using CSharpClicker.Web.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CSharpClicker.Web.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly IMediator mediator;
    private readonly RouletteService rouletteService;

    public HomeController(IMediator mediator, RouletteService rouletteService)
    {
        this.mediator = mediator;
        this.rouletteService = rouletteService;
    }

    public async Task<IActionResult> Index()
    {
        var boosts = await mediator.Send(new GetBoostsQuery());
        var user = await mediator.Send(new GetCurrentUserQuery());

        var viewModel = new IndexViewModel()
        {
            Boosts = boosts,
            User = user,
        };

        return View(viewModel);
    } 

    [HttpPost("score")]
    public async Task<ScoreDto> AddToScore(AddPointsCommand command)
        => await mediator.Send(command);

    [HttpGet("roulette")]
    public async Task<IActionResult> Roulette()
    {
        var user = await mediator.Send(new GetCurrentUserQuery());

        var viewModel = new RouletteViewModel
        {
            User = user,
        };

        return View(viewModel);
    }

    [HttpGet("roulette/set")]
    public JsonResult RouletteSet()
    {
        var rouletteSet = rouletteService.GetRouletteSet().ToArray();
        return Json(rouletteSet);
    }

    [HttpPost("roulette")]
    public async Task<RouletteDto> Roulette(RollRouletteCommand command)
        => await mediator.Send(command);
}
