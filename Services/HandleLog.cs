using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using IpInfo;

namespace WebApplication25.Services
{
    public class HandleLog
    {
        public string IP { get; set; }
       
        public string[] lines { get; set; }
        public async Task<bool> GetData(string path)
        {
            if(File.Exists(path))
            {
                lines = File.ReadAllLines(path);

            }
            return await Task.Run(() => false);

        }
    }
}
