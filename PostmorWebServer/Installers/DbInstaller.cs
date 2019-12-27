using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PostmorWebServer.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PostmorWebServer.Data;
using PostmorWebServer.Data.Entities;

namespace PostmorWebServer.Installers
{
    public class DbInstaller : IInstaller
    {
        public void InstallServices(IConfiguration configuration, IServiceCollection services)
        {
            services.AddDbContext<DataContext>(options =>
                 options.UseSqlServer(
            configuration.GetConnectionString("DefaultConnection")));
            services.AddDefaultIdentity<User>()
                .AddEntityFrameworkStores<DataContext>();
        }
    }
}
