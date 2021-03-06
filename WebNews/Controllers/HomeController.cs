using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WebNews.Models;
using WebNews.ModelViews;

namespace WebNews.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly WebNewsContext _context;

        public HomeController(ILogger<HomeController> logger, WebNewsContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            HomeViewModel model = new HomeViewModel();

            var ls = _context.Posts.Include(x => x.Cat).AsNoTracking().ToList();
            model.LatestPosst = ls;
            model.Populars = ls;
            model.Recents = ls;
            model.Trendings = ls;
            model.Inspiration = ls;
            model.Featured = ls.FirstOrDefault();
            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }
        [Route("lien-he.html", Name ="Contact")]
        public IActionResult Contact()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
