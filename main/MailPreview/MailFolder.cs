using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using MimeKit;

namespace MailPreview
{
    public class MailFolder
    {
        private DirectoryInfo _path;

        public MailFolder(DirectoryInfo path)
        {
            _path = path ?? throw new ArgumentNullException(nameof(path));
        }

        public IDictionary<FileInfo, MimeMessage> GetMailItems()
        {
            var result = new Dictionary<FileInfo, MimeMessage>();
            _path.Refresh();
            foreach (var file in _path.GetFiles())
            {
                using (var stream = file.OpenRead())
                {
                    result.Add(file, MimeMessage.Load(stream));
                }                
            }
            return result;
        }
    }
}
