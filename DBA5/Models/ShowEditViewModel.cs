namespace DBA5.Models
{
   using System.ComponentModel.DataAnnotations;
   using System;

   public class ShowEditViewModel
   {
      public int Id { get; set; }

      [Required, StringLength(150)]
      public string Name { get; set; }

      [Required, StringLength(50)]
      public string Genre { get; set; }

      [Required, DataType(DataType.Date)]
      public DateTime ReleaseDate { get; set; }

      [Required, StringLength(250)]
      public string ImageUrl { get; set; }

      [StringLength(250)]
      public string Coordinator { get; set; }

      [DataType(DataType.MultilineText)]
      public string Premise { get; set; }

      
   }
}
