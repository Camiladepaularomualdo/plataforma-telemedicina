using ClinifyMobile.Models;

namespace ClinifyMobile.Helpers;

/// <summary>
/// Converte AppointmentStatus para labels e cores de UI.
/// </summary>
public static class StatusHelper
{
    public static string GetLabel(AppointmentStatus status) => status switch
    {
        AppointmentStatus.Agendado  => "Agendado",
        AppointmentStatus.EmEspera  => "Em Espera",
        AppointmentStatus.Atendido  => "Atendido",
        AppointmentStatus.Faltou    => "Faltou",
        AppointmentStatus.Cancelado => "Cancelado",
        _ => "Desconhecido"
    };

    public static Color GetColor(AppointmentStatus status) => status switch
    {
        AppointmentStatus.Agendado  => Color.FromArgb("#4A90D9"),
        AppointmentStatus.EmEspera  => Color.FromArgb("#F5A623"),
        AppointmentStatus.Atendido  => Color.FromArgb("#27AE60"),
        AppointmentStatus.Faltou    => Color.FromArgb("#E74C3C"),
        AppointmentStatus.Cancelado => Color.FromArgb("#95A5A6"),
        _ => Colors.Gray
    };

    public static Color GetLightColor(AppointmentStatus status) => status switch
    {
        AppointmentStatus.Agendado  => Color.FromArgb("#EBF4FF"),
        AppointmentStatus.EmEspera  => Color.FromArgb("#FFF8EC"),
        AppointmentStatus.Atendido  => Color.FromArgb("#EAFAF1"),
        AppointmentStatus.Faltou    => Color.FromArgb("#FDEDEC"),
        AppointmentStatus.Cancelado => Color.FromArgb("#F2F3F4"),
        _ => Colors.LightGray
    };

    /// <summary>Lista de status disponíveis para pickers.</summary>
    public static List<StatusOption> AllStatuses =>
    [
        new(AppointmentStatus.Agendado,  "Agendado"),
        new(AppointmentStatus.EmEspera,  "Em Espera"),
        new(AppointmentStatus.Atendido,  "Atendido"),
        new(AppointmentStatus.Faltou,    "Faltou"),
        new(AppointmentStatus.Cancelado, "Cancelado"),
    ];
}

public record StatusOption(AppointmentStatus Value, string Label)
{
    public override string ToString() => Label;
}
