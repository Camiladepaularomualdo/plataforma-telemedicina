namespace ClinifyMobile.Helpers;

/// <summary>
/// DelegatingHandler que injeta automaticamente o token JWT
/// em todas as requisições HTTP, se disponível.
/// Registrado no DI e associado ao HttpClient nomeado.
/// </summary>
public class AuthDelegatingHandler : DelegatingHandler
{
    private readonly IServiceProvider _serviceProvider;

    public AuthDelegatingHandler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        // Obtém o token da sessão sem circular dependency
        // (SessionService é resolvido sob demanda)
        try
        {
            var session = _serviceProvider.GetService<Services.SessionService>();
            var token = session?.Token;
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }
        catch { /* Ignora se ainda não há sessão */ }

        return await base.SendAsync(request, cancellationToken);
    }
}
