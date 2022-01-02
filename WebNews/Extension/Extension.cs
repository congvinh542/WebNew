using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WebNews.Extension
{
    public static class Extension
    {
        public static string ToVnd(this double donGia)
        {
            return donGia.ToString("#,##0") + "đ"; // chuyển đổi 5 => 5.000đ
        }
        public static string ToUrlFriendly(this string url)
        {
            var resualt = url.ToLower().Trim();
            resualt = Regex.Replace(resualt, "áàạảãấầậẩẫăắằặẵ", "a");
            resualt = Regex.Replace(resualt, "éèẹẻẽêếềệểễ", "e");
            resualt = Regex.Replace(resualt, "óòọỏõôốồổỗộơớờởỡợ", "o");
            resualt = Regex.Replace(resualt, "úùủụuũưứừựửữ", "u");
            resualt = Regex.Replace(resualt, "íìịỉĩ", "i");
            resualt = Regex.Replace(resualt, "ýỳỵỷỹ", "y");
            resualt = Regex.Replace(resualt, "đ", "d");
            resualt = Regex.Replace(resualt, "[^a-z0-9-]", "");
            resualt = Regex.Replace(resualt, "(-)+", "-");
            // chuyển đổi 'chào bạn' => 'chao-ban'
            return resualt;
        }
    }
}
