using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebNews.Enums;
using WebNews.Models;

namespace WebNews.Controllers.Components
{
    public class PopularViewComponent : ViewComponent
    {
        private readonly WebNewsContext _context;
        private IMemoryCache _memoryCache;
        public PopularViewComponent(WebNewsContext context, IMemoryCache memoryCache)
        {
            _context = context;
            _memoryCache = memoryCache;
        }
        public IViewComponentResult Invoke()
        {
            var _tinseo = _memoryCache.GetOrCreate(CacheKeys.Popular, entry =>
            {
                entry.SlidingExpiration = TimeSpan.MaxValue;
                return GetlsCategories();
            });
            return View(_tinseo);
        }
        public List<Post> GetlsCategories()
        {
            List<Post> lstins = new List<Post>();
            lstins = _context.Posts
                .AsNoTracking()
                .Where(x => x.Published == true)
                .OrderByDescending(x => x.Views)
                .Take(6)
                .ToList();
            return lstins;
        }
    }
}
