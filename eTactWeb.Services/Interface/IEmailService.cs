using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTactWeb.Services.Interface
{
    public interface IEmailService
    {
        Task SendEmailAsync(
            string emailTo,
            string subject,
            string message,
            byte[] attachment = null,
            string attachmentName = "Report.pdf");
    }
}
