using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using MailPreview.HttpHost.Views;
using Microsoft.AspNetCore.Mvc;

namespace MailPreview.HttpHost.Controllers
{
    [Route("api/[controller]")]
    [Route("api/v1/[controller]")]
    public class MailController : ControllerBase
    {

        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(MailController));

        [HttpGet]
        [Route("")]
        public IActionResult GetMail()
        {
            try
            {
                return Ok(GetSimpleMail());
            }
            catch (Exception ex)
            {
                HealthController.Errors++;
                Log.Error(ex);
                return StatusCode(500, ex.Message);
            }
            finally
            {
                HealthController.Activity++;
            }
        }

        [HttpGet]
        [Route("view.html")]        
        public IActionResult GetMailHtml()
        {
            try
            {
                var cb = new StringBuilder();
                cb.Append("<html><head></head><body>");
                cb.Append("<table><tr style='font-weight:bold'><td>Received</td><td>From</td><td>To</td><td>Subject</td><td>Attachments</td></tr>");
                foreach (var item in GetSimpleMail().OrderByDescending(o => o.Received))
                {
                    cb.Append(
                        $"<tr><td>{WrapUrl(item.Received.ToShortDateString(), item.File)} {WrapUrl(item.Received.ToShortTimeString(), item.File)}</td>" +
                        $"<td>{WrapUrl(item.From, item.File)}</td>" +
                        $"<td>{WrapUrl(item.To,item.File)}</td>" +
                        $"<td>{WrapUrl(item.Subject,item.File)}</td>" +
                        $"<td>{WrapUrl(item.Attachments,item.File)}</td></tr>");
                }
                cb.Append("</table></body></html>");
                return Content(cb.ToString(), "text/html");
            }
            catch (Exception ex)
            {
                HealthController.Errors++;
                Log.Error(ex);
                return StatusCode(500, ex.Message);
            }
            finally
            {
                HealthController.Activity++;
            }
        }

        [HttpGet]
        [Route("file/{name}")]
        public IActionResult GetFile(string name)
        {
            try
            {
                var file = new FileInfo($"{ConfigurationManager.MailPath}\\{WebUtility.UrlDecode(name)}");
                if (!file.Exists)
                {
                    return StatusCode(404);
                }
                using (var stream = file.OpenRead())
                using (var ms = new MemoryStream())
                {
                    stream.CopyTo(ms);
                    return File(ms.ToArray(), "message/rfc822", name);
                }
            }
            catch (Exception ex)
            {
                HealthController.Errors++;
                Log.Error(ex);
                return StatusCode(500, ex.Message);
            }
            finally
            {
                HealthController.Activity++;
            }
        }

        private string WrapUrl(object obj, string name)
        {
            return $"<a href='file/{WebUtility.UrlEncode(name)}'>{obj}</a>";
        }

        private IEnumerable<SimpleMail> GetSimpleMail()
        {
            var result = new List<SimpleMail>();
            var folder = new MailFolder(new DirectoryInfo(ConfigurationManager.MailPath));
            foreach (var item in folder.GetMailItems())
            {
                var mime = item.Value;
                result.Add(new SimpleMail
                {
                    To = mime.To.ToString(),
                    From = mime.From.ToString(),
                    File = item.Key.Name,
                    Subject = mime.Subject,
                    Attachments = mime.Attachments.Count(),
                    Received = mime.Date.DateTime
                });
            }
            return result;
        }
    }
}
