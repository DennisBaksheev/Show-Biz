using System.ComponentModel.DataAnnotations;
using System;
namespace DBA5.Models { 
public class ActorEditViewModel
{
   public int Id { get; set; }
   [Required, StringLength(150)]
   public string Name { get; set; }
   [StringLength(150)]
   public string AlternateName { get; set; }
   public DateTime? BirthDate { get; set; }
   public double? Height { get; set; }
   [Required, StringLength(250)]
   public string ImageUrl { get; set; }
   [Required, StringLength(250)]
   public string Executive { get; set; }
   public string Biography { get; set; }
}
}