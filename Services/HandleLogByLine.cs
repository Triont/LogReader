using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using WebApplication25.Models;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.IO;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using System.Net.Http;
using IpInfo;
using Newtonsoft.Json;
using Newtonsoft;

namespace WebApplication25.Services
{
    public class HandleLogByLine
    {


        private readonly AppDbContext appDbContext;
        private ILogger<HandleLog> Logger;
        public HandleLogByLine(AppDbContext appDbContext, ILogger<HandleLog> logger)
        {
            this.appDbContext = appDbContext;
            this.Logger = logger;
        }



        public string[] lines { get; set; }
     
        public async Task<bool> GetData(string path)
        {
            if (File.Exists(path))
            {
                lines = File.ReadAllLines(path);
                List<string> allowed = new List<string>();
                for (int i = 0; i < lines.Length; i++)
                {
                    if (!(lines[i].Contains("media") || lines[i].Contains("scripts") || lines[i].Contains("css")
                        || lines[i].Contains("/js/") || lines[i].Contains("assets")
                        || lines[i].Contains("png") || lines[i].Contains("jpg")
                        || lines[i].Contains("php") || lines[i].Contains("/administrator/")
                        || lines[i].Contains("ico")

                        ))
                    // if (lines[i].Contains("search"))
                    {
                        if (!(lines[i].Contains(" - admin ") || lines[i].Contains("/administrator ")))
                        {
                          allowed.Add(lines[i]);
                        }
                    }
                }

                //allowed.Add(lines[0]);
                //allowed.Add(lines[1]);
                //allowed.Add(lines[4593]);
                //allowed.Add(lines[249]);
                //allowed.Add(lines[359]);
                //allowed.Add(lines[499]); allowed.Add(lines[5]);
                //allowed.Add(lines[1540]);
                //allowed.Add(lines[888]); allowed.Add(lines[89]);
                //allowed.Add(lines[90]);
                //allowed.Add(lines[493]);

                List<string> mainTables = new List<string>();
                List<string> ip = new List<string>();
                List<string> files = new List<string>();

                IPinfo infoIp = new IPinfo();
                FilesInfo filesInfo = new FilesInfo();


                List<byte[]> ip_list = new List<byte[]>();
                List<string> dateTimeStr = new List<string>();
                List<DateTime> dateTimes = new List<DateTime>();
                List<string> urls = new List<string>();
                List<string> request_types = new List<string>();
                List<string> compNames = new List<string>();
                List<int> results = new List<int>();
                List<long> dataVolumes = new List<long>();
                List<string> names = new List<string>();

             //   Parallel.For(0, allowed.Count, async(i) =>
                for(int i=0; i<allowed.Count;i++)
                {
                     var _ip = GetIp(allowed[i]);
                     ip_list.Add(GetIp(allowed[i]));
                     var _datetimestr = await GetDateTimesString(allowed[i]);
                     dateTimeStr.Add(await GetDateTimesString(allowed[i]));
                     dateTimes.Add(await _GetDateTimes(_datetimestr));
                     var t = await GetRequests(allowed[i]);
                     string request;
                     string url;
                     ParseRequest(t, out request, out url);
                     request_types.Add(request);
                     urls.Add(url);
                     
                     var check_comp = await appDbContext.IpInfo.FirstOrDefaultAsync(i => i.IPAddress==_ip);
                     if (check_comp == null)
                     {
                         var compName = await GetCompaniesName(_ip);
                         compNames.Add(compName);
                     }
                     else
                     {


                         var compName_ = await appDbContext.IpInfo.FirstOrDefaultAsync(i => i.IPAddress == _ip);
                         compNames.Add(compName_.CompanyName);
                     }
                    int _res;
                    long volume;

                    GetResultAndDataVolume(allowed[i], out _res, out volume);
                    var check_name = await appDbContext.FilesInfos.FirstOrDefaultAsync(a => a.Path == url);
                     if (check_name==null)
                     {
                        if(_res==200)
                        {
                                var name = await GetNames(url);
                                 names.Add(name);
                        }
                        else
                        {
                            names.Add(String.Empty);
                        }
                           
                     }
                     else
                     {
                         var _name = await appDbContext.FilesInfos.FirstOrDefaultAsync(a => a.Path == url);
                         names.Add(_name.Name);
                     }
                    
                 
                     results.Add(_res);
                     dataVolumes.Add(volume);
                   

                    
                 }



                for (int i = 0; i < allowed.Count; i++)
                {
                    //if(compNames.Count==ip_list.Count && urls.Count==compNames.Count && urls.Count==names.Count && dataVolumes.Count==urls.Count)
                    infoIp.CompanyName = compNames[i];
                    infoIp.IPAddress = ip_list[i];
                    if (names[i] != string.Empty)
                    {
                        filesInfo.Path = urls[i];
                        filesInfo.Name = names[i];
                        filesInfo.DataVolume = dataVolumes[i];
                    }
                    if (!ip.Contains(JsonConvert.SerializeObject(infoIp)))
                    {
                        infoIp = new IPinfo() { CompanyName = compNames[i], IPAddress = ip_list[i] };
                        ip.Add(JsonConvert.SerializeObject(infoIp));
                    }
                    if (!files.Contains(JsonConvert.SerializeObject(filesInfo)))
                    {
                        filesInfo = new FilesInfo() { Name = names[i], Path = urls[i], DataVolume = dataVolumes[i] };
                    }

                    if ((appDbContext.IpInfo.Where(q => q.IPAddress == infoIp.IPAddress).Count() == 0) && appDbContext.FilesInfos.Where(q => q.Path == filesInfo.Path).Count() == 0)
                    {


                        MainTable mainTable = new MainTable()
                        {
                            DataVolume = dataVolumes[i],
                            DateTime = dateTimes[i],
                            DateTimeLog = dateTimeStr[i],
                            FilesInfo = filesInfo,
                            _IPinfo = infoIp,
                            RequestType = request_types[i],
                            RequestResult = results[i]
                        };
                        if (!mainTables.Contains(JsonConvert.SerializeObject(mainTable)))
                        {
                            mainTables.Add(JsonConvert.SerializeObject(mainTable));
                            await appDbContext.MainTable.AddAsync(mainTable);

                        }
                        if (!files.Contains(JsonConvert.SerializeObject(filesInfo)))
                        {
                            files.Add(JsonConvert.SerializeObject(filesInfo));
                            await appDbContext.FilesInfos.AddAsync(filesInfo);
                        }
                        if (!ip.Contains(JsonConvert.SerializeObject(infoIp)))
                        {
                            ip.Add(JsonConvert.SerializeObject(infoIp));
                            await appDbContext.IpInfo.AddAsync(infoIp);
                        }
                    }
                }
                await appDbContext.SaveChangesAsync();
             
            }
            return await Task.Run(() => false);
        }





