﻿using MediatR;

namespace CSharpClicker.Web.UseCases.Register;

public record RegisterCommand(string UserName, string Password, string CaptchaToken) : IRequest<Unit>;
