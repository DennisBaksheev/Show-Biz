// File: EpisodeEditViewModel.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace DBA5.Models
{
   public class EpisodeEditViewModel
   {
      public int Id { get; set; }

      [Required, StringLength(150)]
      public string Name { get; set; }

      // Add other properties that you want to be able to edit
      [Display(Name = "Season")]
      public int SeasonNumber { get; set; }

      [Display(Name = "Episode")]
      public int EpisodeNumber { get; set; }

      [Required]
      public string Genre { get; set; }

      [Required, Display(Name = "Date Aired"), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true), DataType(DataType.Date)]
      public DateTime AirDate { get; set; }

      [Required, StringLength(250), Display(Name = "Image")]
      public string ImageUrl { get; set; }

      
   }
}
