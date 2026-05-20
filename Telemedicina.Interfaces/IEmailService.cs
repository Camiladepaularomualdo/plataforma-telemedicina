using System.Threading.Tasks;

namespace Telemedicina.Interfaces;

public interface IEmailService
{
    Task SendMeetingUrlEmailAsync(string patientEmail, string doctorName, string meetingUrl, string gmailAddress, string gmailAppPassword);
    Task SendTemporaryPasswordEmailAsync(string patientEmail, string doctorName, string tempPassword, string gmailAddress, string gmailAppPassword);
}
