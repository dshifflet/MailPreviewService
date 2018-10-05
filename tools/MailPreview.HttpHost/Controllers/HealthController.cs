using Microsoft.AspNetCore.Mvc;

namespace MailPreview.HttpHost.Controllers
{
    [Route("[controller]")]
    [Route("api/[controller]")]
    [Route("api/v1/[controller]")]
    public class HealthController : ControllerBase
    {
        public static string Status = "Healthy";
        public static long Activity = 0;
        public static long Errors = 0;

        [HttpGet]
        [Route("")]
        public ContentResult GetHealth(string url)
        {
            return Content(
                string.Format("{0} ({1} Activity {2} Errors)", Status, Activity, Errors)
            );
        }
    }
}