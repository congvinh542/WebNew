using System;
using System.Collections.Generic;

#nullable disable

namespace WebNews.Models
{
    public partial class UserPage
    {
        public UserPage()
        {
            Comments = new HashSet<Comment>();
            RepComments = new HashSet<RepComment>();
        }

        public int Uid { get; set; }
        public string UserName { get; set; }
        public string Image { get; set; }
        public string Email { get; set; }
        public string PassWord { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<RepComment> RepComments { get; set; }
    }
}
