using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;


namespace MyBlog.Models


{

    public class Post
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        [Display(Name = "Заголовок")]
        public string Title { get; set; }

        [Required]
        [MaxLength(256)]
        [Display(Name = "Стислий опис")]
        public string Description { get; set; }

        [Required]
        [MaxLength(256)]
        [Display(Name = "Текст")]
        public string Content { get; set; }


        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Дата публікації")]
        public DateTime PublishDate { get; set; }

        [Required]
        [DataType(DataType.Time)]
        [Display(Name = "Час публікації")]
        public DateTime PublishTime { get; set; }


        [MaxLength(100)]
        [Display(Name = "Зображення")]
        public string ImagePath { get; set; }

        public int CategoryId { get; set; }

        [Display(Name = "Категорія")]
        public virtual Category Category { get; set; }

        public virtual List<Comment>? Comments { get; set; }

    }

}

