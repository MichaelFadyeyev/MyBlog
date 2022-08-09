using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;


namespace MyBlog.Models
{

    public class Comment
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(256)]
        [Display(Name = "Comment Text")]
        public string Content { get; set; }


        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Publish Date")]
        public DateTime PublishDate { get; set; }

        [Required]
        [DataType(DataType.Time)]
        [Display(Name = "Publish Time")]
        public DateTime PublishTime { get; set; }

        public int ApplicationUserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }

        public int PostId { get; set; }
        public virtual Post Post { get; set; }
    }

}
