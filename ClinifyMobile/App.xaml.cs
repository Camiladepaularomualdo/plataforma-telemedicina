using ClinifyMobile.Services;

namespace ClinifyMobile;

public partial class App : Application
{
    public App(AppShell shell, SessionService session)
    {
        InitializeComponent();
        MainPage = shell;

        // Restaura a sessão do SecureStorage de forma async segura.
        // Isso garante que DoctorId está em memória antes da LoginPage verificar IsLoggedIn.
        Dispatcher.Dispatch(async () =>
        {
            await session.LoadFromStorageAsync();
        });
    }
}
