using System;
using System.Collections.Generic;

#nullable disable

namespace WebNews.Models
{
    public partial class RepComment
    {
        public int ReplyId { get; set; }
        public string ComemntRl { get; set; }
        public DateTime? CommentDate { get; set; }
        public int? CommentId { get; set; }
        public int? Uid { get; set; }

        public virtual Comment Comment { get; set; }
        public virtual UserPage UidNavigation { get; set; }
    }
}
