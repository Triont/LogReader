using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using WebApplication25.Models;
using System.Threading;
using System.Threading.Tasks;
using WebApplication25.Services;
using Microsoft.EntityFrameworkCore;

namespace WebApplication25.Services
{
    public class HostService : IHostedService, IDisposable
    {

        private readonly ILogger <HostService>logger;
        private readonly IServiceScopeFactory serviceScopeFactory;
        private Timer _timer;
        public HostService(ILogger<HostService> logger, IServiceScopeFactory serviceScopeFactory)
        {
            this.logger = logger;
            this.serviceScopeFactory = serviceScopeFactory;
           

        }
        public void Dispose()
        {
            this._timer.Dispose();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Hosted service is started");
            await Task.Run(() =>
            {

                _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(120));
            });
        
        }
        public async void DoWork(object state)
        {
            await Task.Run(async () => {
                using (var scope = serviceScopeFactory.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    var handler = scope.ServiceProvider.GetRequiredService<HandleLog>();
                    var lst = await db.UploadedFiles.Where(u => u.WasRead !=true).ToListAsync();
                    foreach (var r in lst)
                    {
                        await handler.GetData(r.Path);
                        r.WasRead = true;
                        db.UploadedFiles.Update(r);
                    }
                    await db.SaveChangesAsync();

                }

            });
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("HostedService is Stopping");
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }
    }

    public class HostedBackground : BackgroundService
    {
        private readonly IServiceScopeFactory serviceScope;
        public HostedBackground(IServiceScopeFactory scopeFactory) 
        {
            this.serviceScope = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = serviceScope.CreateScope())
                {

                     Task.Run(async () =>
                    {
                        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                        var handler = scope.ServiceProvider.GetRequiredService<HandleLog>();
                        var lst = await db.UploadedFiles.Where(u => u.WasRead != true).ToListAsync();
                        foreach (var r in lst)
                        {
                            await handler.GetData(r.Path);
                            r.WasRead = true;
                            db.UploadedFiles.Update(r);
                        }
                        await db.SaveChangesAsync();
                    }).Wait();
                   await Task.Delay(5, stoppingToken);
                }
            }
        }
    }
}
