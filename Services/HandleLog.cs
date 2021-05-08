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

using Abot2.Core;
using Abot2.Crawler;
using Abot2.Poco;
using Serilog;
using System.Collections.Concurrent;
using System.Threading;

namespace WebApplication25.Services
{

   
    public class HandleLog
    {
        private readonly AppDbContext appDbContext;
        private ILogger<HandleLog> Logger;
        public HandleLog(AppDbContext appDbContext, ILogger<HandleLog> logger)
        {
            this.appDbContext = appDbContext;
            this.Logger = logger;
        }


        public string[] lines { get; set; }
    
        public async Task<bool> GetData(string path)
        {
            Logger.LogInformation($"{DateTime.Now.ToString()} GetData method HandleLog class is started");
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
                        || lines[i].Contains("ico") ||lines[i].Contains("=opensearch")

                        ))
                    // if (lines[i].Contains("search"))
                    {
                        if (!(lines[i].Contains(" - admin ") || lines[i].Contains("/administrator ")))
                        {
                            allowed.Add(lines[i]);
                        }
                    }
                }

             
                var data_time_logged = await GetDateTimesString(allowed);
                var ip_addresses = await GetAllIp(allowed);
                var datetimes_list = await _GetDateTimes(data_time_logged);
                List<string> based_requests = await GetRequests(allowed);


                List<string> request_types;
                List<string> urls;

                ParseRequest(based_requests, out request_types, out urls);

                List<int> _result;
                List<long> _dataVolume;
                GetResultAndDataVolume(allowed, out _result, out _dataVolume);


                var Names = await GetNames(urls, new ConcurrentDictionary<string, string>(), _result);

               var CompaniesName = await GetCompaniesName(ip_addresses);




                List<byte[]> byte_arr_ipaddr = new List<byte[]>();
                for (int i = 0; i < ip_addresses.Count; i++)
                {
                    byte_arr_ipaddr.Add(Encoding.ASCII.GetBytes(ip_addresses[i]));
                }
           
             



                var tempDataListIp = await appDbContext.IpInfo.ToListAsync();
                var tempDataListFiles = await appDbContext.FilesInfos.ToListAsync();
               

                for (int _i = 0; _i < allowed.Count; _i++)
                {
                    Logger.LogInformation($"{_i} iteration of  data save loop started");
                    var _m_count = appDbContext.MainTable.Count();
                    MainTable _main = new MainTable();
                    if (_m_count != 0)
                    {
                        var _ip = await appDbContext.IpInfo.FirstOrDefaultAsync(i => i.IPAddress == byte_arr_ipaddr[_i]);
                        var _file = await appDbContext.FilesInfos.FirstOrDefaultAsync(i => i.Path == urls[_i]);

                        //if db not contains ip and file path
                        if (_file == null && _ip == null)
                        {
                            if (Names.ContainsKey(urls[_i]) && urls[_i] != string.Empty)
                            {


                                if (CompaniesName.ContainsKey(ip_addresses[_i]) && ip_addresses[_i] != string.Empty)
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
                                      // CompanyName = qqq.Result[_i]
                                      CompanyName = CompaniesName[ip_addresses[_i]]
                                  },


                                        FilesInfo = new FilesInfo()
                                        {
                                            DataVolume = _dataVolume[_i],
                                            Name = Names[urls[_i]],
                                            Path = urls[_i]
                                        }


                                    }

                                  );
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

                             //           _IPinfo =

                             //new IPinfo()
                             //{
                             //    IPAddress = byte_arr_ipaddr[_i],
                             //         // CompanyName = qqq.Result[_i]
                             //         CompanyName = CompaniesName[ip_addresses[_i]]
                             //},


                                        FilesInfo = new FilesInfo()
                                        {
                                            DataVolume = _dataVolume[_i],
                                            Name = Names[urls[_i]],
                                            Path = urls[_i]
                                        }


                                    }

                             );
                                }

                                
                            }



                            else
                            {

                                if (CompaniesName.ContainsKey(ip_addresses[_i]) && ip_addresses[_i] != string.Empty)
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
                                  // CompanyName = qqq.Result[_i]
                                  CompanyName = CompaniesName[ip_addresses[_i]]
                                  },


                                        //FilesInfo = new FilesInfo()
                                        //{
                                        //    DataVolume = _dataVolume[_i],
                                        //    Name = Names[urls[_i]],
                                        //    Path = urls[_i]
                                        //}


                                    });
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

                                //        _IPinfo =

                                //new IPinfo()
                                //{
                                //    IPAddress = byte_arr_ipaddr[_i],
                                //      // CompanyName = qqq.Result[_i]
                                //      CompanyName = CompaniesName[ip_addresses[_i]]
                                //},


                                        //FilesInfo = new FilesInfo()
                                        //{
                                        //    DataVolume = _dataVolume[_i],
                                        //    Name = Names[urls[_i]],
                                        //    Path = urls[_i]
                                        //}


                                    });
                                }


                            }
                        }


                        else if (_file != null && _ip == null)
                        {
                            if (Names.ContainsKey(urls[_i]) && urls[_i]!=string.Empty)
                            {
                                if (CompaniesName.ContainsKey(ip_addresses[_i])&& ip_addresses[_i]!=string.Empty)
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
                                   CompanyName = CompaniesName[ip_addresses[_i]]
                               },


                                        FilesInfo = _file ?? new FilesInfo()
                                        {
                                            DataVolume = _dataVolume[_i],
                                            Name = Names[urls[_i]],
                                            Path = urls[_i]
                                        }


                                    }
                               );
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

                              //          _IPinfo =

                              //new IPinfo()
                              //{
                              //    IPAddress = byte_arr_ipaddr[_i],
                              //    CompanyName = CompaniesName[ip_addresses[_i]]
                              //},


                                        FilesInfo = _file ?? new FilesInfo()
                                        {
                                            DataVolume = _dataVolume[_i],
                                            Name = Names[urls[_i]],
                                            Path = urls[_i]
                                        }


                                    }
                              );
                                }
                            }

                            else
                            {
                                if (CompaniesName.ContainsKey(ip_addresses[_i]) && ip_addresses[_i] != string.Empty)
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
                                   CompanyName = CompaniesName[ip_addresses[_i]]
                               },


                                        //FilesInfo = _file ?? new FilesInfo()
                                        //{
                                        //    DataVolume = _dataVolume[_i],
                                        //    Name = Names[urls[_i]],
                                        //    Path = urls[_i]
                                        //}


                                    }
                               );
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

                                        //          _IPinfo =

                                        //new IPinfo()
                                        //{
                                        //    IPAddress = byte_arr_ipaddr[_i],
                                        //    CompanyName = CompaniesName[ip_addresses[_i]]
                                        //},


                                        //FilesInfo = _file ?? new FilesInfo()
                                        //{
                                        //    DataVolume = _dataVolume[_i],
                                        //    Name = Names[urls[_i]],
                                        //    Path = urls[_i]
                                        //}


                                    }
                              );
                                }

                            }

                        }
                        else if (_file == null && _ip != null)
                        {
                            if (Names.ContainsKey(urls[_i]) && urls[_i]!=string.Empty)
                            {
                                if (CompaniesName.ContainsKey(ip_addresses[_i]) && ip_addresses[_i]!=string.Empty)
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
                                  CompanyName = CompaniesName[ip_addresses[_i]]
                              },


                                        FilesInfo = new FilesInfo()
                                        {
                                            DataVolume = _dataVolume[_i],
                                            Name = Names[urls[_i]],
                                            Path = urls[_i]
                                        }


                                    }
                              );
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

                              //          _IPinfo = _ip ??

                              //new IPinfo()
                              //{
                              //    IPAddress = byte_arr_ipaddr[_i],
                              //    CompanyName = CompaniesName[ip_addresses[_i]]
                              //},


                                        FilesInfo = new FilesInfo()
                                        {
                                            DataVolume = _dataVolume[_i],
                                            Name = Names[urls[_i]],
                                            Path = urls[_i]
                                        }


                                    }
                              );
                                }
                            }

                            else
                            {
                                if (CompaniesName.ContainsKey(ip_addresses[_i]) && ip_addresses[_i] != string.Empty)
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
                                  CompanyName = CompaniesName[ip_addresses[_i]]
                              },


                                        //FilesInfo = new FilesInfo()
                                        //{
                                        //    DataVolume = _dataVolume[_i],
                                        //    Name = Names[urls[_i]],
                                        //    Path = urls[_i]
                                        //}


                                    }
                              );
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

                                        //          _IPinfo = _ip ??

                                        //new IPinfo()
                                        //{
                                        //    IPAddress = byte_arr_ipaddr[_i],
                                        //    CompanyName = CompaniesName[ip_addresses[_i]]
                                        //},


                                        //FilesInfo = new FilesInfo()
                                        //{
                                        //    DataVolume = _dataVolume[_i],
                                        //    Name = Names[urls[_i]],
                                        //    Path = urls[_i]
                                        //}


                                    }
                              );
                                }

                            }
                        }
                        else if (_file != null && _ip != null)
                        {
                            if (Names.ContainsKey(urls[_i]) && urls[_i]!=string.Empty)
                            {
                                if (CompaniesName.ContainsKey(ip_addresses[_i])&& ip_addresses[_i]!=string.Empty )
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
                                                   CompanyName = CompaniesName[ip_addresses[_i]]
                                               },


                                            FilesInfo = _file ?? new FilesInfo()
                                            {
                                                DataVolume = _dataVolume[_i],
                                                Name = Names[urls[_i]],
                                                Path = urls[_i]
                                            }


                                        }
                                               );
                                    }
                                }

                                else
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

                                            //_IPinfo = _ip ??

                                            //   new IPinfo()
                                            //   {
                                            //       IPAddress = byte_arr_ipaddr[_i],
                                            //       CompanyName = CompaniesName[ip_addresses[_i]]
                                            //   },


                                            FilesInfo = _file ?? new FilesInfo()
                                            {
                                                DataVolume = _dataVolume[_i],
                                                Name = Names[urls[_i]],
                                                Path = urls[_i]
                                            }


                                        }
                                               );
                                    }
                                }
                            }

                            else
                            {


                                if (CompaniesName.ContainsKey(ip_addresses[_i]) && ip_addresses[_i] != string.Empty)
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
                                                   CompanyName = CompaniesName[ip_addresses[_i]]
                                               },


                                            //FilesInfo = _file ?? new FilesInfo()
                                            //{
                                            //    DataVolume = _dataVolume[_i],
                                            //    Name = Names[urls[_i]],
                                            //    Path = urls[_i]
                                            //}


                                        }
                                               );
                                    }
                                }

                                else
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

                                            //_IPinfo = _ip ??

                                            //   new IPinfo()
                                            //   {
                                            //       IPAddress = byte_arr_ipaddr[_i],
                                            //       CompanyName = CompaniesName[ip_addresses[_i]]
                                            //   },


                                            //FilesInfo = _file ?? new FilesInfo()
                                            //{
                                            //    DataVolume = _dataVolume[_i],
                                            //    Name = Names[urls[_i]],
                                            //    Path = urls[_i]
                                            //}


                                        }
                                               );
                                    }
                                }

                            }
                          
                        }

                    }
                    else
                    {
                        if (Names.ContainsKey(urls[_i]) && urls[_i]!=string.Empty)
                        {
                            if (CompaniesName.ContainsKey(ip_addresses[_i])&& ip_addresses[_i]!=string.Empty)
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
                                       CompanyName = CompaniesName[ip_addresses[_i]]
                                   },


                                    FilesInfo = new FilesInfo()
                                    {
                                        DataVolume = _dataVolume[_i],
                                        Name = Names[urls[_i]],
                                        Path = urls[_i]
                                    }


                                }
                                   );
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

                                //    _IPinfo =

                                //new IPinfo()
                                //{
                                //    IPAddress = byte_arr_ipaddr[_i],
                                //    CompanyName = CompaniesName[ip_addresses[_i]]
                                //},


                                    FilesInfo = new FilesInfo()
                                    {
                                        DataVolume = _dataVolume[_i],
                                        Name = Names[urls[_i]],
                                        Path = urls[_i]
                                    }


                                }
                                );
                            }
                        }
                        else
                        {
                            if (CompaniesName.ContainsKey(ip_addresses[_i]) && ip_addresses[_i] != string.Empty)
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
                                       CompanyName = CompaniesName[ip_addresses[_i]]
                                   },


                                    //FilesInfo = new FilesInfo()
                                    //{
                                    //    DataVolume = _dataVolume[_i],
                                    //    Name = Names[urls[_i]],
                                    //    Path = urls[_i]
                                    //}


                                }
                                   );
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

                                    //    _IPinfo =

                                    //new IPinfo()
                                    //{
                                    //    IPAddress = byte_arr_ipaddr[_i],
                                    //    CompanyName = CompaniesName[ip_addresses[_i]]
                                    //},


                                    //FilesInfo = new FilesInfo()
                                    //{
                                    //    DataVolume = _dataVolume[_i],
                                    //    Name = Names[urls[_i]],
                                    //    Path = urls[_i]
                                    //}


                                }
                                );
                            }
                        }


                    }
                   
                    await appDbContext.SaveChangesAsync();
                    Logger.LogInformation($"{DateTime.Now.ToString()} data is saved to db");

                }

                await appDbContext.SaveChangesAsync();
            }
                await appDbContext.SaveChangesAsync();
            Logger.LogInformation($"{DateTime.Now.ToString()} GetData method HandleLog class is stoped");
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
            Logger.LogInformation($"{DateTime.Now.ToString()} GetAllIp method HandleLog class is started");
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
            Logger.LogInformation($"{DateTime.Now.ToString()} GetAllIp method HandleLog class is stoped");
            return await Task.Run(() => vs);



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
            Logger.LogInformation($"{DateTime.Now.ToString()} GetDateTimesString method HandleLog class is started");
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
            Logger.LogInformation($"{DateTime.Now.ToString()} GetDateTimesString method HandleLog class is stoped");
            return tmp;

        }


     

        public async Task< List<DateTime>> _GetDateTimes(List<string> str)
        {
            Logger.LogInformation($"{DateTime.Now.ToString()} _GetDateTimes method HandleLog class is started");
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
            Logger.LogInformation($"{DateTime.Now.ToString()} _GetDateTimes method HandleLog class is stoped");
            return dateTimes;
        }
     
        public async Task<ConcurrentDictionary<string, string>> GetNames(List<string> requests, ConcurrentDictionary<string, string> done, List<int> resutl_request, string url = "http://tariscope.com")
        {
            Logger.LogInformation($"{DateTime.Now.ToString()} GetNames method HandleLog class is started");
            List<string> names = new List<string>();
            //for (int i = 0; i < requests.Count; i++)
            //{
            //    StringBuilder stringBuilder = new StringBuilder(url);
            //    stringBuilder.Append(requests[i]);

            //    WebClient x = new WebClient();
            //    x.Proxy = null;
            //    string _s = string.Empty;
            var crw = new PoliteWebCrawler();
           // crw.PageCrawlCompleted += (q);

            HttpClient httpClient = new HttpClient();
            BlockingCollection<HttpClient> webClients = new BlockingCollection<HttpClient>() { new HttpClient(), new HttpClient(), new HttpClient(),

                new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(), 
                new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(),

                new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(),
                new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(),

                new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(),
                new HttpClient(),new HttpClient(), new HttpClient(), new HttpClient(),

                new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(),
                new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(),

                new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(),
                new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(),

                new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(),
                new HttpClient(),new HttpClient(), new HttpClient(), new HttpClient(),

                new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(),
                new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(),

                new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(),
                new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(),

                new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(),
                new HttpClient(),new HttpClient(), new HttpClient(), new HttpClient(),

                new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(),
                new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(),

                new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(),
                new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(),

                new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(),
                new HttpClient(),new HttpClient(), new HttpClient(), new HttpClient(),

                new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(),
                new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(),

                new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(),
                new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(),

                new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(),
                new HttpClient(),new HttpClient(), new HttpClient(), new HttpClient(),

                new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(),
                new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(),

                new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(),
                new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(),

                new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(),
                new HttpClient(),new HttpClient(), new HttpClient(), new HttpClient(),

                new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(),
                new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(),

                new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(),
                new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(),

                new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(),
                new HttpClient(),new HttpClient(), new HttpClient(), new HttpClient(),

                new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(),
                new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(),

                new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(),
                new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(),

                new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(),
                new HttpClient(),new HttpClient(), new HttpClient(), new HttpClient(),

                new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(),
                new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(),

                new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(),
                new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(),

                new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(),
                new HttpClient()

            };
            BlockingCollection<string> vs = new BlockingCollection<string>();
            foreach(var s in requests)
            {
                vs.Add(s);
            }
           // while (vs.Count > 0)
           // {
           //     var wc = webClients.Take();
           //     var r = vs.Take();
           //     if (!done.ContainsKey(r))
           //     {

                  
           //         done.TryAdd(r, Regex.Match(await wc.GetStringAsync(new Uri(url + r)), @"\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>",
           //RegexOptions.IgnoreCase).Groups["Title"].Value );
           //         webClients.Add(new HttpClient());
           //        // w += (s, e) =>
           //        //{
           //        ////  done.Add(r, e.Result);
           //        //webClients.Add(new WebClient());
           //        //};
           //     }
           // }
            List<Task> threads = new List<Task>() {
                Task.Run(async() =>
                {
                      while (vs.Count > 0)
                    {
                            Logger.LogInformation($"{Task.CurrentId} task run and alredy get {done.Count} elements");
                        var wc = webClients.Take();
                var r = vs.Take();
                if (!done.ContainsKey(r))
                {


                    done.TryAdd(r, Regex.Match(await wc.GetStringAsync(new Uri(url + r)), @"\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>",
           RegexOptions.IgnoreCase).Groups["Title"].Value );
                    webClients.Add(new HttpClient());
                        // wc.DownloadStringCompleted+=(s, e)=>
                    } }
                }),
               Task.Run(async() =>
                {
                        Logger.LogInformation($"{Task.CurrentId} task run and alredy get {done.Count} elements");
                      while (vs.Count > 0)
                    { var wc = webClients.Take();
                var r = vs.Take();
                if (!done.ContainsKey(r))
                {


                    done.TryAdd(r, Regex.Match(await wc.GetStringAsync(new Uri(url + r)), @"\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>",
           RegexOptions.IgnoreCase).Groups["Title"].Value );
                    webClients.Add(new HttpClient());
                        // wc.DownloadStringCompleted+=(s, e)=>
                    } }
                        webClients.Add(new HttpClient());
                }),
              Task.Run(async () =>
                {
                        Logger.LogInformation($"{Task.CurrentId} task run and alredy get {done.Count} elements");

                   while (vs.Count > 0)
                    {
                            Logger.LogInformation($"{Task.CurrentId} task run and alredy get {done.Count} elements");
                        var wc = webClients.Take();
                var r = vs.Take();
                if (!done.ContainsKey(r))
                {


                    done.TryAdd(r, Regex.Match(await wc.GetStringAsync(new Uri(url + r)), @"\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>",
           RegexOptions.IgnoreCase).Groups["Title"].Value );
                    webClients.Add(new HttpClient());
                        // wc.DownloadStringCompleted+=(s, e)=>
                    }

                      webClients.Add(new HttpClient());
                    }

                }),
               // var _third=
                  Task.Run(async () =>
                {
                        Logger.LogInformation($"{Task.CurrentId} task run and alredy get {done.Count} elements");
                    while (vs.Count > 0)
                    {
                            Logger.LogInformation($"{Task.CurrentId} task run and alredy get {done.Count} elements");
                        var wc = webClients.Take();
                var r = vs.Take();
                if (!done.ContainsKey(r))
                {


                    done.TryAdd(r, Regex.Match(await wc.GetStringAsync(new Uri(url + r)), @"\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>",
           RegexOptions.IgnoreCase).Groups["Title"].Value );
                    webClients.Add(new HttpClient());
                        // wc.DownloadStringCompleted+=(s, e)=>
                    }

                      webClients.Add(new HttpClient());
                    }

                }),
             //   var _fourth=
               Task.Run(async () =>
                {
                        Logger.LogInformation($"{Task.CurrentId} task run and alredy get {done.Count} elements");
                    while (vs.Count > 0)
                    {
                            Logger.LogInformation($"{Task.CurrentId} task run and alredy get {done.Count} elements");
                        var wc = webClients.Take();
                var r = vs.Take();
                if (!done.ContainsKey(r))
                {


                    done.TryAdd(r, Regex.Match(await wc.GetStringAsync(new Uri(url + r)), @"\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>",
           RegexOptions.IgnoreCase).Groups["Title"].Value );
                    webClients.Add(new HttpClient());
                        // wc.DownloadStringCompleted+=(s, e)=>
                    }
                      webClients.Add(new HttpClient());
                    }

                }),
             //   var _fn=
                  Task.Run(async () =>
                {
                        Logger.LogInformation($"{Task.CurrentId} task run and alredy get {done.Count} elements");

                     while (vs.Count > 0)
                    {
                            Logger.LogInformation($"{Task.CurrentId} task run and alredy get {done.Count} elements");
                        var wc = webClients.Take();
                var r = vs.Take();
                if (!done.ContainsKey(r))
                {


                    done.TryAdd(r, Regex.Match(await wc.GetStringAsync(new Uri(url + r)), @"\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>",
           RegexOptions.IgnoreCase).Groups["Title"].Value );
                    webClients.Add(new HttpClient());
                        // wc.DownloadStringCompleted+=(s, e)=>
                    }
                      webClients.Add(new HttpClient());
                    }

                }),
              //  var _six=
                  Task.Run(async () =>
                {

                     while (vs.Count > 0)
                    {
                        Logger.LogInformation($"{Task.CurrentId} task run and alredy get {done.Count} elements");
                        var wc = webClients.Take();
                var r = vs.Take();
                if (!done.ContainsKey(r))
                {


                    done.TryAdd(r, Regex.Match(await wc.GetStringAsync(new Uri(url + r)), @"\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>",
           RegexOptions.IgnoreCase).Groups["Title"].Value );
                    webClients.Add(new HttpClient());
                        // wc.DownloadStringCompleted+=(s, e)=>
                    }

                      webClients.Add(new HttpClient());
                    }

                }),
              //  var _seven=
                Task.Run(async () =>
                {

                      while (vs.Count > 0)
                    {
                            Logger.LogInformation($"{Task.CurrentId} task run and alredy get {done.Count} elements");
                        var wc = webClients.Take();
                var r = vs.Take();
                if (!done.ContainsKey(r))
                {


                    done.TryAdd(r, Regex.Match(await wc.GetStringAsync(new Uri(url + r)), @"\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>",
           RegexOptions.IgnoreCase).Groups["Title"].Value );
                    webClients.Add(new HttpClient());
                               webClients.Add(new HttpClient());

                        // wc.DownloadStringCompleted+=(s, e)=>
                    }

                      webClients.Add(new HttpClient());
                    }

                }),
            //    var eight=
                 Task.Run(async () =>
                {

                     while (vs.Count > 0)
                    {
                            Logger.LogInformation($"{Task.CurrentId} task run and alredy get {done.Count} elements");
                        var wc = webClients.Take();
                var r = vs.Take();
                if (!done.ContainsKey(r))
                {


                    done.TryAdd(r, Regex.Match(await wc.GetStringAsync(new Uri(url + r)), @"\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>",
           RegexOptions.IgnoreCase).Groups["Title"].Value );
                    webClients.Add(new HttpClient());
                               webClients.Add(new HttpClient());
                        // wc.DownloadStringCompleted+=(s, e)=>
                    }

                      webClients.Add(new HttpClient());
                    }

                }),
                //var nine=
                  Task.Run(async () =>
                {

                      while (vs.Count > 0)
                    {
                            Logger.LogInformation($"{Task.CurrentId} task run and alredy get {done.Count} elements");
                        var wc = webClients.Take();
                var r = vs.Take();
                if (!done.ContainsKey(r))
                {


                    done.TryAdd(r, Regex.Match(await wc.GetStringAsync(new Uri(url + r)), @"\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>",
           RegexOptions.IgnoreCase).Groups["Title"].Value );
                    webClients.Add(new HttpClient());
                               webClients.Add(new HttpClient());
                        // wc.DownloadStringCompleted+=(s, e)=>
                    }
                      webClients.Add(new HttpClient());
                    }

                }),
              //  var ten=
                  Task.Run(async () =>
                {

                    while (vs.Count > 0)
                    {
                            Logger.LogInformation($"{Task.CurrentId} task run and alredy get {done.Count} elements");
                        var wc = webClients.Take();
                var r = vs.Take();
                if (!done.ContainsKey(r))
                {


                    done.TryAdd(r, Regex.Match(await wc.GetStringAsync(new Uri(url + r)), @"\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>",
           RegexOptions.IgnoreCase).Groups["Title"].Value );
                    webClients.Add(new HttpClient());
                               webClients.Add(new HttpClient());
                        // wc.DownloadStringCompleted+=(s, e)=>
                    }

                      webClients.Add(new HttpClient());
                    }

                }),
                //var eleven=
                 Task.Run(async () =>
                {

                    while (vs.Count > 0)
                    {
                            Logger.LogInformation($"{Task.CurrentId} task run and alredy get {done.Count} elements");
                        var wc = webClients.Take();
                var r = vs.Take();
                if (!done.ContainsKey(r))
                {


                    done.TryAdd(r, Regex.Match(await wc.GetStringAsync(new Uri(url + r)), @"\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>",
           RegexOptions.IgnoreCase).Groups["Title"].Value );
                    webClients.Add(new HttpClient());
                               webClients.Add(new HttpClient());
                        // wc.DownloadStringCompleted+=(s, e)=>
                    }
                      webClients.Add(new HttpClient());
                    }

                })
                };


            //var tasks = new List<Task<IEnumerable<string>>>();
            //var batchSize = 400;
            //int numberOfBatches = (int)Math.Ceiling((double)requests.Count() / batchSize);
            //var tmp_list = new List<String>();
            //for(int i=0;i<numberOfBatches;i++)
            //{
            //    Logger.LogInformation($"{i} number of batches start");
            //    try
            //    {
            //      var curr = requests.Skip(i * batchSize).Take(batchSize);
            //    var tasks = curr.Select(d => httpClient.GetStringAsync(url+d));
            //    tmp_list.AddRange(await Task.WhenAll(tasks));
            //    }

            //    catch(HttpRequestException e)
            //    {
            //        Logger.LogError($"{e.Message} {e.InnerException} {e.Data}");
            //    }
            //    //tasks.Add(httpClient.GetStringAsync(curr.));
            //    Logger.LogInformation($"{i} number of batches finished");
            //}

            //var tasks = requests.Select(i => httpClient.GetStringAsync(url+i));
            await Task.WhenAll(threads).ContinueWith((t) => { return done; });

            Logger.LogInformation($"{DateTime.Now} GetNames method finished");

            return done;
            //using(MyClient myClient= new MyClient())
            //{
            //    myClient.HeadOnly = true;
            //    if (!done.ContainsKey(requests[i]))
            //    {
            //        var db_data = await appDbContext.MainTable.FirstOrDefaultAsync(qq => qq.FilesInfo.Path == requests[i]);
            //        if (db_data == null)
            //        {
            //            try
            //            {
            //                if (resutl_request[i] == 200)
            //                {


            //                    _s = await x.DownloadStringTaskAsync(stringBuilder.ToString());
            //                }
            //                else
            //                {
            //                    _s = string.Empty;
            //                }
            //            }
            //            catch (WebException e)
            //            {
            //                Logger.LogError($"{e.Message}, {e.Response}");
            //            }
            //        }
            //        else
            //        {
            //            _s = db_data.FilesInfo.Name;
            //            done.Add(requests[i], _s);
            //        }
            //    }
            //    else
            //    {
            //        _s = done[requests[i]];
            //    }

            //}

            //string _data = String.Empty;
            //x.DownloadDataCompleted += (sender, e) =>
            //{
            //    _data = Encoding.ASCII.GetString(e.Result);
            //};

            //foreach(var t in threads )
            //{
            //    t.Start();
            //}
            //    await Task.WhenAll(threads);

            //Dictionary<string, string> res = new Dictionary<string, string>();
            //    for (int i = 0; i < done.Count; i++)
            //    {
            //    done.ElementAt(i)

            //        string title = Regex.Match(done.ElementAt(i).Value, @"\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>",
            //RegexOptions.IgnoreCase).Groups["Title"].Value;
            //        //if (title != String.Empty)
            //        //{
            //        //   if (!names.Contains(title))
            //        names.Add(title);
            //        //}
            //else
            //{
            //    var _tmp = stringBuilder.ToString().Split(new char[] { '/' });

            //    // if (!names.Contains(_tmp.Last()))
            //    names.Add(_tmp.Last());
            //}
            // }


            //  Logger.LogInformation($"{DateTime.Now.ToString()} GetNames method HandleLog class is started");
            //return done;
        }

        public async Task<Tuple<List<string>, Dictionary<string, string>>> GetNames(List<string> requests, string url  = "http://tariscope.com")
        {
            Logger.LogInformation($"{DateTime.Now.ToString()} GetNames method HandleLog class is started");
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
                    //check if db contains this request
                    var _dbinfo = await appDbContext.FilesInfos.FirstOrDefaultAsync(p => p.Path == requests[i]);
                    if (_dbinfo == null)
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
                        _s = _dbinfo.Name;
                        done.Add(requests[i], _s);
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

            
          
                title = Regex.Match(_s, @"\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>",
        RegexOptions.IgnoreCase).Groups["Title"].Value;
                if (title != String.Empty)
                {
             
                     names.Add(title); 
                }
                else
                {
                   
                    var _tmp = stringBuilder.ToString().Split(new char[] { '/' });

                    names.Add(_tmp.Last()); 
                }
            }
            Logger.LogInformation($"{DateTime.Now.ToString()} GetNames method HandleLog class is started");
            return new  (names, done);
        }


        public void ParseRequest(List<string> requests, out List<string> Type, out List<string> url)
        {
            Logger.LogInformation($"{DateTime.Now.ToString()} ParseRequest method HandleLog class is started");
            Type = new List<string>();
            url = new List<string>();
         
            for (int i = 0; i < requests.Count; i++)
            {
                var tmp = requests[i].Split(new char[] { ' ' });
                var request = tmp[0].Remove(0, 1);
                Type.Add(request);
                url.Add(tmp[1]);
            }
            Logger.LogInformation($"{DateTime.Now.ToString()} ParseRequest method HandleLog class is stoped");

        }
        public void GetResultAndDataVolume(List<string> str, out List<int> results, out List<long> volume)
        {
            Logger.LogInformation($"{DateTime.Now.ToString()} GetREsultAndDataVolume method HandleLog class is started");
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
            Logger.LogInformation($"{DateTime.Now.ToString()} GetResultAndDataVolume method HandleLog class is stoped");

        }


        public async Task<ConcurrentDictionary<string, string>> GetCompaniesName(List<string> ip)
        {
            Logger.LogInformation($"{DateTime.Now.ToString()} GetCompaniesNames method HandleLog class is started");
            using var cl0 = new HttpClient();
            using var cl1 = new HttpClient();
            using var cl2 = new HttpClient();
            using var cl3 = new HttpClient();
            using var cl4 = new HttpClient();
            using var cl5 = new HttpClient();
            using var cl6 = new HttpClient();
            using var cl7 = new HttpClient();
            using var cl8 = new HttpClient();
            using var cl9 = new HttpClient();
            using var cl10 = new HttpClient();
            using var cl11 = new HttpClient();
            using var cl12= new HttpClient();
            using var cl13 = new HttpClient();
            using var cl14= new HttpClient();
            using var cl15= new HttpClient();
            using var cl16 = new HttpClient();
            using var cl17 = new HttpClient();
            using var cl18 = new HttpClient();
            using var cl19 = new HttpClient();
            using var cl20 = new HttpClient();
            using var cl21= new HttpClient();
            using var cl22 = new HttpClient();

            ConcurrentDictionary<string, string> keyValuePairs = new ConcurrentDictionary<string, string>();
            BlockingCollection<HttpClient> httpClients = new BlockingCollection<HttpClient>()
            {
                new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(), new HttpClient(),
                new HttpClient(), new HttpClient(), new HttpClient()
            };
            BlockingCollection<string> _ip = new BlockingCollection<string>();
            for(int i=0; i<ip.Count;i++)
            {
                _ip.Add(ip[i]);
            }
            BlockingCollection<IpInfoApi> ipInfoApis = new BlockingCollection<IpInfoApi>()
            {
                new IpInfoApi("e233289eabbf1b", cl0),  new IpInfoApi("e233289eabbf1b", cl1), new IpInfoApi("e233289eabbf1b", cl2), new IpInfoApi("e233289eabbf1b", cl3), new IpInfoApi("e233289eabbf1b", cl4), new IpInfoApi("e233289eabbf1b", cl5),
                 new IpInfoApi("e233289eabbf1b", cl6), new IpInfoApi("e233289eabbf1b", cl7), new IpInfoApi("e233289eabbf1b", cl8), new IpInfoApi("e233289eabbf1b", cl9), new IpInfoApi("e233289eabbf1b", cl10),
                
            };

            List<Task> tasks = new List<Task>()
            {
                Task.Run(async() =>
                {
                    while(_ip.Count>0)
                    {
                        var q=ipInfoApis.Take();
                        var ipAddr=_ip.Take();
                        keyValuePairs.TryAdd(ipAddr, await q.GetOrganizationByIpAsync(ipAddr));
                        ipInfoApis.Add(new IpInfoApi("e233289eabbf1b", cl0));
                        
                    }

                }),
                Task.Run(async() =>
                {

                    while(_ip.Count>0)
                    {
                        var q=ipInfoApis.Take();
                        var ipAddr=_ip.Take();
                        keyValuePairs.TryAdd(ipAddr, await q.GetOrganizationByIpAsync(ipAddr));
                        ipInfoApis.Add(new IpInfoApi("e233289eabbf1b", cl1));

                    }

                }),
                 Task.Run(async() =>
                {
                    while(_ip.Count>0)
                    {
                        var q=ipInfoApis.Take();
                        var ipAddr=_ip.Take();
                        keyValuePairs.TryAdd(ipAddr, await q.GetOrganizationByIpAsync(ipAddr));
                        ipInfoApis.Add(new IpInfoApi("e233289eabbf1b", cl2));

                    }

                }),
                Task.Run(async() =>
                {

                    while(_ip.Count>0)
                    {
                        var q=ipInfoApis.Take();
                        var ipAddr=_ip.Take();
                        keyValuePairs.TryAdd(ipAddr, await q.GetOrganizationByIpAsync(ipAddr));
                        ipInfoApis.Add(new IpInfoApi("e233289eabbf1b", cl3));

                    }

                }),
                 Task.Run(async() =>
                {
                    while(_ip.Count>0)
                    {
                        var q=ipInfoApis.Take();
                        var ipAddr=_ip.Take();
                        keyValuePairs.TryAdd(ipAddr, await q.GetOrganizationByIpAsync(ipAddr));
                        ipInfoApis.Add(new IpInfoApi("e233289eabbf1b", cl9));

                    }

                }),
                Task.Run(async() =>
                {

                    while(_ip.Count>0)
                    {
                        var q=ipInfoApis.Take();
                        var ipAddr=_ip.Take();
                        keyValuePairs.TryAdd(ipAddr, await q.GetOrganizationByIpAsync(ipAddr));
                        ipInfoApis.Add(new IpInfoApi("e233289eabbf1b", cl20));

                    }

                })

            };


          await  Task.WhenAll(tasks).ContinueWith((t) => keyValuePairs);
         //   return keyValuePairs;
                //token on ipinfo.io, monthly limit has already been used
            //var api = new IpInfoApi("e233289eabbf1b", cl);
          
            //List<string> companies = new List<string>();
            //for (int i = 0; i < ip.Count; i++)
            //{
            //    //check if db contains such data
            //    var _dbdata = await appDbContext.IpInfo.FirstOrDefaultAsync(t => t.IPAddress == Encoding.ASCII.GetBytes(ip[i]));
            //    if (_dbdata == null)
            //    {



            //        var r = await api.GetOrganizationByIpAsync(ip[i]);




            //        companies.Add(r);
            //    }
            //    else
            //    {
            //        companies.Add(_dbdata.CompanyName);
            //    }
            //}
            Logger.LogInformation($"{DateTime.Now.ToString()} GetCompaniesName method HandleLog class is stoped");
            return keyValuePairs;
        }


        public async  Task< List<string>> GetRequests(List<string> all)
        {
            Logger.LogInformation($"{DateTime.Now.ToString()} GetRequests method HandleLog class is started");
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
            Logger.LogInformation($"{DateTime.Now.ToString()} GetRequests method HandleLog class is stoped");
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
