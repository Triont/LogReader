using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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

                // await  handleLog.GetData(filePath);
                await handleLogParallel.GetData(filePath);



            }
            return RedirectToAction("Index");
        }


        public async Task<IActionResult> MainTable()
        {
            var _m = await appDbContext.MainTable.Include(i => i.FilesInfo).Include(s => s._IPinfo).ToListAsync();
            MainModelView mainModelView = new MainModelView() { MainTables = _m };
            return View(mainModelView);
        }
        public async Task<IActionResult> IpTable()
        {
            var m = await appDbContext.IpInfo.ToListAsync();
            var categories = m.Select(i => i.CompanyName).Distinct().ToList();
            var _m = new IpModelView() { IpData = m, Filters=categories };
            return View(_m);
        }
        public async Task<IActionResult> FilesTable()
        {
            var m = await appDbContext.FilesInfos.ToListAsync();
            var _m = new FilesModelView()
            {
                FilesInfos = m
            };
            return View(_m);
        }


        public async Task<IActionResult> SearchInMain(MainModelView mainModelView, string returnUrl="")
        {
            if(mainModelView.Search!=null)
            {
                long id;
                if (long.TryParse(mainModelView.Search, out id))
   
                        {
                    var q = await appDbContext.MainTable.Where(i => i.Id == id).ToListAsync();
                    MainModelView mainModelView1 = new MainModelView() { MainTables = q };
                    return View(mainModelView1);
                }
                else
                {
                    var tmp = await appDbContext.MainTable.Where(i => i.FilesInfo.Path.Contains(mainModelView.Search)).ToListAsync();
                 

                    var cmp = await appDbContext.MainTable.Where(i => i._IPinfo.CompanyName.Contains(mainModelView.Search)).ToListAsync();

                    tmp.AddRange(cmp);
                    MainModelView mainModelView1 = new MainModelView()
                    {
                        MainTables = tmp
                    };
                    return View(mainModelView1);
                }

            }
            if(Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
           
           
        }




        public async Task<IActionResult> SearchInIp(IpModelView _ipModelView, string returnUrl )
        {
            if (_ipModelView._search != null)
            {
                long id;
                if (long.TryParse(_ipModelView._search, out id))

                {
                    var q = await appDbContext.IpInfo.Where(i => i.Id == id).ToListAsync();
                    IpModelView mainModelView1 = new IpModelView() { IpData = q };
                    return View(mainModelView1);
                }
                else
                {
                  


                    var cmp = await appDbContext.IpInfo.Where(i => i.CompanyName.Contains(_ipModelView._search)).ToListAsync();

                  
                    IpModelView ipView = new IpModelView()
                    {
                        IpData = cmp
                    };
                    return View(ipView);
                }

            }
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }


        }
        public async Task<IActionResult> SearchInFiles(FilesModelView filesnModelView, string returnUrl = "")
        {
            if (filesnModelView._search != null)
            {
                long id;
                if (long.TryParse(filesnModelView._search, out id))

                {
                    var q = await appDbContext.MainTable.Where(i => i.Id == id).ToListAsync();
                    MainModelView mainModelView1 = new MainModelView() { MainTables = q };
                    return View(mainModelView1);
                }
                else
                {
                    var tmp = await appDbContext.FilesInfos.Where(i => i.Path.Contains(filesnModelView._search)).ToListAsync();


                    var cmp = await appDbContext.FilesInfos.Where(i => i.Name.Contains(filesnModelView._search)).ToListAsync();

                    tmp.AddRange(cmp);
                    FilesModelView  fileModelView1 = new FilesModelView()
                    {
                        FilesInfos = tmp
                    };
                    return View(filesnModelView);
                }

            }
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
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
    }
}
