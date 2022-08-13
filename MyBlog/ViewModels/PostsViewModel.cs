 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyBlog.Models;


namespace MyBlog.ViewModels
{
    public class PostsViewModel
    {
        public List<Post> Posts { get; set; }
        public SelectList Categories { get; set; }
        public PageViewModel Paginator { get; set; }
    }
}