        public byte[] GetIp(string line)
        {
            Regex ip = new Regex(@"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b");

            Regex incorrect_ip = new Regex(@"\b\d{1,3}\-\d{1,3}\-\d{1,3}\-\d{1,3}");


            byte[] arr = new byte[2048];
            if(ip.Match(line).Success)
            {
                var tmp = ip.Match(line).Value;
                var byte_val = Encoding.ASCII.GetBytes(tmp);
                arr = byte_val;
            }
            else if(incorrect_ip.Match(line).Success)
            {
                var tmp = incorrect_ip.Match(line).Value;
                var correct = tmp.Replace('-', '.');
                arr = Encoding.ASCII.GetBytes(correct);
            }
            return arr;
          




        


        }
        public async Task<List<string>> GetAllIp(List<string> lines)
        {
            Regex ip = new Regex(@"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b");
            Regex incorrect_ip = new Regex(@"\b\d{1,3}\-\d{1,3}\-\d{1,3}\-\d{1,3}");
            List<string> vs = new List<string>();
            for (int i = 0; i < lines.Count; i++)
            {

                var tmp = ip.Match(lines[i]);
                var _incor = incorrect_ip.Match(lines[i]);
                if (tmp.Success)
                {
                    var val = tmp.Value;
                    vs.Add(val);
                }
                else if (_incor.Success)
                {
                    var _v = _incor.Value;
                    var corrected_ip = _v.Replace('-', '.');
                    vs.Add(corrected_ip);
                }



            }

            return await Task.Run(() => vs);



        }
      

        public async Task<string> GetDateTimesString(string line)
        {
            string tmp = string.Empty;
            Regex regex = new Regex(@"\[([^[\]]+)\]");
            await Task.Run(() =>
            {
                if(regex.Match(line).Success)
                {
                    var m = regex.Match(line);
                    tmp = m.Value;
                }
             
                   
                   
                
            });

            return tmp;

        }


