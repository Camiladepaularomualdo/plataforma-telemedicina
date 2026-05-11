using ClinifyMobile.Helpers;
using ClinifyMobile.Services;
using ClinifyMobile.ViewModels;
using ClinifyMobile.Views;
using Microsoft.Extensions.Logging;

namespace ClinifyMobile;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // ─── HTTP Client com DelegatingHandler ──────────────────────────────
        builder.Services.AddTransient<AuthDelegatingHandler>();

        builder.Services.AddHttpClient<ApiService>(client =>
        {
            client.BaseAddress = new Uri(AppSettings.ApiBaseUrl + "/");
            client.Timeout     = TimeSpan.FromSeconds(AppSettings.HttpTimeoutSeconds);
        })
        .AddHttpMessageHandler<AuthDelegatingHandler>()
        .ConfigurePrimaryHttpMessageHandler(() =>
        {
            // ATENÇÃO: Em produção remova DangerousAcceptAnyServerCertificateValidator
            // Necessário apenas para certificado self-signed em desenvolvimento
            return new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };
        });

        // ─── Services (Singleton = vive todo o app) ──────────────────────────
        builder.Services.AddSingleton<SessionService>();
        builder.Services.AddSingleton<AuthService>();
        builder.Services.AddSingleton<AppointmentService>();
        builder.Services.AddSingleton<PatientService>();

        // ─── ViewModels (Transient = nova instância por página) ─────────────
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<AgendaViewModel>();
        builder.Services.AddTransient<AppointmentsListViewModel>();
        builder.Services.AddTransient<AddPatientViewModel>();
        builder.Services.AddTransient<AddAppointmentViewModel>();

        // ─── Views ──────────────────────────────────────────────────────────
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<AgendaPage>();
        builder.Services.AddTransient<AppointmentsListPage>();
        builder.Services.AddTransient<AddPatientPage>();
        builder.Services.AddTransient<AddAppointmentPage>();

        // Shell e App precisam ser Singleton
        builder.Services.AddSingleton<AppShell>();
        builder.Services.AddSingleton<App>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
