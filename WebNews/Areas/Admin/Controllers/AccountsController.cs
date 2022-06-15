using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using WebNews.Areas.Admin.Models;
using WebNews.Extension;
using WebNews.Helpers;
using WebNews.Models;

namespace WebNews.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AccountsController : Controller
    {
        private readonly WebNewsContext _context;

        public AccountsController(WebNewsContext context)
        {
            _context = context;
        }

        // GET: Admin/Accounts
        public IActionResult Index(int? page)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = Utilities.PAGE_SIZE;

            var lsAccounts = _context.Accounts
                .Include(a => a.Role)
                .OrderByDescending(x => x.CreateDate);
            PagedList<Account> models = new PagedList<Account>(lsAccounts, pageNumber, pageSize);
            return View(models);
        }
        [Route("thong-tin-tai-khoang.html", Name = "Profile")]
        public async Task<IActionResult> Profile()
        {
            return View(await _context.Accounts.ToListAsync());
        }

        // GET: Admin/Roles/Details/5

        // GET: Admin/Login
        [HttpGet]
        [AllowAnonymous]
        [Route("dang-nhap.html", Name = "Login")]           
        public IActionResult Login(string returnUrl = null)
        {
            var taikhoangID = HttpContext.Session.GetString("AccountId");
            if (taikhoangID != null) return RedirectToAction("Indexs", "Home", new { Area = "Admin" });
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // GET: Admin/Login
        [HttpPost]
        [AllowAnonymous]
        [Route("dang-nhap.html", Name = "Login")]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Account kh = _context.Accounts
                        .Include(p => p.Role)
                        .SingleOrDefault(p => p.Email.ToLower() == model.EmailID.ToLower().Trim());

                    if (kh == null)
                    {
                        ViewBag.Error1 = "Email đăng nhập không tồn tại !";
                        return View(model);
                    }
                    string pass = model.Password.Trim() /*+ kh.Salt.Trim().ToMD5()*/;
                    if (kh.PassWord.Trim() != pass)
                    {
                        ViewBag.Error = "Mật khẩu đăng nhập chưa chính xác !";
                        return View(model);
                    }

                    //Đăng nhập thành công
                    //Ghi nhận thời gian đăng nhập

                    kh.LastLogin = DateTime.Now;
                    _context.Update(kh);
                    await _context.SaveChangesAsync();

                    var taikhoangID = HttpContext.Session.GetString("AccountId");

                    //Identity
                    //Lưu Session Makh

                    HttpContext.Session.SetString("AccountId", kh.AccountId.ToString());

                    ////Identity
                        var userClaims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, kh.FullName),
                            new Claim(ClaimTypes.Email, kh.Email),
                            new Claim("AccountId", kh.AccountId.ToString()),
                            new Claim("RoleId", kh.RoleId.ToString()),
                            new Claim(ClaimTypes.Role, kh.Role.RoleName)
                        };
                    
                    var grandmaIdentity = new ClaimsIdentity(userClaims, "User Identity");
                    var userPrincipal = new ClaimsPrincipal(new[] { grandmaIdentity });
                    await HttpContext.SignInAsync(userPrincipal);
                    

                    return RedirectToAction("Indexs", "Home", new { Area = "Admin" });
                }
            }
            catch
            {
                return RedirectToAction("Login", "Accounts", new { Area = "Admin" });
            }
            return RedirectToAction("Login", "Accounts", new { Area = "Admin" });
        }

        //GET:Admin/Logout
        [Route("dang-xuat.html", Name = "Logout")]
        public IActionResult Logout()
        {
            try
            {
                HttpContext.SignOutAsync();
                HttpContext.Session.Remove("AccountId");
                return RedirectToAction("Index", "Home");
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Admin/ChangePassword
        [Route("doi-mat-khau.html", Name = "ChangePassword")]
        [Authorize, HttpGet]
        public IActionResult ChangePassword()
        {
            if (!User.Identity.IsAuthenticated) Response.Redirect("/dang-nhap.html");
            var taikhoangID = HttpContext.Session.GetString("AccountId");
            if (taikhoangID == null) return RedirectToAction("Login", "Accounts", new { Area = "Admin" });

            return View();
        }

        //GET:Admin/ChangePassword
        [Route("doi-mat-khau.html", Name = "ChangePassword")]
        [Authorize, HttpPost]
        public IActionResult ChangePassword(ChangePassWordViewModel model)
        {
            if (!User.Identity.IsAuthenticated) Response.Redirect("/dang-nhap.html");
            var taikhoangID = HttpContext.Session.GetString("AccountId");
            if (taikhoangID == null) return RedirectToAction("Login", "Accounts", new { Area = "Admin" });
            if (ModelState.IsValid)
            {
                var account = _context.Accounts.AsNoTracking().FirstOrDefault(x => x.AccountId == int.Parse(taikhoangID));
                if(account == null) return RedirectToAction("Login", "Accounts", new { Area = "Admin" });
                try
                {
                    string passnow = (model.PassWordNow.Trim() + account.Salt.Trim()).ToMD5();
                    if (passnow == account.PassWord.Trim())
                    {
                        _context.Update(account);
                        _context.SaveChanges();
                        return RedirectToAction("Profile", "Accounts", new { Area = "Admin" });
                    }
                    else
                    {
                        ViewBag.Error = "Mật khẩu đăng nhập chưa chính xác !";
                        return View();
                    }
                }
                catch
                {
                    return View();
                }
            }
            return View();
        }

        //GET:Admin/Edit profile
        [Route("edit-profile.html", Name = "EditProfile")]
        [Authorize, HttpGet]
        public IActionResult EditProfile()
        {
            if (!User.Identity.IsAuthenticated) Response.Redirect("/dang-nhap.html");
            var taikhoangID = HttpContext.Session.GetString("AccountId");
            if (taikhoangID == null) return RedirectToAction("Login", "Accounts", new { Area = "Admin" });
            var account = _context.Accounts.AsNoTracking().FirstOrDefault(x => x.AccountId == int.Parse(taikhoangID));
            if (account == null) return NotFound();
            return View(account);
        }

       // GET:Admin/Edit profile
        [Route("edit-profile.html", Name = "EditProfile")]
        [Authorize, HttpPost]
        public async Task<IActionResult> EditProfileAsync(Account model, Microsoft.AspNetCore.Http.IFormFile fThumb)
        {
            if (!User.Identity.IsAuthenticated) Response.Redirect("/dang-nhap.html");
            var taikhoangID = HttpContext.Session.GetString("AccountId");
            if (taikhoangID == null) return RedirectToAction("Login", "Accounts", new { Area = "Admin" });
            if (ModelState.IsValid)
            {
                var account = _context.Accounts.AsNoTracking().FirstOrDefault(x => x.AccountId == int.Parse(taikhoangID));
                try
                {
                    account.FullName = model.FullName;
                    account.Phone = model.Phone;
                    account.Email = model.Email;

                    if (fThumb != null)
                    {
                        string extension = Path.GetExtension(fThumb.FileName);
                        string NewName = Utilities.SEOUrl(account.FullName) + "preview_" + extension;
                        account.Thumb = await Utilities.UploadFile(fThumb, @"accounts\", NewName.ToLower());
                    }

                    _context.Update(account);
                    _context.SaveChanges();
                    return RedirectToAction("Profile", "Accounts", new { Area = "Admin" });
                }
                catch
                {
                    return View(model);
                }
            }
            return View();
        }


         //GET: Admin/Accounts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts
                .Include(a => a.Role)
                .FirstOrDefaultAsync(m => m.AccountId == id);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

         //GET: Admin/Accounts/Create
        public IActionResult Create()
        {
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleId");
            return View();
        }

         //POST: Admin/Accounts/Create
         //To protect from overposting attacks, enable the specific properties you want to bind to.
         //For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AccountId,FullName,Email,Phone,PassWork,Salt,Active,CreateDate,RoleId,LastLogin")] Account account, Microsoft.AspNetCore.Http.IFormFile fThumb)
        {
            if (ModelState.IsValid)
            {
                account.LastLogin = null;
                account.Salt = Utilities.GetRandomKey();
                var passwordMD5 = (account.PassWord + account.Salt).ToMD5();
                account.PassWord = passwordMD5;
                account.CreateDate = DateTime.Now;
                if (fThumb != null)
                {
                    string extension = Path.GetExtension(fThumb.FileName);
                    string NewName = Utilities.SEOUrl(account.FullName) + "preview_" + extension;
                    account.Thumb = await Utilities.UploadFile(fThumb, @"accounts\", NewName.ToLower());
                }

                _context.Add(account);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleId", account.RoleId);
            return View(account);
        }

        // GET: Admin/Accounts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
            {
                return NotFound();
            }
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleId", account.RoleId);
            return View(account);
        }

         //POST: Admin/Accounts/Edit/5
         //To protect from overposting attacks, enable the specific properties you want to bind to.
         //For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AccountId,FullName,Email,Phone,PassWork,Salt,Active,CreateDate,RoleId,LastLogin")] Account account)
        {
            if (id != account.AccountId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    account.LastLogin = null;
                    account.Salt = Utilities.GetRandomKey();
                    var passwordMD5 = (account.PassWord + account.Salt).ToMD5();
                    account.PassWord = passwordMD5;
                    _context.Update(account);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AccountExists(account.AccountId))
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
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleId", account.RoleId);
            return View(account);
        }

         //GET: Admin/Accounts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts
                .Include(a => a.Role)
                .FirstOrDefaultAsync(m => m.AccountId == id);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

         //POST: Admin/Accounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var account = await _context.Accounts.FindAsync(id);
            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AccountExists(int id)
        {
            return _context.Accounts.Any(e => e.AccountId == id);
        }
    }
}
