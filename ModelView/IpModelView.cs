using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication25.Models;

namespace WebApplication25.ModelView
{
    public class IpModelView
    {
        public List<IPinfo> IpData { get; set; }
        public List<string> Filters { get; set; }
        public List<string> CheckedItems { get; set; }
      
        public string _search { get; set; }
    }
    public class FilesModelView
    {
        public List<FilesInfo> FilesInfos { get; set;  }
        public List<string> Filters { get; set; }
        public List<string> CheckedItems { get; set; }
            public string _search { get; set; }
      
    }
}
