using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DBA5.Models
{
    public class GenreBaseViewModel
    {
        // Columns
        [Key]
        public int Id { get; set; }

        [Required, StringLength(200)]
        public string Name { get; set; }
    }
}