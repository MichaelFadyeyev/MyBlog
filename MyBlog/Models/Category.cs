using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyBlog.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        [Display(Name = "Категорія")]
        public string Name { get; set; }

        public virtual List<Post> Posts {get;set;}
    }
}
