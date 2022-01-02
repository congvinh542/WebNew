using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using WebNews.Helpers;
using WebNews.Models;

namespace WebNews.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize()]
    public class PostsController : Controller
    {
        private readonly WebNewsContext _context;

        public PostsController(WebNewsContext context)
        {
            _context = context;
        }

        // GET: Admin/Posts
        public IActionResult Index(int? page, int catID = 0)
        {
            // Kiểm tra quyền truy cập
            if (!User.Identity.IsAuthenticated) Response.Redirect("/dang-nhap.html");
            var taikhoangID = HttpContext.Session.GetString("AccountId");
            if (taikhoangID == null) return RedirectToAction("Login", "Accounts", new { Area = "Admin" });
            var account = _context.Accounts.AsNoTracking().FirstOrDefault(x => x.AccountId == int.Parse(taikhoangID));
            if (account == null) return NotFound();

            List<Post> lsPosts = new List<Post>();
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = Utilities.PAGE_SIZE;

            if(account.RoleId == 2) // Admin
            {
                lsPosts = _context.Posts
               .AsNoTracking()
               .Include(d => d.Account).Include(d => d.Cat)
               .OrderByDescending(x => x.CatId)
               .ToList();
            }
            else
            {
                lsPosts = _context.Posts
               .AsNoTracking()
               .Include(d => d.Account).Include(d => d.Cat)
               .Where(x => x.AccountId == account.AccountId)
               .OrderByDescending(x => x.CatId)
               .ToList();
            }

            if (catID != 0) // Admin
            {
                lsPosts = _context.Posts
                    .AsNoTracking().Where(x => x.CatId == catID)
                    .Include(x => x.Cat)
                    .OrderByDescending(x => x.PostId).ToList();
            }
            else
            {
                lsPosts = _context.Posts
                    .AsNoTracking()
                    .Include(x => x.Cat)
                    .OrderByDescending(x => x.PostId).ToList();
            }   

            PagedList<Post> models = new PagedList<Post>(lsPosts.AsQueryable(), pageNumber, pageSize);
            ViewBag.CurrentPage = pageNumber;
            ViewBag.CurrentCat = catID;
            ViewData["DanhMuc"] = new SelectList(_context.Categories, "CatId", "CatName");
            return View(models);
        }

        // GET: Admin/Filtter
        public IActionResult Filtter(int catID = 0)
        {
            var url = $"/Admin/Posts/Index?catID={catID}";
            if (catID == 0)
            {
                url = $"/Admin/Posts/Index";
            }
            else
            {
                url = $"/Admin/Posts/Index?catID={catID}";
            }
            return Json(new { status = "success", redirectUrl = url });
        }

        // GET: Admin/Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Account)
                .Include(p => p.Cat)
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // GET: Admin/Posts/Create
        public IActionResult Create()
        {
            if (!User.Identity.IsAuthenticated) Response.Redirect("/dang-nhap.html");
            var taikhoangID = HttpContext.Session.GetString("AccountId");
            if (taikhoangID == null) return RedirectToAction("Login", "Accounts", new { Area = "Admin" });
            ViewData["DanhMuc"] = new SelectList(_context.Categories, "CatId", "CatName");
            return View();
        }

        // POST: Admin/Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PostId,Title,ShortContent,Content,Thumb,Published,Alias,DateTime,Author,Tags,Link,CatId,IsHot,IsNewfeed,AccountId")] Post post, Microsoft.AspNetCore.Http.IFormFile fThumb)
        {
            // Kiểm tra quyền truy cập
            if (!User.Identity.IsAuthenticated) Response.Redirect("/dang-nhap.html");
            var taikhoangID = HttpContext.Session.GetString("AccountId");
            if (taikhoangID == null) return RedirectToAction("Login", "Accounts", new { Area = "Admin" });
            var account = _context.Accounts.AsNoTracking().FirstOrDefault(x => x.AccountId == int.Parse(taikhoangID));
            if (account == null) return NotFound();

            if (ModelState.IsValid)
            {
                post.AccountId = account.AccountId;
                post.Author = account.FullName;
                if (post.CatId == null) post.CatId = 1;
                post.DateTime = DateTime.Now;
                post.Alias = Utilities.SEOUrl(post.Title);
                post.Views = 0;
                if (fThumb != null)
                {
                    string extension = Path.GetExtension(fThumb.FileName);
                    string NewName = Utilities.SEOUrl(post.Title) + "preview_" + extension;
                    post.Thumb = await Utilities.UploadFile(fThumb, @"new\", NewName.ToLower());
                }
                _context.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AccountId"] = new SelectList(_context.Accounts, "AccountId", "AccountId", post.AccountId);
            ViewData["DanhMuc"] = new SelectList(_context.Categories, "CatId", "CatName", post.CatId);
            return View(post);
        }

        // GET: Admin/Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            ViewData["DanhMuc"] = new SelectList(_context.Categories, "CatId", "CatName", post.CatId);
            if (!User.Identity.IsAuthenticated) Response.Redirect("/dang-nhap.html");
            var taikhoangID = HttpContext.Session.GetString("AccountId");
            if (taikhoangID == null) return RedirectToAction("Login", "Accounts", new { Area = "Admin" });
            return View(post);
        }

        // POST: Admin/Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PostId,Title,ShortContent,Content,Thumb,Published,Alias,DateTime,Author,Tags,Link,CatId,IsHot,IsNewfeed,AccountId")] Post post, Microsoft.AspNetCore.Http.IFormFile fThumb)
        {
            if (id != post.PostId)
            {
                return NotFound();
            }
            if (!User.Identity.IsAuthenticated) Response.Redirect("/dang-nhap.html");
            var taikhoangID = HttpContext.Session.GetString("AccountId");
            if (taikhoangID == null) return RedirectToAction("Login", "Accounts", new { Area = "Admin" });
            var account = _context.Accounts.AsNoTracking().FirstOrDefault(x => x.AccountId == int.Parse(taikhoangID));
            if (account == null) return NotFound();

            if (account.RoleId != 1)
            {
                if (post.AccountId != account.AccountId) return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (fThumb != null)
                    {
                        string extension = Path.GetExtension(fThumb.FileName);
                        string NewName = Utilities.SEOUrl(post.Title) + "preview_" + extension;
                        post.Thumb = await Utilities.UploadFile(fThumb, @"new\", NewName.ToLower());
                    }
                    post.AccountId = account.AccountId;
                    post.Author = account.FullName;
                    post.Alias = Utilities.SEOUrl(post.Title);

                    _context.Update(post);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.PostId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["AccountId"] = new SelectList(_context.Accounts, "AccountId", "AccountId", post.AccountId);
            ViewData["DanhMuc"] = new SelectList(_context.Categories, "CatId", "CatName", post.CatId);
            return View(post);
        }

        // GET: Admin/Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Account)
                .Include(p => p.Cat)
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Admin/Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.PostId == id);
        }
    }
}
