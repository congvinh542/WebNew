using System;
using System.Collections.Generic;

#nullable disable

namespace WebNews.Models
{
    public partial class Post
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string ShortContent { get; set; }
        public string Content { get; set; }
        public string Thumb { get; set; }
        public bool Published { get; set; }
        public string Alias { get; set; }
        public DateTime? DateTime { get; set; }
        public string Author { get; set; }
        public string Tags { get; set; }
        public string MetaKey { get; set; }
        public string MetaDec { get; set; }
        public int? CatId { get; set; }
        public bool IsHot { get; set; }
        public bool IsNewfeed { get; set; }
        public int? AccountId { get; set; }
        public int? Views { get; set; }
        public string Link { get; set; }

        public virtual Account Account { get; set; }
        public virtual Category Cat { get; set; }
    }
}
