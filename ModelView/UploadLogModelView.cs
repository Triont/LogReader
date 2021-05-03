using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WebApplication25.Models;

namespace WebApplication25.ModelView
{
    public class UploadLogModelView
    {
        [Required(ErrorMessage = "Please select a file.")]
        [DataType(DataType.Upload)]
            
        [AllowedExtensions(new string[] {".log"})]
       
        public IFormFile FormFile { get; set; } 
    }
}
