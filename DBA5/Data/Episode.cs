using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DBA5.Data
{
    [Table("Episode")]
    public class Episode
    {
        // Constructor
        public Episode() 
        {
            AirDate = DateTime.Now;
        }

        // Columns
        public int Id { get; set; }

        [Required, StringLength(150)]
        public string Name { get; set; }

        public int SeasonNumber { get; set; } 

        public int EpisodeNumber { get; set; }

        [Required]
        public string Genre { get; set; }

        [Required, DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime AirDate { get; set; }

        [Required, StringLength(250)]
        public string ImageUrl { get; set; }

        [Required, StringLength(250)]
        public string Clerk { get; set; }

        public string Premise { get; set; }

        // TODO 1 - Design model class needs two properties for the media item
        // Here, both can be null, but the view model classes will require values
        [StringLength(200)]
        public string VideoContentType { get; set; }
        public byte[] Video { get; set; }


        // Navigation Property
        [Required]
        public Show Show { get; set; }
    }
}