using System;
using System.Collections.Generic;

#nullable disable

namespace WebNews.Models
{
    public partial class Comment
    {
        public Comment()
        {
            RepComments = new HashSet<RepComment>();
        }

        public int CommentId { get; set; }
        public string Comment1 { get; set; }
        public DateTime? CommentDate { get; set; }
        public int? PostId { get; set; }
        public int? Uid { get; set; }

        public virtual Post Post { get; set; }
        public virtual UserPage UidNavigation { get; set; }
        public virtual ICollection<RepComment> RepComments { get; set; }
    }
}
