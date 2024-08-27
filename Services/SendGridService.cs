using BlogPersonal.Extras;
using BlogPersonal.Interfaces;
using BlogPersonal.Models.View;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net.Sockets;
using System.Net;

namespace BlogPersonal.Services
{
    public class SendGridService : IEmailSendService
    {
        private readonly SendGridConfiguration _configuration;
        private readonly EmailAddress _myEmailAddress;
        public SendGridService(SendGridConfiguration sendGridConfiguration)
        {
            _configuration = sendGridConfiguration;
            _myEmailAddress = new EmailAddress(_configuration.AdminEmail, _configuration.SenderName);
        }
        public async Task<bool> SendEmailAsync(ContactViewModel model)
        {
            var client = new SendGridClient(_configuration.ApiKey);

            var emailFrom = new EmailAddress(_configuration.SenderEmail, _configuration.SenderName);

            var emailToUser = new EmailAddress(model.Email, model.Name);
            var thankYouMessage = MailHelper.CreateSingleEmail(
            emailFrom,
            emailToUser,
            "Gracias por contactarme",
            $"Hola {model.Name},\n\nGracias por comunicarte conmigo. He recibido tu mensaje y te responderé a la brevedad.\n\nSaludos,\n{_configuration.SenderName}",
            $"Hola {model.Name},<br><br>Gracias por comunicarte conmigo. He recibido tu mensaje y te responderé a la brevedad.<br><br>Saludos,<br>Abel"
        );

            var adminMessage = MailHelper.CreateSingleEmail(_myEmailAddress, emailFrom, $"Nuevo mensaje de {model.Name}",
                $"Has recibido un nuevo mensaje de {model.Name} ({model.Email}).\n\nAsunto: {model.Subject}\n\nMensaje:\n{model.Message}",
                $"Has recibido un nuevo mensaje de {model.Name} ({model.Email}).<br><br>Asunto: {model.Subject}<br><br>Mensaje:<br>{model.Message}");

            var responseUser = await client.SendEmailAsync(thankYouMessage);
            if(responseUser.IsSuccessStatusCode)
                return (await client.SendEmailAsync(adminMessage)).IsSuccessStatusCode;
            return responseUser.IsSuccessStatusCode;
        }

        public static bool VerifyDomain(string email)
        {
            string domain = email.Split('@').Last();
            try
            {
                IPHostEntry host = Dns.GetHostEntry(domain);
                Console.WriteLine(host.HostName);
                Console.WriteLine(host.AddressList.Length);
                return host.AddressList.Length > 0;
            }
            catch (SocketException)
            {
                return false;
            }
        }
    }
}
