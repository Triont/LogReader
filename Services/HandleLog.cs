using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication25.Models;
using System.IO;
using IpInfo;
using System.Text.RegularExpressions;
using System.Text;
using System.Net;
using System.Net.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace WebApplication25.Services
{

    public class LogElement
    {
        public FilesInfo FilesInfo { get; set; } = new FilesInfo();
        public IPinfo IpInfo { get; set; } = new IPinfo();
        public MainTable MainTable { get; set; } = new MainTable();
    }
    public class HandleLog
    {
        private readonly AppDbContext appDbContext;
        private ILogger<HandleLog> Logger;
        public HandleLog(AppDbContext appDbContext, ILogger<HandleLog> logger)
        {
            this.appDbContext = appDbContext;
            this.Logger = logger;
        }
        public string IP { get; set; }

        public string Text { get; set; }


        public string[] lines { get; set; }
        public List<LogElement> LogElements { get; set; } = new List<LogElement>();
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
                          // allowed.Add(lines[i]);
                        }
                    }
                }

                allowed.Add(lines[0]);
                allowed.Add(lines[1]);
                allowed.Add(lines[4593]);
                allowed.Add(lines[249]);
                allowed.Add(lines[359]);
                allowed.Add(lines[499]); allowed.Add(lines[5]);
                allowed.Add(lines[1540]);
                allowed.Add(lines[888]); allowed.Add(lines[89]);
                allowed.Add(lines[90]);
                allowed.Add(lines[493]);
             //   !var _allowed = allowed[0];
                var data_time_logged = await GetDateTimesString(allowed);
                var ip_addresses = await GetAllIp(allowed);
                var datetimes_list = await _GetDateTimes(data_time_logged);
                List<string> based_requests = await GetRequests(allowed);


                List<string> request_types;
                List<string> urls;

                ParseRequest(based_requests, out request_types, out urls);


                //  var t=    await GetNames(urls);
                //   var a=await appDbContext.FilesInfos.ToDictionaryAsync(x => x.Path, x => x.Name);
                //  var af=   await GetNames(urls);
         //       var Names = await GetNames(urls, new Dictionary<string, string>());

           //     List<string> CompaniesName = await GetCompaniesName(ip_addresses);


              
            
                List<byte[]> byte_arr_ipaddr = new List<byte[]>();
                for(int i=0; i<ip_addresses.Count;i++)
                {
                    byte_arr_ipaddr.Add(Encoding.ASCII.GetBytes(ip_addresses[i]));
                }
                var q = Task.Run(() => GetNames(urls, new Dictionary<string, string>()));
                var qqq = Task.Run(() => GetCompaniesName(ip_addresses));
                List<Task<List<string>>> lst_task = new List<Task<List<string>>>() { q, qqq };
                List<int> _result;
                List<long> _dataVolume;
                GetResultAndDataVolume(allowed,out _result,out _dataVolume);

                

                if (allowed.Count== _result.Count && allowed.Count==byte_arr_ipaddr.Count )
                {

                }
                var tempDataListIp = await appDbContext.IpInfo.ToListAsync();
                var tempDataListFiles = await appDbContext.FilesInfos.ToListAsync();
                //if (q.IsCompleted && qqq.IsCompleted)
           await     Task.WhenAll(lst_task).ContinueWith(async(o) =>
                {

                    for (int _i = 0; _i < allowed.Count; _i++)
                    {

                        var _m_count = appDbContext.MainTable.Count();
                        MainTable _main = new MainTable();
                        if (_m_count != 0)
                        {
                            var _ip = await appDbContext.IpInfo.FirstOrDefaultAsync(i => i.IPAddress == byte_arr_ipaddr[_i]);
                            var _file = await appDbContext.FilesInfos.FirstOrDefaultAsync(i => i.Path == urls[_i]);

                            // var _ip =  tempDataListIp.FirstOrDefault(i => i.IPAddress == byte_arr_ipaddr[_i]);
                            // var _file =  tempDataListFiles.FirstOrDefault(i => i.Path == urls[_i]);


                            if (_file == null && _ip == null)
                            {
                                await appDbContext.MainTable.AddAsync(new MainTable()
                                {
                                    DateTime = datetimes_list[_i],
                                    DataVolume = _dataVolume[_i],
                                    RequestResult = _result[_i],
                                    RequestType = request_types[_i],
                                    DateTimeLog = data_time_logged[_i],

                                    _IPinfo =

                              new IPinfo()
                              {
                                  IPAddress = byte_arr_ipaddr[_i],
                                  CompanyName = qqq.Result[_i]
                              },


                                    FilesInfo = new FilesInfo()
                                    {
                                        DataVolume = _dataVolume[_i],
                                        Name = q.Result[_i],
                                        Path = urls[_i]
                                    }


                                }
                              );
                            }


                            else if (_file != null && _ip == null)
                            {
                                await appDbContext.MainTable.AddAsync(new MainTable()
                                {
                                    DateTime = datetimes_list[_i],
                                    DataVolume = _dataVolume[_i],
                                    RequestResult = _result[_i],
                                    RequestType = request_types[_i],
                                    DateTimeLog = data_time_logged[_i],

                                    _IPinfo =

                               new IPinfo()
                               {
                                   IPAddress = byte_arr_ipaddr[_i],
                                   CompanyName = qqq.Result[_i]
                               },


                                    FilesInfo = _file ?? new FilesInfo()
                                    {
                                        DataVolume = _dataVolume[_i],
                                        Name = q.Result[_i],
                                        Path = urls[_i]
                                    }


                                }
                               );

                            }
                            else if (_file == null && _ip != null)
                            {
                                await appDbContext.MainTable.AddAsync(new MainTable()
                                {
                                    DateTime = datetimes_list[_i],
                                    DataVolume = _dataVolume[_i],
                                    RequestResult = _result[_i],
                                    RequestType = request_types[_i],
                                    DateTimeLog = data_time_logged[_i],

                                    _IPinfo = _ip ??

                              new IPinfo()
                              {
                                  IPAddress = byte_arr_ipaddr[_i],
                                  CompanyName = qqq.Result[_i]
                              },


                                    FilesInfo = new FilesInfo()
                                    {
                                        DataVolume = _dataVolume[_i],
                                        Name = q.Result[_i],
                                        Path = urls[_i]
                                    }


                                }
                              );
                            }
                            else if (_file != null && _ip != null)
                            {

                                if (!appDbContext.MainTable.Any(i => ((i.DataVolume == _dataVolume[_i])
                                   && (i.DateTime == datetimes_list[_i]) && (i.DateTimeLog == data_time_logged[_i])
                                   && (i.RequestResult == _result[_i]) && (i.RequestType == request_types[_i])
                                   && (i._IPinfo.Id == _ip.Id)) && (i.FilesInfo.Id == _file.Id)
                               ))
                                {
                                    await appDbContext.MainTable.AddAsync(new MainTable()
                                    {
                                        DateTime = datetimes_list[_i],
                                        DataVolume = _dataVolume[_i],
                                        RequestResult = _result[_i],
                                        RequestType = request_types[_i],
                                        DateTimeLog = data_time_logged[_i],

                                        _IPinfo = _ip ??

                                           new IPinfo()
                                           {
                                               IPAddress = byte_arr_ipaddr[_i],
                                               CompanyName = qqq.Result[_i]
                                           },


                                        FilesInfo = _file ?? new FilesInfo()
                                        {
                                            DataVolume = _dataVolume[_i],
                                            Name = q.Result[_i],
                                            Path = urls[_i]
                                        }


                                    }
                                           );
                                }
                                // _main = await appDbContext.MainTable?.FirstOrDefaultAsync(i => ((i.DataVolume == _dataVolume[_i])
                                //   && (i.DateTime == datetimes_list[_i] )&& (i.DateTimeLog == data_time_logged[_i])
                                //   && (i.RequestResult == _result[_i]) && (i.RequestType == request_types[_i])
                                //   && (i._IPinfo.Id == _ip.Id) &&( i.FilesInfo.Id == _file.Id)
                                //) );


                                //if (_main == null)
                                //{

                                //    await appDbContext.MainTable.AddAsync(new MainTable()
                                //    {
                                //        DateTime = datetimes_list[_i],
                                //        DataVolume = _dataVolume[_i],
                                //        RequestResult = _result[_i],
                                //        RequestType = request_types[_i],
                                //        DateTimeLog = data_time_logged[_i],

                                //        _IPinfo = _ip ??

                                //           new IPinfo()
                                //           {
                                //               IPAddress = byte_arr_ipaddr[_i],
                                //               CompanyName = CompaniesName[_i]
                                //           },


                                //        FilesInfo = _file ?? new FilesInfo()
                                //        {
                                //            DataVolume = _dataVolume[_i],
                                //            Name = Names[_i],
                                //            Path = urls[_i]
                                //        }


                                //    }
                                //           );
                                //}
                            }

                        }
                        else
                        {


                            await appDbContext.MainTable.AddAsync(new MainTable()
                            {
                                DateTime = datetimes_list[_i],
                                DataVolume = _dataVolume[_i],
                                RequestResult = _result[_i],
                                RequestType = request_types[_i],
                                DateTimeLog = data_time_logged[_i],

                                _IPinfo =

                                   new IPinfo()
                                   {
                                       IPAddress = byte_arr_ipaddr[_i],
                                       CompanyName = qqq.Result[_i]
                                   },


                                FilesInfo = new FilesInfo()
                                {
                                    DataVolume = _dataVolume[_i],
                                    Name = q.Result[_i],
                                    Path = urls[_i]
                                }


                            }
                                   );



                        }
                        // if (appDbContext.IpInfo.Where(i=>i.IPAddress==byte_arr_ipaddr[0]).Any())
                        // {
                        await appDbContext.SaveChangesAsync();


                    }

                    await appDbContext.SaveChangesAsync();
                });
                await appDbContext.SaveChangesAsync();
                // GetDateTimesString();
                var fullText = await File.ReadAllTextAsync(path);


                //Regex ip = new Regex(@"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b");
                //MatchCollection result = ip.Matches(fullText);
                //for (int i = 0; i < fullText.Length; i++)
                //{
                //    MainTable mainTable = new();
                //    FilesInfo filesInfo = new();
                //    IPinfo ip_info = new();
                //    //    ip_info.IPAddress=result[]



                //}

            }
            return await Task.Run(() => false);
        }





        public async Task<List<byte[]>> GetAllIp(string[] lines)
        {
            Regex ip = new Regex(@"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b");
            List<byte[]> vs = new List<byte[]>();
            for (int i = 0; i < lines.Length; i++)
            {
                var tmp = ip.Match(lines[i]).Value;
                var byte_val = Encoding.ASCII.GetBytes(tmp);
                vs.Add(byte_val);
            }

            return await Task.Run(() => vs);



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
                    var val= tmp.Value;
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
        public async Task<byte[]> GetIp(string line)
        {
            throw new Exception();
        }

        public async Task< List<string>> GetDateTimesString(string[] lines)
        {
            List<string> tmp = new List<string>();
            Regex regex = new Regex(@"\[([^[\]]+)\]");
           await Task.Run(() =>
            {
                for (int i = 0; i < lines.Length; i++)
                {
                    var m = regex.Match(lines[i]);
                    tmp.Add(m.Value);
                }
            });
           
            return tmp;

        }


        public async Task<List<string>> GetDateTimesString(List<string> lines)
        {
            List<string> tmp = new List<string>();
            Regex regex = new Regex(@"\[([^[\]]+)\]");

           await  Task.Run(() =>
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


        public async Task< List<DateTime>> GetDateTimes(List<string> str)
        {
            List<DateTime> dateTimes = new List<DateTime>();

         await   Task.Run(() => {


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
                    var _minutes = string.Empty;

                    if ((time_zone[time_zone.Length - 1] == 0) && (time_zone[time_zone.Length - 2] == 0))
                    {
                        r = time_zone.TrimEnd('0');
                    }

                    else
                    {
                        _minutes = time_zone.Substring(2, 2);
                    }
                    string h = r.TrimStart('0');
                    double min_v;
                    double hours;
                    TimeSpan time_m = new TimeSpan();
                    if (_minutes != string.Empty)
                    {
                        Double.TryParse(_minutes, out min_v);
                        time_m = TimeSpan.FromMinutes(min_v);
                    }
                    Double.TryParse(h, out hours);
                    var res_date = new DateTime();

                    switch (sign)
                    {
                        case '-':
                            res_date = dateTime1.Subtract(TimeSpan.FromHours(hours)).Subtract(TimeSpan.FromMinutes(time_m.TotalMinutes));
                            break;
                        case '+':
                            res_date = dateTime1.Add(TimeSpan.FromHours(hours)).Add(TimeSpan.FromMinutes(time_m.TotalMinutes));

                            break;
                        default:
                            break;
                    }
                    dateTimes.Add(res_date);
                    //add condition for minutes add and get hours and minutes;
                    var _t = time_zone.TrimStart('0');

                    var dq = dateTime1.Subtract(TimeSpan.FromHours(5));//-5 timezone

                }
            });
            
            return dateTimes;
        }

        public async Task< List<DateTime>> _GetDateTimes(List<string> str)
        {
            List<DateTime> dateTimes = new List<DateTime>();
          await  Task.Run(() =>
            {
                for (int i = 0; i < str.Count; i++)
                {
                    var tmp_str0 = str[i].Remove(0, 1);
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
                    dateTimes.Add(ResultDateTime);



                }
            });
           
            return dateTimes;
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
                using(MyClient myClient= new MyClient())
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
                //if (!done.ContainsKey(requests[i]))
                //{
                //    try
                //    {
                //        _s = await x.DownloadStringTaskAsync(stringBuilder.ToString());
                //    }
                //    catch (WebException e)
                //    {
                //        Logger.LogError($"{e.Message}, {e.Response}");
                //    }
                //}
                //else
                //{
                //    _s = done[requests[i]];
                //}
                string _data = String.Empty;
                x.DownloadDataCompleted += (sender, e) =>
                {
                    _data = Encoding.ASCII.GetString(e.Result);
                };

                //    x.DownloadStringAsync(new Uri(stringBuilder.ToString()));
                // string source = x.DownloadString(stringBuilder.ToString());
                //await Task.Run(() =>
                //{
                //    try {
                //        _data = x.DownloadString(stringBuilder.ToString());
                //    }
                //    catch (WebException e)
                //    {
                //        Logger.LogError($"{e.Message}, {e.Response}");
                //    }

                //});
        
                string title = Regex.Match(_s, @"\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>",
        RegexOptions.IgnoreCase).Groups["Title"].Value;
                if (title != String.Empty)
                {
                 //   if (!names.Contains(title))
                    names.Add(title); 
                }
                else
                {
                  var _tmp=  stringBuilder.ToString().Split(new char[] { '/' });

                   // if (!names.Contains(_tmp.Last()))
                     names.Add(_tmp.Last()); 
                }
            }
            return names;
        }

        public async Task<Tuple<List<string>, Dictionary<string, string>>> GetNames(List<string> requests, string url  = "http://tariscope.com")
        {
            Dictionary<string, string> done = new Dictionary<string, string>();

            List<string> names = new List<string>();
            for (int i = 0; i < requests.Count; i++)
            {
                StringBuilder stringBuilder = new StringBuilder(url);
                stringBuilder.Append(requests[i]);

                WebClient x = new WebClient();
                x.Proxy = null;
                string _s = string.Empty;
                string title = string.Empty;
                if (!done.ContainsKey(requests[i]))
                {
                    try
                    {
                        _s = await x.DownloadStringTaskAsync(stringBuilder.ToString());
                         title = Regex.Match(_s, @"\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>",
       RegexOptions.IgnoreCase).Groups["Title"].Value;

                        if (title != string.Empty)
                        {
                            done.Add(requests[i], title);
                        }
                        else
                        {
                            var __tmp = title.Split(new char[] { '/' }).Last();
                            done.Add(requests[i], __tmp);
                        }
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
                string _data = String.Empty;
                x.DownloadDataCompleted += (sender, e) =>
                {
                    _data = Encoding.ASCII.GetString(e.Result);
                };

                //    x.DownloadStringAsync(new Uri(stringBuilder.ToString()));
                // string source = x.DownloadString(stringBuilder.ToString());
                //await Task.Run(() =>
                //{
                //    try {
                //        _data = x.DownloadString(stringBuilder.ToString());
                //    }
                //    catch (WebException e)
                //    {
                //        Logger.LogError($"{e.Message}, {e.Response}");
                //    }

                //});

                title = Regex.Match(_s, @"\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>",
        RegexOptions.IgnoreCase).Groups["Title"].Value;
                if (title != String.Empty)
                {
                   // if (!names.Contains(title))
                     names.Add(title); 
                }
                else
                {
                    var _tmp = stringBuilder.ToString().Split(new char[] { '/' });

                 //   if (!names.Contains(_tmp.Last()))
                    names.Add(_tmp.Last()); 
                }
            }
            return new  (names, done);
        }


        //public List<string> GetRequestType(List<string> str)
        //{

        //}

        public void ParseRequest(List<string> requests, out List<string> Type, out List<string> url)
        {
            Type = new List<string>();
            url = new List<string>();
         
            for (int i = 0; i < requests.Count; i++)
            {
                var tmp = requests[i].Split(new char[] { ' ' });
                var request = tmp[0].Remove(0, 1);
                Type.Add(request);
                url.Add(tmp[1]);
            }

        }
        public void GetResultAndDataVolume(List<string> str, out List<int> results, out List<long> volume)
        {

            results = new List<int>();
            volume = new List<long>();
            for (int i = 0; i < str.Count; i++)
            {
                var tmp = str[i].Split(new char[] { '"' })[2];

                int _result;
                long _volume;
                var _tmp = tmp.Split(new char[] { ' ' });
                for (int j = 0; j < _tmp.Length; j++)
                {
                    switch (j)
                    {
                        case 1:
                            int.TryParse(_tmp[j], out _result);
                            results.Add(_result);
                            break;
                        case 2:
                            long.TryParse(_tmp[j], out _volume);
                            volume.Add(_volume);
                            break;
                        default:
                            break;
                    }


                }


            }


        }


        public async Task<List<string>> GetCompaniesName(List<string> ip)
        {
            using var cl = new HttpClient();
            var api = new IpInfoApi("530f45ab84efc1",cl);
          
            List<string> companies = new List<string>();
            for (int i = 0; i < ip.Count; i++)
            {

                var r = await api.GetOrganizationByIpAsync(ip[i]);

                   // ;
                
                
               companies.Add(r);
            }
            return companies;
        }


        public async  Task< List<string>> GetRequests(List<string> all)
        {
            Regex regex1 = new Regex("\"[^\"]*\"");
            List<string> vs = new List<string>();

         await   Task.Run(() =>
            {
                for (int i = 0; i < all.Count; i++)
                {
                    var tmp_r = regex1.Match(all[i]);
                    if (tmp_r.Success)
                    {
                        vs.Add(tmp_r.Value);
                    }
                }
            });
          
            return vs;
        }

       



    }
    public class MyClient : WebClient
    {
        public bool HeadOnly { get; set; }
        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest req = base.GetWebRequest(address);
            if (HeadOnly && req.Method == "GET")
            {
                req.Method = "HEAD";
            }
            return req;
        }
    }

}
