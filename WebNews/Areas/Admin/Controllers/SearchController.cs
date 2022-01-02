using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebNews.Models;

namespace WebNews.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize("Admin")]
    public class SearchController : Controller
    {
        private readonly WebNewsContext _context;

        public SearchController(WebNewsContext context)
        {
            _context = context;
        }
        [HttpPost]
        public IActionResult FindBaiViet(string keyword)
        {
            if (keyword != null && keyword.Trim().Length > 3)
            {
                var ls = _context.Posts
                    .Include(x => x.Cat)
                    .AsNoTracking()
                .Where(x => x.Title.Contains(keyword) || x.Content.Contains(keyword))
                .OrderByDescending(x => x.DateTime)
                .ToList();
                return PartialView("ListBaiVietSearchPartial", ls);
            }
            else
            {
                return PartialView("ListBaiVietSearchPartial", null);
            }
        }
    }
}