        public async Task<List<string>> GetDateTimesString(List<string> lines)
        {
            List<string> tmp = new List<string>();
            Regex regex = new Regex(@"\[([^[\]]+)\]");

            await Task.Run(() =>
            {
                for (int i = 0; i < lines.Count; i++)
                {
                    var m = regex.Match(lines[i]);
                    if (m.Success)
                    {
                        tmp.Add(m.Value);
                    }
                }
            });
            //for (int i = 0; i < lines.Count; i++)
            //{
            //    var m = regex.Match(lines[i]);
            //    if (m.Success)
            //    {
            //        tmp.Add(m.Value);
            //    }
            //}
            return tmp;

        }


   

        public async Task<DateTime> _GetDateTimes(string str)
        {
            DateTime dateTime = new DateTime();
            await Task.Run(() =>
            {
                                
                    var tmp_str0 = str.Remove(0, 1);
                    var tmp_str1 = tmp_str0.Remove(tmp_str0.Length - 1, 1);
                    var _date = tmp_str1.Split(new char[] { ':' })[0];
                    var tmp_str2 = tmp_str1.Replace(_date, string.Empty);
                    var tmp_str3 = tmp_str2.Remove(0, 1);
                    var _time = tmp_str3.Split(new char[] { ' ' })[0];
                    var timezone = tmp_str3.Split(new char[] { ' ' })[1];

                    var date_parse = DateTime.Parse(_date);
                    var time_ = TimeSpan.Parse(_time);
                    DateTime dateTime = date_parse;
                    var DateTimeRes = dateTime.Add(time_);
                    DateTime ResultDateTime = new DateTime();

                    switch (timezone[0])
                    {
                        case '+':
                            string temp_timez = timezone.Remove(0, 1);
                            string t_timezone_h = temp_timez.Substring(0, temp_timez.Length / 2);
                            string t_timezone_m = temp_timez.Substring(temp_timez.Length / 2);
                            double hours;
                            double minutes;
                            Double.TryParse(t_timezone_h, out hours);
                            Double.TryParse(t_timezone_m, out minutes);
                            var h_add = TimeSpan.FromHours(hours);
                            var m_add = TimeSpan.FromMinutes(minutes);
                            ResultDateTime = DateTimeRes.Add(h_add).Add(m_add);
                            break;
                        case '-':
                            string temp_timez_ = timezone.Remove(0, 1);
                            string t_timezone_h0 = temp_timez_.Substring(0, temp_timez_.Length / 2);
                            string t_timezone_m0 = temp_timez_.Substring(temp_timez_.Length / 2);
                            double hours_s;
                            double minutes_s;
                            Double.TryParse(t_timezone_h0, out hours_s);
                            Double.TryParse(t_timezone_m0, out minutes_s);
                            var h_sb = TimeSpan.FromHours(hours_s);
                            var m_sb = TimeSpan.FromMinutes(minutes_s);
                            ResultDateTime = DateTimeRes.Subtract(h_sb).Subtract(m_sb);
                            break;

                    }
                dateTime = ResultDateTime;



                }
            );

            return dateTime;
        }

        public async Task<List<string>> GetNames(List<string> requests, Dictionary<string, string> done, string url = "http://tariscope.com")
        {
            List<string> names = new List<string>();
            for (int i = 0; i < requests.Count; i++)
            {
                StringBuilder stringBuilder = new StringBuilder(url);
                stringBuilder.Append(requests[i]);

                WebClient x = new WebClient();
                x.Proxy = null;
                string _s = string.Empty;
                using (MyClient myClient = new MyClient())
                {
                    myClient.HeadOnly = true;
                    if (!done.ContainsKey(requests[i]))
                    {
                        try
                        {
                            _s = await x.DownloadStringTaskAsync(stringBuilder.ToString());
                        }
                        catch (WebException e)
                        {
                            Logger.LogError($"{e.Message}, {e.Response}");
                        }
                    }
                    else
                    {
                        _s = done[requests[i]];
                    }

                }
            
                string _data = String.Empty;
                x.DownloadDataCompleted += (sender, e) =>
                {
                    _data = Encoding.ASCII.GetString(e.Result);
                };

         

                string title = Regex.Match(_s, @"\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>",
        RegexOptions.IgnoreCase).Groups["Title"].Value;
                if (title != String.Empty)
                {
                    //   if (!names.Contains(title))
                    names.Add(title);
                }
                else
                {
                    var _tmp = stringBuilder.ToString().Split(new char[] { '/' });

                    // if (!names.Contains(_tmp.Last()))
                    names.Add(_tmp.Last());
                }
            }
            return names;
        }

