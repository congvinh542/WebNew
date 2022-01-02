using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WebNews.Models;

namespace WebNews.ModelViews
{
    public class HomeViewModel
    {
        [Display(Name = "Phổ biến")]
        public List<Post> Populars { set; get; }
        public List<Post> Inspiration{ set; get; }
        [Display(Name = "Gần đây")]
        public List<Post> Recents{set;get;}
        public List<Post> Trendings{set;get;}
        public List<Post> LatestPosst{set;get;}
        public Post Featured { set; get; }
    }
}
