using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Photo_Sea.Models
{
    public partial class TablePhoto
    {
        [Key]
        public int Id { get; set; }
        public string Cname { get; set; }
        public string PictureName { get; set; }
        public string Type { get; set; }
        public DateTime? Date { get; set; }
        [NotMapped]
        [DisplayName("Upload Photo")]
        public IFormFile PhotoName { get; set; }
    }
}