        public async Task<string> GetNames(string request, string url = "http://tariscope.com")
        {
            Dictionary<string, string> done = new Dictionary<string, string>();

            string names = string.Empty;
          
                StringBuilder stringBuilder = new StringBuilder(url);
                stringBuilder.Append(request);

                WebClient x = new WebClient();
                x.Proxy = null;
                string _s = string.Empty;
                string title = string.Empty;
            if (!done.ContainsKey(request))
            {
                var tmp_db = await appDbContext.FilesInfos.FirstOrDefaultAsync(tq => tq.Path == request);
                if (tmp_db == null)
                {
                    try
                    {



                        _s = await x.DownloadStringTaskAsync(stringBuilder.ToString());
                        title = Regex.Match(_s, @"\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>",
      RegexOptions.IgnoreCase).Groups["Title"].Value;

                        if (title != string.Empty)
                        {
                            done.Add(request, title);
                        }
                        else
                        {
                            var __tmp = title.Split(new char[] { '/' }).Last();
                            done.Add(request, __tmp);
                        }

                    }
                    catch (WebException e)
                    {
                        Logger.LogError($"{e.Message}, {e.Response}");
                    }
                }
                else
                {
                    _s = tmp_db.Name;
                    done.Add(request, _s);
                }
            }
            else
            {
                _s = done[request];
            }
            string _data = String.Empty;
                x.DownloadDataCompleted += (sender, e) =>
                {
                    _data = Encoding.ASCII.GetString(e.Result);
                };

               

                title = Regex.Match(_s, @"\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>",
        RegexOptions.IgnoreCase).Groups["Title"].Value;
                if (title != String.Empty)
                {
                // if (!names.Contains(title))
                names = title;
                }
                else
                {
                    var _tmp = stringBuilder.ToString().Split(new char[] { '/' });

                    //   if (!names.Contains(_tmp.Last()))
                    names=_tmp.Last();
                }

            return names;
        }


        //public List<string> GetRequestType(List<string> str)
        //{

        //}

        public void ParseRequest(string request,out string Type,out string url)
        {
            Type = string.Empty;
            url = string.Empty;
            
                var tmp = request.Split(new char[] { ' ' });
                var _request = tmp[0].Remove(0, 1);
            Type = _request;
            url = tmp[1];
            

        }
        public void GetResultAndDataVolume(string str, out int results,out long volume)
        {

            results = 0;
            volume = 0;
           
                var tmp = str.Split(new char[] { '"' })[2];

                int _result;
                long _volume;
                var _tmp = tmp.Split(new char[] { ' ' });
                for (int j = 0; j < _tmp.Length; j++)
                {
                    switch (j)
                    {
                        case 1:
                            int.TryParse(_tmp[j], out _result);
                            results=_result;
                            break;
                        case 2:
                            long.TryParse(_tmp[j], out _volume);
                            volume=_volume;
                            break;
                        default:
                            break;
                    }


                }


            


        }


        public async Task<string> GetCompaniesName(byte[] ip)
        {
            
            string company = string.Empty;
            string _ip = Encoding.ASCII.GetString(ip);
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            httpClientHandler.ServerCertificateCustomValidationCallback= (sender, cert, chain, sslPolicyErrors) => { return true; };
            using var cl = new HttpClient(httpClientHandler);
            var api = new IpInfoApi("530f45ab84efc1", cl);

            var r = await api.GetOrganizationByIpAsync(_ip);

            // ;


            company = r;
            return company ;
            }
           
        


        public async Task<string> GetRequests(string all)
        {
            Regex regex1 = new Regex("\"[^\"]*\"");
            string vs = string.Empty;

            await Task.Run(() =>
            {
                
                    var tmp_r = regex1.Match(all);
                    if (tmp_r.Success)
                    {
                    vs= tmp_r.Value;
                    }
                
            });

            return vs;
        }



    }
}
