using MediatR;

namespace CSharpClicker.Web.UseCases.Login;

public record LoginCommand(string UserName, string Password, bool RememberMe) : IRequest<Unit>;
