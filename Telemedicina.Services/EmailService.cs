using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Telemedicina.Interfaces;

namespace Telemedicina.Services;

public class EmailService : IEmailService
{
    public async Task SendMeetingUrlEmailAsync(string patientEmail, string doctorName, string meetingUrl, string gmailAddress, string gmailAppPassword)
    {
        if (string.IsNullOrEmpty(patientEmail))
            throw new ArgumentException("E-mail do paciente não pode ser vazio.");

        var message = new MailMessage
        {
            From = new MailAddress(gmailAddress, doctorName),
            Subject = $"Sua consulta online com Dr(a). {doctorName}",
            Body = $@"
                <h2>Olá!</h2>
                <p>O Dr(a). {doctorName} iniciou a sua sala de atendimento online.</p>
                <br>
                <p>Clique no link abaixo para entrar na chamada de vídeo:</p>
                <p><a href='{meetingUrl}'>{meetingUrl}</a></p>
                <br>
                <p>Atenciosamente,<br>Equipe Clinfy-Atendimento-Online.</p>
                <br>
                <p style='color: #64748b; font-size: 0.85rem; border-top: 1px solid #e2e8f0; padding-top: 1rem; margin-top: 2rem;'>
                    clinfy
                </p>
            ",
            IsBodyHtml = true
        };
        message.To.Add(new MailAddress(patientEmail));

        var passwordToUse = gmailAppPassword?.Replace("\n", "").Replace("\r", "").Replace(" ", "");

        using var client = new SmtpClient("smtp.gmail.com", 587)
        {
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(gmailAddress, passwordToUse),
            EnableSsl = true,
            DeliveryMethod = SmtpDeliveryMethod.Network
        };

        await client.SendMailAsync(message);
    }
}
