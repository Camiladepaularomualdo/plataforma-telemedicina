using System.Text.Json.Serialization;

namespace ClinifyMobile.Models;

public class Doctor
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("cpf")]
    public string Cpf { get; set; } = string.Empty;

    [JsonPropertyName("phone")]
    public string Phone { get; set; } = string.Empty;

    [JsonPropertyName("birthDate")]
    public DateTime BirthDate { get; set; }

    [JsonPropertyName("credits")]
    public int Credits { get; set; }

    [JsonPropertyName("rule")]
    public string Rule { get; set; } = "usr";

    [JsonPropertyName("isDeleted")]
    public bool IsDeleted { get; set; }
}
