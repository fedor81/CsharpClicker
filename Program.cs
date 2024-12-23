using CSharpClicker.Web.Infrastructure.Abstractions;
using CSharpClicker.Web.Infrastructure.DataAccess;
using CSharpClicker.Web.Infrastructure.Implementations;
using CSharpClicker.Web.Initializers;
using CsharpClicker.Web.UseCases.Captcha;
using CSharpClicker.Web.UseCases.Captcha;
using CSharpClicker.Web.UseCases.Email;
using CSharpClicker.Web.UseCases.Roulette;

namespace CSharpClicker.Web;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        ConfigureServices(builder.Services);

        var app = builder.Build();

        using var scope = app.Services.CreateScope();
        using var appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        DbContextInitializer.InitializeDbContext(appDbContext);

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseStaticFiles();
        app.UseSwagger();
        app.UseSwaggerUI();

        app.MapControllers();
        app.MapDefaultControllerRoute();
        app.MapHealthChecks("health-check");

        app.Run();
    }

    private static async void ConfigureServices(IServiceCollection services)
    {
        var recaptchaSettings = services.BuildServiceProvider()
            .GetRequiredService<IConfiguration>()
            .GetSection("Recaptcha")
            .Get<RecaptchaSettings>();

        services.AddSingleton<RecaptchaService>(provider =>
            new RecaptchaService(recaptchaSettings.ProjectId, recaptchaSettings.RecaptchaKey));

        var emailSettings = services.BuildServiceProvider()
            .GetRequiredService<IConfiguration>()
            .GetSection("EmailService")
            .Get<EmailServiceSettings>();

        services.AddSingleton<EmailService>(provider =>
            new EmailService(emailSettings.FromName, emailSettings.FromAddress, emailSettings.SmtpServer,
                emailSettings.SmtpPort, emailSettings.SmtpLogin, emailSettings.SmtpPassword));

        services.AddScoped<RouletteService>();

        services.AddHealthChecks();
        services.AddSwaggerGen();

        services.AddAutoMapper(typeof(Program).Assembly);
        services.AddMediatR(o => o.RegisterServicesFromAssembly(typeof(Program).Assembly));

        services.AddAuthentication()
            .AddCookie(o => o.LoginPath = "/auth/login");
        services.AddAuthorization();
        services.AddControllersWithViews();

        services.AddScoped<ICurrentUserAccessor, CurrentUserAccessor>();
        services.AddScoped<IAppDbContext, AppDbContext>();

        IdentityInitializer.AddIdentity(services);
        DbContextInitializer.AddAppDbContext(services);
    }
}
