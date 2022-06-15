using System;
using System.Collections.Generic;

#nullable disable

namespace WebNews.Models
{
    public partial class ReplyCm
    {
        public int Id { get; set; }
        public string Comemnt { get; set; }
        public DateTime? CommentDate { get; set; }
        public int? Cmid { get; set; }
        public int? AccountId { get; set; }

        public virtual Account Account { get; set; }
        public virtual Comment Cm { get; set; }
    }
}
