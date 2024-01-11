using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DBA5.Models
{
    public class ActorMediaItemAddFormViewModel
    {
        public int ActorId { get; set; }

        [Display(Name = "Actor Name")]
        public string ActorName { get; set; }

        // Brief descriptive caption
        [Required, StringLength(100)]
        public string Caption { get; set; }

        // TODO 09 - In this "Form" class, the property type is string, and the data type is upload
        // The DataType.Upload uses the Views > Shared > EditorTemplates > Upload.cshtml template
        [Required]
        [Display(Name = "Upload Content")]
        [DataType(DataType.Upload)]
        public string ContentUpload { get; set; }
    }

    public class ActorMediaItemAddViewModel
    {
        public int ActorId { get; set; }

        // Brief descriptive caption
        [Required, StringLength(100)]
        public string Caption { get; set; }

        // TODO 11 - In this "Form" class, the property type is HttpPostedFileBase, and the data type is upload
        [Required]
        public HttpPostedFileBase ContentUpload { get; set; }
    }

    public class ActorMediaItemBaseViewModel
    {
        public int Id { get; set; }

        // Brief descriptive caption
        [Required, StringLength(100)]
        public string Caption { get; set; }

        public string ContentType { get; set; }
    }

    public class ActorMediaItemWithContentViewModel : ActorMediaItemBaseViewModel
    {
        public byte[] Content { get; set; }
    }

}