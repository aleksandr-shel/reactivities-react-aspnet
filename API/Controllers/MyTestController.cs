using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;

namespace API.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class MyTestController : ControllerBase
    {
        private readonly ILogger<MyTestController> _logger;

        public MyTestController(ILogger<MyTestController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public IActionResult PostImage(IFormFile myFile)
        {
            _logger.LogInformation(myFile.FileName + " "
                + myFile.Length);
            using (var ms = new MemoryStream())
            {
                myFile.CopyTo(ms);
                var fileBytes = ms.ToArray();
                string s = Convert.ToBase64String(fileBytes);
                // act on the Base64 data
                //_logger.LogInformation(s);
            }
            return Ok();
        }
    }
}
