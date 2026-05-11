namespace ClinifyMobile.Helpers;

public static class AppSettings
{
    /// <summary>
    /// Base URL da API.
    /// - Emulador Android: use http://10.0.2.2:7199/api
    /// - Dispositivo físico: use o IP do servidor na rede local, ex: http://192.168.1.100:7199/api
    /// - Produção: substitua pela URL real
    /// </summary>
    public static string ApiBaseUrl { get; set; } = "https://app.clinfy.com.br/apiv1/api";

    // Timeouts
    public static int HttpTimeoutSeconds { get; set; } = 30;
}
