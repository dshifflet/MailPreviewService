using System;

namespace MailPreview.HttpHost.Views
{
    public class SimpleMail
    {
        public string To { get; set; }
        public string From { get; set; }
        public string Subject { get; set; }
        public DateTime Received { get; set; }
        public int Attachments { get; set; }
        public string File { get; set; }
    }
}