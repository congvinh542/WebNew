using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using WebNews.Helpers;
using WebNews.Models;

namespace WebNews.Controllers
{
    public class PostsController : Controller
    {
        private readonly WebNewsContext _context;

        public PostsController(WebNewsContext context)
        {
            _context = context;
        }

        // GET: Posts
        // GET: List
        [Route("{Alias}", Name = "ListTin")]
        public IActionResult List(string Alias, int? page)
        {
            if (string.IsNullOrEmpty(Alias)) return RedirectToAction("Home", "Index");
            var danhmuc = _context.Categories.FirstOrDefault(x => x.Alias == Alias);
            if (danhmuc == null) return RedirectToAction("Home", "Index");

            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSzie = Utilities.PAGE_SIZE2;
            List<Post> lsPost = new List<Post>();
            List<Account> lsAccount = new List<Account>();
            if (!string.IsNullOrEmpty(Alias))
            {
                lsPost = _context.Posts.Include(x => x.Cat)
               .Include(x => x.Cat)
               .Where(x=>x.CatId == danhmuc.CatId)
               .AsNoTracking()
               .OrderByDescending(x => x.DateTime)
               .ToList();
            }
            else
            {
                lsPost = _context.Posts.Include(x => x.Cat)
              .Include(x => x.Cat)
              .AsNoTracking()
              .OrderByDescending(x => x.DateTime)
              .ToList();
            }

            PagedList<Post> models = new PagedList<Post>(lsPost.AsQueryable(), pageNumber, pageSzie);
            ViewBag.CurrentPage = pageNumber;

            ViewBag.DanhMuc = danhmuc;
            return View(models);
        }
            
        // GET: Posts/Details/5
        [Route("/{Alias}.html", Name = "PostDetails")]
        public async Task<IActionResult> Details(string Alias)
        {
            if (string.IsNullOrEmpty(Alias))
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Account)
                .Include(p => p.Cat)
                .FirstOrDefaultAsync(m => m.Alias == Alias);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }
        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.PostId == id);
        }
    }
}
