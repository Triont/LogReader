using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication25.Models;

namespace WebApplication25.ModelView
{
    public class MainModelView
    {
        public List<MainTable> MainTables { get; set; }
        //public DateTime DateTime { get; set; }
        //public string DateTimeLog { get; set; }
        //public string RequestType { get; set; }
        //public int RequestResult { get; set; }

        //public long DataVolume { get; set; }
        //public string IpAddress { get; set; }
        //public string CompanyName { get; set; }
        //public string PageName { get; set; }
        //public string FilePath { get; set; }

        public string Search { get; set; }
        public List<string> Filters { get; set; }
        public List<string> CheckedItems { get; set; }

    }
}
