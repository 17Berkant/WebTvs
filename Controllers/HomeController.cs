using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using WebTvs.Models;

namespace WebTvs.Controllers
{
    public class HomeController : Controller
    {
        private readonly ConcurrentBag<TvsCheck> _receivedDataList;

        public HomeController(ConcurrentBag<TvsCheck> receivedDataList)
        {
            _receivedDataList = receivedDataList;
        }

        public IActionResult Index()
        {
            return View(_receivedDataList);
        }
    }
}
