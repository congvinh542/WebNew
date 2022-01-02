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
    public class NavMobiViewComponent : ViewComponent
    {
        private readonly WebNewsContext _context;
        private IMemoryCache _memoryCache;
        public NavMobiViewComponent(WebNewsContext context, IMemoryCache memoryCache)
        {
            _context = context;
            _memoryCache = memoryCache;
        }
        public IViewComponentResult Invoke()
        {
            var _danhmucs = _memoryCache.GetOrCreate(CacheKeys.Categories, entry =>
            {
                entry.SlidingExpiration = TimeSpan.MaxValue;
                return GetlsCategories();
            });
            return View(_danhmucs);
        }
        public List<Category> GetlsCategories()
        {
            List<Category> lstins = new List<Category>();
            lstins = _context.Categories
                .AsNoTracking()
                .Where(x => x.Published == true)
                .OrderBy(x => x.Odering)
                .ToList();
            return lstins;
        }
    }
}
