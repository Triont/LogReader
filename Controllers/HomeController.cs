using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication25.Models;
using WebApplication25.ModelView;
using WebApplication25.Infrastructure;

using WebApplication25.Services;

namespace WebApplication25.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext appDbContext;
        private readonly IWebHostEnvironment hostingEnvironment;
        private readonly HandleLog handleLog;
        private readonly HandleLogByLine handleLogParallel;
        public HomeController(ILogger<HomeController> logger, AppDbContext appDbContext,
            IWebHostEnvironment webHostEnvironment, HandleLog handleLog, HandleLogByLine handleLogParallel)
        {
            _logger = logger;
            this.appDbContext = appDbContext;
            hostingEnvironment = webHostEnvironment;
            this.handleLog = handleLog;
            this.handleLogParallel = handleLogParallel;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public async Task< IActionResult> UploadFile(UploadLogModelView uploadLogModelView)
        {
            if (ModelState.IsValid)
            {
                if (uploadLogModelView.FormFile != null)
                {
                    var uniqueFileName = GetUniqueFileName(uploadLogModelView.FormFile.FileName);

                    var uploads = Path.Combine(hostingEnvironment.WebRootPath, "uploads");
                    if (!Directory.Exists(uploads))
                    {
                        Directory.CreateDirectory(uploads);
                    }
                    var filePath = Path.Combine(uploads, uniqueFileName);
                    FileStream fileStream = new FileStream(filePath, FileMode.Create);
                    uploadLogModelView.FormFile.CopyTo(fileStream);
                    fileStream.Close();
                    await appDbContext.UploadedFiles.AddAsync(new UploadedFilesInfo() { Path = filePath, WasRead = false });
                    await appDbContext.SaveChangesAsync();

                    //processing data by column
                  await Task.Run(() => handleLog.GetData(filePath));


                    //processing data by line
               //   await handleLogParallel.GetData(filePath);



                }
            }
            return RedirectToAction("Index");
        }

        //method for show maintable and get data based on search and filters info
        public async Task<IActionResult> MainTable(MainModelView mainModelView, string []filters)
        {
            _logger.LogInformation($"{DateTime.Now.ToString()} MainTable method HomeController class");
            //do search
            if(mainModelView.Search!=null)
            {
                long id;
                //try get long value from search string
                if (long.TryParse(mainModelView.Search, out id))

                {
                    var q = await appDbContext.MainTable.Where(i => i.Id == id).ToListAsync();
                    var f = await appDbContext.MainTable.ToListAsync();
                    var filt = f.Select(i => i._IPinfo.CompanyName).Distinct().ToList();
                    MainModelView mainModelView1 = new MainModelView() { MainTables = q, Filters=filt };

                    return View(mainModelView1);
                }
                //else do search in path and company names
                else if (mainModelView.Search.Length != 0)
                {
                   var tmp = await appDbContext.MainTable.Where(i => i.FilesInfo.Path.Contains(mainModelView.Search)).ToListAsync();


                    var cmp = await appDbContext.MainTable.Where(i => i._IPinfo.CompanyName.Contains(mainModelView.Search)).ToListAsync();

                    tmp.AddRange(cmp);
                       var f = await appDbContext.MainTable.ToListAsync();
                      var filt = f.Select(i => i._IPinfo.CompanyName).Distinct().ToList();
                    MainModelView mainModelView11 = new MainModelView()
                    {
                        MainTables = tmp, Filters=filt
                    };
                  
                    return View(mainModelView11);
                  
                }

            }
            //do filter
            if(filters!=null)
            {
                List<MainTable> infos = new List<MainTable>();
                List<string> _check = new List<string>();

                for (int i = 0; i < filters.Length; i++)
                {
                    string tmp = filters[i].Remove(filters[i].Length-2, 2);
                      var r = await appDbContext.MainTable.Where(k => k._IPinfo.CompanyName.Contains(tmp)).ToListAsync();
             
                    
                   infos.AddRange(r);
                    _check.Add(filters[i]);
                }

                foreach(var i in appDbContext.IpInfo.ToList())
                {
                    this._logger.LogInformation($"{i.CompanyName} \n");
                }
                if (filters.Length == 0)
                {
                    infos = await appDbContext.MainTable.ToListAsync();
                }
                MainModelView mainModelView123 = new MainModelView()
                {
                    CheckedItems = _check,
                    Filters = await appDbContext.MainTable.Select(i =>
    i._IPinfo.CompanyName).Distinct().ToListAsync(),

                    //                  Filters = await appDbContext.MainTable.Select(i =>
                    //i.RequestType).Distinct().ToListAsync(),
                    MainTables = infos
                };

                return View(mainModelView123);

            }
            
                var _m = await appDbContext.MainTable.ToListAsync();
                var fty = _m.Select(i => i._IPinfo.CompanyName).Distinct().ToList();
                MainModelView mainModelView145 = new MainModelView() { MainTables = _m, Filters=fty };
                return View(mainModelView145);
            
        }
        //method for show iptable and get data based on search and filters info
        public async Task<IActionResult> IpTable(IpModelView ipModelView, string []filters)
        {
            _logger.LogInformation($"{DateTime.Now.ToString()} IpTable method HomeController class");
            var m = await appDbContext.IpInfo.ToListAsync();
            var categories = m.Select(i => i.CompanyName).Distinct().ToList();


         

            //do search

            if (ipModelView._search != null)
            {
                long id;
                if (long.TryParse(ipModelView._search, out id))

                {
                    var q = await appDbContext.IpInfo.Where(i => i.Id == id).ToListAsync();

                    IpModelView ipModel = new IpModelView() { IpData = q , Filters=categories};
                     return View(ipModel);
                }
                else
                {



                    var cmp = await appDbContext.IpInfo.Where(i => i.CompanyName.Contains(ipModelView._search)).ToListAsync();


                    IpModelView _ipView = new IpModelView()
                    {
                        IpData = cmp, Filters=categories
                    };
                               return View(_ipView);
                }

            }
            //do filter
            if(filters!=null)
            {
                List<IPinfo> infos = new List<IPinfo>();
                List<string> _c = new List<string>();
            

               
                    for (int i = 0; i < filters.Length; i++)
                    {

                    string tmp = filters[i].Remove(filters[i].Length - 2, 2);
                    var r = await appDbContext.IpInfo.Where(k => k.CompanyName.Contains(tmp)).ToListAsync();

                    infos.AddRange(r);
                   
         

                    _c.Add(filters[i]);
                    }
                
                if (filters.Length == 0)
                {
                    infos = await appDbContext.IpInfo.ToListAsync();
                }
                IpModelView __ipModelView = new IpModelView()
                {
                    CheckedItems = _c,
                    Filters = await appDbContext.IpInfo.Select(i => i.CompanyName).Distinct().ToListAsync(),
                    IpData = infos
                };
                return View(__ipModelView);
            }

         
            var _m = new IpModelView() { IpData = m, Filters=categories };
            return View(_m);
        }
        //method for show filestable and get data based on search and filters info
        public async Task<IActionResult> FilesTable( FilesModelView filesModelView, string [] filters)
        {
            _logger.LogInformation($"{DateTime.Now.ToString()} FilesTable method HomeController class");

            //do search
            if (filesModelView._search != null)
            {
                long id;
                if (long.TryParse(filesModelView._search, out id))

                {
                    var q = await appDbContext.FilesInfos.Where(i => i.Id == id).ToListAsync();
                    FilesModelView mainModelView1 = new FilesModelView()
                    {
                        FilesInfos = q,
                         Filters = await appDbContext.FilesInfos.Select(i=>i.Name).Distinct().ToListAsync()
                    };
                    return View(mainModelView1);
                }
                else
                {
                     var tmp = await appDbContext.FilesInfos.Where(i => i.Path.Contains(filesModelView._search)).ToListAsync();


                    var cmp = await appDbContext.FilesInfos.Where(i => i.Name.Contains(filesModelView._search)).ToListAsync();

                    tmp.AddRange(cmp);
                    FilesModelView fileModelView = new FilesModelView()
                    {
                        FilesInfos = tmp,
                           Filters = await appDbContext.FilesInfos.Select(i => i.Name).Distinct().ToListAsync()
                    };
                    //       TempData["FilesData"] = JsonConvert.SerializeObject(fileModelView1);

                    return View(filesModelView);
                }

            }
            //do filter
            if(filters!=null)
            {
                List<FilesInfo> infos = new List<FilesInfo>();
                List<string> check = new List<string>();
                for (int i = 0; i < filters.Length; i++)
                {
                    var r = await appDbContext.FilesInfos.Where(k => k.Name.Contains(filters[i])).ToListAsync();
                    infos.AddRange(r);
                    check.Add(filters[i]);
                }
                if (filters.Length == 0)
                {
                    infos = await appDbContext.FilesInfos.ToListAsync();
                }
                FilesModelView f_ilesModelView = new FilesModelView();
                f_ilesModelView.CheckedItems = check;
                f_ilesModelView.FilesInfos = infos;
                f_ilesModelView.Filters = await appDbContext.FilesInfos.Select(i => i.Name).Distinct().ToListAsync();


                return View(f_ilesModelView);
            }

            var m = await appDbContext.FilesInfos.ToListAsync();
            var _m = new FilesModelView()
            {
                FilesInfos = m, Filters= m.Select(i=>i.Name).Distinct().ToList()
            };
            return View(_m);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private string GetUniqueFileName(string fileName)
        {
            fileName = Path.GetFileName(fileName);
            return Path.GetFileNameWithoutExtension(fileName)
                      + "_"
                      + Guid.NewGuid().ToString().Substring(0, 10)
                      + Path.GetExtension(fileName);
        }





        #region old code
        public async Task<IActionResult> SearchInMain(MainModelView mainModelView)
        {
            if(mainModelView.Search!=null)
            {
                long id;
                if (long.TryParse(mainModelView.Search, out id))
   
                        {
                    var q = await appDbContext.MainTable.Where(i => i.Id == id).ToListAsync();
                    var f = await appDbContext.MainTable.ToListAsync();
                  var filt=  f.Select(i => i._IPinfo.CompanyName).Distinct().ToList();
                    MainModelView mainModelView1 = new MainModelView() { MainTables = q };
                TempData["MainData"] = JsonConvert.SerializeObject(mainModelView1);
                    return RedirectToAction("MainTable", "Home");
                }
                else if(mainModelView.Search.Length!=0)
                {
                    //var tmp = await appDbContext.MainTable.Where(i => i.FilesInfo.Path.Contains(mainModelView.Search)).ToListAsync();
                 

                    var cmp = await appDbContext.MainTable.Where(i => i._IPinfo.CompanyName.Contains(mainModelView.Search)).ToListAsync();

                    //tmp.AddRange(cmp);
                 //   var f = await appDbContext.MainTable.ToListAsync();
                  //  var filt = f.Select(i => i._IPinfo.CompanyName).Distinct().ToList();
                    MainModelView mainModelView11 = new MainModelView()
                    {
                        MainTables = cmp, 
                    };
                    string str= JsonConvert.SerializeObject(cmp);
                    byte[] arr = Encoding.ASCII.GetBytes(str);
                   var tq= StringCompressor.CompressString(str);
                //    string h = "hello";
                  TempData["MainData"] = tq;
                    
                    //return RedirectToAction("MainTable", "Home");
                }

            }
           
           
                return RedirectToAction("MainTable", "Home");
            
           
           
        }




        public async Task<IActionResult> SearchInIp(IpModelView _ipModelView )
        {
            if (_ipModelView._search != null)
            {
                long id;
                if (long.TryParse(_ipModelView._search, out id))

                {
                    var q = await appDbContext.IpInfo.Where(i => i.Id == id).ToListAsync();
                    
                    IpModelView ipModel = new IpModelView() { IpData = q };
                    return RedirectToAction("IpTable", "Home");
                }
                else 
                {
                  


                    var cmp = await appDbContext.IpInfo.Where(i => i.CompanyName.Contains(_ipModelView._search)).ToListAsync();

                  
                    IpModelView ipView = new IpModelView()
                    {
                        IpData = cmp
                    };
                      return RedirectToAction("IpTable", "Home");
                }

            }
           
           
                return RedirectToAction("IpTable", "Home");
            


        }
     
        public async Task<IActionResult> SearchInFiles(FilesModelView filesnModelView)
        {
            if (filesnModelView._search != null)
            {
                long id;
                if (long.TryParse(filesnModelView._search, out id))

                {
                    var q = await appDbContext.FilesInfos.Where(i => i.Id == id).ToListAsync();
                    FilesModelView mainModelView1 = new FilesModelView() { FilesInfos = q,
                
                    };
                    TempData["FilesData"] = JsonConvert.SerializeObject(mainModelView1);
                    return RedirectToAction("FilesTable", "Home");
                }
                else
                {
                   


                    var cmp = await appDbContext.FilesInfos.Where(i => i.Name.Contains(filesnModelView._search)).ToListAsync();

                    //tmp.AddRange(cmp);
                    FilesModelView  fileModelView1 = new FilesModelView()
                    {
                        FilesInfos = cmp
                  
                    };
         
                    return RedirectToAction("FilesTable", "Home");
                }

            }
            
            else
            {
                return RedirectToAction("FilesTable", "Home");
            }


        }




        



        public async Task<IActionResult> GetFilterdedIpData(string[] filters)
        {
            List<IPinfo> infos = new List<IPinfo>();
            List<string> _c = new List<string>();
           var r = await appDbContext.IpInfo.ToListAsync();

            for (int qq = 0; qq < r.Count; qq++)
            {
                for (int i = 0; i < filters.Length; i++)
                {

                    if(r[qq].CompanyName.Contains(filters[i]))
                    {
                        infos.Add(r[qq]);
                    }
                    //var rr = await appDbContext.MainTable.Select(tt => tt._IPinfo).Where(q => q.CompanyName.Contains(filters[i])).ToListAsync();
                    //infos.AddRange(r);

                    _c.Add(filters[i]);
                }
            }
            if (filters.Length == 0)
            {
                infos = await appDbContext.IpInfo.ToListAsync();
            }
            IpModelView ipModelView = new IpModelView()
            {
                CheckedItems = _c,
                Filters = await appDbContext.IpInfo.Select(i => i.CompanyName).Distinct().ToListAsync(),
                IpData = infos
            };
            string tmp = JsonConvert.SerializeObject(ipModelView, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling=ReferenceLoopHandling.Ignore});
            TempData["FilteredIp"] = tmp;
            return RedirectToAction("IpTable", "Home");


        }

        public async Task<IActionResult> GetFilterdedFilesData(string[] filters)
        {
            List<FilesInfo> infos = new List<FilesInfo>();
            List<string> check = new List<string>();
            for (int i = 0; i < filters.Length; i++)
            {
                var r = await appDbContext.FilesInfos.Where(k => k.Name.Contains(filters[i])).ToListAsync();
                infos.AddRange(r);
                check.Add(filters[i]);
            }
            if (filters.Length == 0)
            {
                infos = await appDbContext.FilesInfos.ToListAsync();
            }
            FilesModelView filesModelView = new FilesModelView();
            filesModelView.CheckedItems = check;
            filesModelView.FilesInfos = infos;
            string tmp = JsonConvert.SerializeObject(filesModelView, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            TempData["FilteredFiles"] = tmp;
            return RedirectToAction("FilesTable", "Home");


        }

        public async Task<IActionResult> GetFilterdedMainData(string[] filters)
        {
            List<MainTable> infos = new List<MainTable>();
            List<string> _check = new List<string>();

            for (int i = 0; i < filters.Length; i++)
            {
                var r = await appDbContext.MainTable.Where(k => k.RequestType.Contains(filters[i])).ToListAsync();
                infos.AddRange(r);
                _check.Add(filters[i]);
            }
            if (filters.Length == 0)
            {
                infos = await appDbContext.MainTable.ToListAsync();
            }
            MainModelView mainModelView = new MainModelView()
            {
                CheckedItems = _check,
                Filters = await appDbContext.MainTable.Select(i =>
i._IPinfo.CompanyName).Distinct().ToListAsync(),
                MainTables = infos
            };

            string tmp = JsonConvert.SerializeObject(mainModelView, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            TempData["FilteredMainData"] = tmp;
            return RedirectToAction("MainTable", "Home");


        }
        #endregion
    }
}
