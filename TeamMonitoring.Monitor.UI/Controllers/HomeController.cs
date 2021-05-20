using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using TeamMonitoring.Monitor.UI.Models;

namespace TeamMonitoring.Monitor.UI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HubOptions _hubOptions;

        public HomeController(ILogger<HomeController> logger, IOptions<HubOptions> hubOptions)
        {
            _logger = logger;
            _hubOptions = hubOptions.Value;
        }

        public IActionResult Index()
        {
            return View(new ProximityEventsViewModel { HubUrl = _hubOptions.HubUrl });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
