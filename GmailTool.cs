using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Text;

namespace GmailMCP
{
    [McpServerToolType]
    public class GmailTool(GmailService _gmail)
    {
        [McpServerTool, Description("Read latest email")]
        public async Task<string> ReadLatestEmail()
        {
            var request = _gmail.Users.Messages.List("me");
            request.MaxResults = 1; // Limit to the latest
            var response = await request.ExecuteAsync();
            var messageId = response.Messages[0].Id;
            var messageRequest = _gmail.Users.Messages.Get("me", messageId);
            var message = await messageRequest.ExecuteAsync();
            var subjectHeader = message.Payload.Headers.FirstOrDefault(h => h.Name == "Subject")?.Value ?? "No Subject";
            var fromHeader = message.Payload.Headers.FirstOrDefault(h => h.Name == "From")?.Value ?? "No Sender";
            var body = message.Payload.Parts?.FirstOrDefault(p => p.MimeType == "text/plain")?.Body?.Data ?? message.Payload.Body.Data;
            return $"From: {fromHeader}\nSubject: {subjectHeader}\n\n{DecodeBase64Url(body)}";
        }

        [McpServerTool, Description("Send an email using Gmail API")]
        public async Task<string> SendEmail(string to, string subject, string body)
        {
            var message = new Google.Apis.Gmail.v1.Data.Message
            {
                Raw = Convert.ToBase64String(Encoding.UTF8.GetBytes($"To: {to}\r\nSubject: {subject}\r\n\r\n{body}"))
            };
            try
            {
                var result = await _gmail.Users.Messages.Send(message, "me").ExecuteAsync();
                return $"Email sent successfully! Message ID: {result.Id}";
            }
            catch (Exception ex)
            {
                return $"Failed to send email: {ex.Message}";
            }
        }

        [McpServerTool, Description("Draft an email using Gmail API")]
        public async Task<string> DraftEmail(string to, string subject, string body)
        {
            var message = new Google.Apis.Gmail.v1.Data.Message
            {
                Raw = CreateRawEmail(to, subject, body)
            };

            var draft = new Draft { Message = message, Id = Guid.NewGuid().ToString() };

            try
            {
                var result = await _gmail.Users.Drafts.Create(draft, "me").ExecuteAsync();
                return $"Draft created successfully! Draft ID: {result.Id}";
            }
            catch (Exception ex)
            {
                return $"Failed to create draft: {ex.Message}";
            }
        }

        private static string CreateRawEmail(string to, string subject, string body)
        {
            var raw = $"To: {to}\r\nSubject: {subject}\r\nContent-Type: text/plain; charset=utf-8\r\n\r\n{body}";
            var bytes = System.Text.Encoding.UTF8.GetBytes(raw);
            return System.Convert.ToBase64String(bytes)
                .Replace('+', '-')
                .Replace('/', '_')
                .Replace("=", "");
        }

        private static string DecodeBase64Url(string base64Url)
        {
            base64Url = base64Url.Replace('-', '+').Replace('_', '/');
            switch (base64Url.Length % 4)
            {
                case 2: base64Url += "=="; break;
                case 3: base64Url += "="; break;
            }
            return System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(base64Url));
        }
    }
}
