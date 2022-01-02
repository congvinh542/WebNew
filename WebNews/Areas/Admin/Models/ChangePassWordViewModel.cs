using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebNews.Areas.Admin.Models
{
    public class ChangePassWordViewModel
    {
        [Key]
        public int AccountId { set; get; }
        [Display(Name = "Mật khẩu hiện tại")]
        public string PassWordNow { set; get; }
        [Display(Name = "Mật khẩu mới")]
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [MinLength(5, ErrorMessage = "Bạn cần đặt mật khẩu tối thiểu 5 ký tự")]
        public string PassWord { set; get; }
        [MinLength(5, ErrorMessage = "Bạn cần đặt mật khẩu tối thiểu 5 ký tự")]
        [Display(Name = "Nhập lại mật khẩu mới")]
        [Compare("PassWord", ErrorMessage = "Mật khẩu không giống nhau")]
        public string ConfimPassWord { set; get; }
    }
}
