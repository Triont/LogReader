using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication25.Models;
using System.IO;
using IpInfo;
using System.Text.RegularExpressions;
using System.Text;

namespace WebApplication25.Services
{

  public  class LogElement
    {
        public FilesInfo FilesInfo { get; set; } = new FilesInfo();
        public IPinfo IpInfo { get; set; } = new IPinfo();
        public MainTable MainTable { get; set; } = new MainTable();
    }
    public class HandleLog
    {
        private readonly AppDbContext appDbContext;
        public string IP { get; set; }
       
        public string Text { get; set; }
        

        public string[] lines { get; set; }
        public List<LogElement> LogElements { get; set; } = new List<LogElement>();
        public async Task<bool> GetData(string path)
        {
            if(File.Exists(path))
            {
                lines = File.ReadAllLines(path);
                var fullText =await File.ReadAllTextAsync(path);


                Regex ip = new Regex(@"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b");
                MatchCollection result = ip.Matches(fullText);
                for (int i=0; i<fullText.Length;i++)
                {
                    MainTable mainTable = new();
                    FilesInfo filesInfo = new();
                    IPinfo ip_info = new();
                   ip_info.IPAddress=result[]



                }


            }
            return await Task.Run(() => false);

        }


        public async Task<List<byte[]>> GetAllIp(string[]lines )
        {
            Regex ip = new Regex(@"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b");
            List<byte[]> vs = new List<byte[]>();
            for(int i=0; i<lines.Length;i++)
            {
                var tmp = ip.Match(lines[i]).Value;
                var byte_val = Encoding.ASCII.GetBytes(tmp);
                vs.Add(byte_val);
            }   
         return   await Task.Run(() => vs);



        }

        public List<string> GetDateTimesString(string[]lines)
        {
            List<string> tmp = new List<string>();
            Regex regex = new Regex(@"\[([^[\]]+)\]");
            for(int i=0;i<lines.Length;i++)
            {
                var m = regex.Match(lines[i]);
                tmp.Add(m.Value);
            }
            return tmp;
            
        }
        public List<DateTime> GetDateTimes(List<string> str)
        {
            List<DateTime> dateTimes = new List<DateTime>();
            for (int i = 0; i < str.Count; i++)
            {
                var tmp_q = str[i].Remove(0, 1);
                var splitted = tmp_q.Split(new char[] { ':' });
                TimeSpan uioa = TimeSpan.Parse("10:11:23");
                Console.WriteLine(uioa);
                DateTime dateTime = DateTime.Parse(splitted[0]);



                StringBuilder stringBuilder = new StringBuilder(tmp_q);
                stringBuilder.Replace(splitted[0], string.Empty);
                stringBuilder.Remove(0, 1);
                string res = stringBuilder.ToString();
                var res_first = res.Split(new char[] { ' ' });
                TimeSpan timeSpan = TimeSpan.Parse(res_first[0]);
                DateTime dateTime1 = new DateTime(
                    dateTime.Year, dateTime.Month, dateTime.Day, timeSpan.Hours, timeSpan.Minutes,
                    timeSpan.Seconds

                    );

                // Console.WriteLine(dateTime1);

                var str_timezone = res_first[1];
                var sign = str_timezone[0];
                var time_zone = str_timezone.Remove(0, 1);
                var r = string.Empty;
                if(time_zone[3]==0)
                {
                    r = time_zone.TrimEnd('0');
                }

                //add condition for minutes add and get hours and minutes;
                var _t = time_zone.TrimStart('0');
              
                var dq = dateTime1.Subtract(TimeSpan.FromHours(5));//-5 timezone

            }
        }
    }
}
