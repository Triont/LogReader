using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication25.Models;

namespace WebApplication25.ModelView
{
    public class UploadLogModelView
    {
        [AllowedExtensions(new string[] {".log"})]
        public IFormFile FormFile { get; set; } 
    }
}
