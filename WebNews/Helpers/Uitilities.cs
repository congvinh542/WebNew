using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WebNews.Helpers
{
    public static class Utilities
    {

        public static string ToVnd(this double donGia)
        {
            return donGia.ToString("#,##0") + " đ";
        }

        public static string SEOUrl(string Url)
        {
            Url = Url.ToLower();
            Url = Regex.Replace(Url, @"[áàạảãâấầậẩẫăắằặẳẵ]", "a");
            Url = Regex.Replace(Url, @"[éèẹẻẽêếềệểễ]", "e");
            Url = Regex.Replace(Url, @"[óòọỏõôốồộổỗơớờợởỡ]", "o");
            Url = Regex.Replace(Url, @"[úùụủũưứừựửữ]", "u");
            Url = Regex.Replace(Url, @"[íìịỉĩ]", "i");
            Url = Regex.Replace(Url, @"[ýỳỵỷỹ]", "y");
            Url = Regex.Replace(Url, @"[đ]", "d");
            Url = Regex.Replace(Url.Trim(), @"[^0-9a-z-\s]", "").Trim();
            Url = Regex.Replace(Url.Trim(), @"\s+", "-");
            Url = Regex.Replace(Url.Trim(), @"\s", "-");
            while (true)
            {
                if (Url.IndexOf("--") != -1)
                {
                    Url = Url.Replace("--", "-");
                }
                else
                {
                    break;
                }
            }

            return Url;
        }
        public static int PAGE_SIZE = 20;
        public static int PAGE_SIZE2 = 5;

        public static string GetRandomKey(int length = 5)
        {
            //chuỗi mẫu (pattern)
            string pattern = @"0123456789zxcvbnmasdfghjklqwertyuiop[]{}:~!@#$%^&*()+";
            Random rd = new Random();
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < length; i++)
            {
                sb.Append(pattern[rd.Next(0, pattern.Length)]);
            }

            return sb.ToString();
        }
        public static async Task<string> UploadFile(Microsoft.AspNetCore.Http.IFormFile file, string sDirectory, string newname = null)
        {
            try
            {
                if (newname == null) newname = file.FileName;
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", sDirectory);
                if (!System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.CreateDirectory(path);
                }
                var supportedTypes = new[] { "jpg", "jpeg", "png", "gif" };
                var fileExt = System.IO.Path.GetExtension(file.FileName).Substring(1);
                if (!supportedTypes.Contains(fileExt.ToLower())) // Khác các file định nghĩa
                {
                    return null;
                }
                else
                {
                    string fullPath = path + "//" + newname;
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    return newname;
                }
            }
            catch
            {
                return null;
            }
        }
        public static async Task UploadFileToFolderImages(Microsoft.AspNetCore.Http.IFormFile file, string newname)
        {
            try
            {
                if (string.IsNullOrEmpty(newname)) newname = file.FileName;
                var tetx = Directory.GetCurrentDirectory();
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", newname);
                string folderImage = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                if (!System.IO.Directory.Exists(folderImage))
                {
                    System.IO.Directory.CreateDirectory(folderImage);
                }
                var supportedTypes = new[] { "jpg", "jpeg", "png", "gif" };
                var fileExt = System.IO.Path.GetExtension(file.FileName).Substring(1);
                if (!supportedTypes.Contains(fileExt.ToLower())) // Khác các file định nghĩa
                {
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                }
            }
            catch
            {
                
            }
        }
    }
}
