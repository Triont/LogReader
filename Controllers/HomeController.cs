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
using WebApplication25.Services;

namespace WebApplication25.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext appDbContext;
        private readonly IWebHostEnvironment hostingEnvironment;
        private readonly HandleLog handleLog;
        private readonly HandleLogParallel handleLogParallel;
        public HomeController(ILogger<HomeController> logger, AppDbContext appDbContext,
            IWebHostEnvironment webHostEnvironment, HandleLog handleLog, HandleLogParallel handleLogParallel)
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

                 await Task.Run(()=>  handleLog.GetData(filePath));
            
                
                
                //await handleLogParallel.GetData(filePath);



            }
            return RedirectToAction("Index");
        }


        public async Task<IActionResult> MainTable()
        {
            if (TempData["MainData"] != null)
            {
                
                    return View(JsonConvert.DeserializeObject<MainModelView>(TempData["MainData"].ToString()));
                
            }
            else if(TempData["FilteredMainData"]!=null)
            {
                string tmp = TempData["FilteredMainData"].ToString();
                var m = JsonConvert.DeserializeObject<MainModelView>(tmp);
             
                return View(m);
                
            }

            else
            {
                var _m = await appDbContext.MainTable.ToListAsync();
                var f = _m.Select(i => i.RequestType).Distinct().ToList();
                MainModelView mainModelView = new MainModelView() { MainTables = _m, Filters=f };
                return View(mainModelView);
            }
        }
        public async Task<IActionResult> IpTable()
        {

          
          
            if (TempData["Ip_data"]!=null)
            {
                 var tmp = JsonConvert.DeserializeObject<IpModelView>(TempData["Ip_data"].ToString());
                            if(tmp!=null)
                            {

                    var _c = tmp.IpData.Select(i => i.CompanyName).Distinct().ToList();
                    tmp.Filters = _c;
                                return View(tmp);
                            }
            }

            if(TempData["FilteredIp"]!=null)
            {
                var tmp = JsonConvert.DeserializeObject<IpModelView>(TempData["FilteredIp"].ToString());
                return View(tmp);
            }
           
            var m = await appDbContext.IpInfo.ToListAsync();
           var categories = m.Select(i => i.CompanyName).Distinct().ToList();
            var _m = new IpModelView() { IpData = m, Filters=categories };
            return View(_m);
        }
        public async Task<IActionResult> FilesTable()
        {
            if(TempData["FilesData"]!=null)
            {
             var tmp=   JsonConvert.DeserializeObject<FilesModelView>(TempData["FilesData"].ToString());
              var names=  tmp.FilesInfos.Select(i => i.Name).Distinct().ToList();
                tmp.Filters = names;
                return View(tmp);
            }

            if(TempData["FilteredFiles"]!=null)
            {
                var _t = JsonConvert.DeserializeObject<FilesModelView>(TempData["FilteredFiles"].ToString());
                _t.Filters = await appDbContext.FilesInfos.Select(i => i.Name).Distinct().ToListAsync();
                return View(_t);
            }

            var m = await appDbContext.FilesInfos.ToListAsync();
            var _m = new FilesModelView()
            {
                FilesInfos = m, Filters= m.Select(i=>i.Name).Distinct().ToList()
            };
            return View(_m);
        }


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
                    MainModelView mainModelView1 = new MainModelView() { MainTables = q, Filters=filt };
                    TempData["MainData"] = JsonConvert.SerializeObject(mainModelView1);
                    return RedirectToAction("MainTable", "Home");
                }
                else
                {
                    var tmp = await appDbContext.MainTable.Where(i => i.FilesInfo.Path.Contains(mainModelView.Search)).ToListAsync();
                 

                    var cmp = await appDbContext.MainTable.Where(i => i._IPinfo.CompanyName.Contains(mainModelView.Search)).ToListAsync();

                    tmp.AddRange(cmp);
                    var f = await appDbContext.MainTable.ToListAsync();
                    var filt = f.Select(i => i._IPinfo.CompanyName).Distinct().ToList();
                    MainModelView mainModelView1 = new MainModelView()
                    {
                        MainTables = tmp, Filters=filt
                    };
                    TempData["MainData"] = JsonConvert.SerializeObject(mainModelView1, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                    return RedirectToAction("MainTable", "Home");
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
                    string json_data = JsonConvert.SerializeObject(ipModel, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                    TempData["Ip_data"] = json_data;
                    return RedirectToAction("IpTable", "Home");
                }
                else 
                {
                  


                    var cmp = await appDbContext.IpInfo.Where(i => i.CompanyName.Contains(_ipModelView._search)).ToListAsync();

                  
                    IpModelView ipView = new IpModelView()
                    {
                        IpData = cmp
                    };
                    TempData["Ip_data"] = JsonConvert.SerializeObject(ipView, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                    return RedirectToAction("IpTable", "Home");
                }

            }
           
           
                return RedirectToAction("IpTable", "Home");
            


        }
        [HttpPost]
        public async Task<IActionResult> SearchInFiles(FilesModelView filesnModelView)
        {
            if (filesnModelView._search != null)
            {
                long id;
                if (long.TryParse(filesnModelView._search, out id))

                {
                    var q = await appDbContext.FilesInfos.Where(i => i.Id == id).ToListAsync();
                    FilesModelView mainModelView1 = new FilesModelView() { FilesInfos = q, Filters = await appDbContext.FilesInfos.Select(i=>i.Name).Distinct().ToListAsync() };
                    TempData["FilesData"] = JsonConvert.SerializeObject(mainModelView1);
                    return RedirectToAction("FilesTable", "Home");
                }
                else
                {
                    var tmp = await appDbContext.FilesInfos.Where(i => i.Path.Contains(filesnModelView._search)).ToListAsync();


                    var cmp = await appDbContext.FilesInfos.Where(i => i.Name.Contains(filesnModelView._search)).ToListAsync();

                    tmp.AddRange(cmp);
                    FilesModelView  fileModelView1 = new FilesModelView()
                    {
                        FilesInfos = tmp,
                        Filters = await appDbContext.FilesInfos.Select(i => i.Name).Distinct().ToListAsync()
                    };
                    TempData["FilesData"] = JsonConvert.SerializeObject(fileModelView1);

                    return RedirectToAction("FilesTable", "Home");
                }

            }
            
            else
            {
                return RedirectToAction("FilesTable", "Home");
            }


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


        public async Task<IActionResult> GetFilterdedIpData(string[] filters)
        {
            List<IPinfo> infos = new List<IPinfo>();
            List<string> _c = new List<string>();
           // var r = await appDbContext.IpInfo.ToListAsync();
     
            for (int i = 0; i < filters.Length; i++)
            {
                var r = await appDbContext.MainTable.Select(tt => tt._IPinfo).Where(q => q.CompanyName.Contains(filters[i])).ToListAsync();
                infos.AddRange(r);
               
                _c.Add(filters[i]);
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
    }
}
