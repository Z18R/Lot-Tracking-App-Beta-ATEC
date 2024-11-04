using LotTrackingApp.Models;
using LotTrackingApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging; 
using System.Diagnostics;

namespace LotTrackingApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly LotTrackingService _lotTrackingService;


        public HomeController(ILogger<HomeController> logger, LotTrackingService lotTrackingService)
        {
            _logger = logger;
            _lotTrackingService = lotTrackingService;

        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public IActionResult GetLotDetails(string lotAlias)
        {
            var lotDetails = _lotTrackingService.GetLotDetails(lotAlias);
            return PartialView("_LotDetailsPartial", lotDetails);
        }
    }
}
