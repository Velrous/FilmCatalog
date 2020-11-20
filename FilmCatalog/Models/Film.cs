using System;
using System.ComponentModel.DataAnnotations;

namespace FilmCatalog.Models
{
    public class Film
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Пожалуйста, введите название фильма")]
        [Display(Name = "Название")]
        [StringLength(100)]
        public string Title { get; set; }

        [Required(ErrorMessage = "Пожалуйста, введите описание фильма")]
        [Display(Name = "Описание")]
        public string Desc { get; set; }

        [Required(ErrorMessage = "Пожалуйста, введите год выпуска")]
        [Display(Name = "Год выпуска")]
        public short Year { get; set; }

        [Required(ErrorMessage = "Пожалуйста, введите режиссёра")]
        [Display(Name = "Режиссёр")]
        [StringLength(150)]
        public string Director { get; set; }

        [Required(ErrorMessage = "Не указан UserEmail")]
        [StringLength(100)]
        public string UserEmail { get; set; }

        [Required(ErrorMessage = "Пожалуйста, добавьте постер фильма")]
        [Display(Name = "Постер фильма")]
        public string Image { get; set; }

    }
}
