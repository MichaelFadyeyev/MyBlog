using System;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;


namespace MyBlog.Models
{
    public class ApplicationUser : IdentityUser
    {
        public virtual List<Comment> Comments { get; set; }

    }

}
