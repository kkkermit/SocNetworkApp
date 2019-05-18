using System.IO;
using Microsoft.AspNetCore.Mvc;

namespace SocNetworkApp.API.Controllers
{
    public class FallBack : ControllerBase
    {
        public IActionResult Index()
        {
            return PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(), 
                "wwwroot", "index.html"), "text/HTML");
        }
    }
}