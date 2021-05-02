using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace WebApplication25.Models
{
    public class MainTable
    {
        public long Id { get; set; }
        public virtual IPinfo _IPinfo { get; set; }
        public virtual FilesInfo FilesInfo { get; set; }  
        public DateTime DateTime { get; set; }
        public string DateTimeLog { get; set; }
        public string RequestType { get; set; }
        public int RequestResult { get; set; }

        public long DataVolume { get; set; }
    }

    public class FilesInfo
    {
        public long Id { get; set; }
        public string Path { get; set; }
        public string Name { get; set; }
        public long DataVolume { get; set; }
        public List<MainTable> MainTables { get; set; }
    }
    public class IPinfo
    {
        public long Id { get; set; }
        [Column(TypeName = "varbinary(16)")]
        public byte[] IPAddress { get; set; }
        public string CompanyName { get; set; }
        public List<MainTable> Requests { get; set; }

    }

    public class AppDbContext:DbContext
    {
        public DbSet<MainTable> MainTable { get; set; }
        public DbSet<IPinfo> IpInfo { get; set; }
        public DbSet<FilesInfo> FilesInfos { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
              : base(options)
        {
            
        }

    }
}
