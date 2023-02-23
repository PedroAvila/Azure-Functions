using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SendGrid.Helpers.Mail;
using System;
using System.Text;
using System.Text.RegularExpressions;

namespace pluralsightfuncs
{
    public class EmailLicenseFile
    {
        [FunctionName("EmailLicenseFile")]
        public void Run([BlobTrigger("licenses/{OrderId}.lic", 
            Connection = "AzureWebJobsStorage")]string licenceFileContents,
            [SendGrid(ApiKey = "SendGridApikey")] out SendGridMessage message,
            string name,
            ILogger log)
        {
            var email = Regex.Match(licenceFileContents,
                    @"^Email\:\ (.+)$", RegexOptions.Multiline).Groups[1].Value;
            log.LogInformation($"Got order from {email}\n License file Name:{name}");

            message = new SendGridMessage();
            message.From = new EmailAddress(Environment.GetEnvironmentVariable("EmailSender"));
            message.AddTo(email);
            var plainTextBytes = Encoding.UTF8.GetBytes(licenceFileContents);
            var base64 = Convert.ToBase64String(plainTextBytes);
            message.AddAttachment(name, base64, "txt/plain");
            message.Subject = "Your license file";
            message.HtmlContent = "Thank you for your order";



        }
    }
}
