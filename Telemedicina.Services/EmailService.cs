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
                <p>Atenciosamente,<br>Equipe Telemedicina.</p>
            ",
            IsBodyHtml = true
        };
        message.To.Add(new MailAddress(patientEmail));

        using var client = new SmtpClient("smtp.gmail.com", 587)
        {
            Credentials = new NetworkCredential(gmailAddress, gmailAppPassword),
            EnableSsl = true
        };

        await client.SendMailAsync(message);
    }
}
