using BlogPersonal.Models.View;

namespace BlogPersonal.Interfaces
{
    public interface IEmailSendService
    {
        public Task<bool> SendEmailAsync(ContactViewModel model);
    }
}
