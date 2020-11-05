using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using QZ.Common;
using QZ.Model.DBContext;

namespace QZ.Interview
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            //注入EF上下文
            string Interview_Conn = string.Empty;
            if (QZ_Helper_Constant.isPwd == "1")
            {
                string str_Public_Key = QZ.Common.QZ_Helper_Constant.passWordKey;
                Interview_Conn = QZ.Common.QZ_Helper_Encryption.DES_Decode(QZ.Common.QZ_Helper_Constant.Interview_Conn, str_Public_Key);
            }
            else
            {
                Interview_Conn = QZ.Common.QZ_Helper_Constant.Interview_Conn;
            }
            services.AddDbContextPool<Interview_DB_EFContext>(options => options.UseSqlServer(Interview_Conn));

            services.AddApplicationInsightsTelemetry(Configuration);

            ContainerBuilder containerBuilder = new ContainerBuilder();

            containerBuilder.RegisterType<Interview_DB_EFContext>().As<DbContext>().InstancePerDependency();

            //集中注册服务
            foreach (var item in GetClassName("QZ.Service"))
            {
                foreach (var typeArray in item.Value)
                {
                    services.AddScoped(typeArray, item.Key);
                }
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.UseStatusCodePages();
        }

        /// <summary>  
        /// 获取程序集中的实现类对应的多个接口
        /// </summary>  
        /// <param name="assemblyName">程序集</param>
        private Dictionary<Type, Type[]> GetClassName(string assemblyName)
        {
            if (!String.IsNullOrEmpty(assemblyName))
            {
                Assembly assembly = Assembly.Load(assemblyName);
                List<Type> ts = assembly.GetTypes().ToList();

                var result = new Dictionary<Type, Type[]>();
                foreach (var item in ts.Where(s => !s.IsInterface && !s.IsAbstract))
                {
                    var interfaceType = item.GetInterfaces();
                    result.Add(item, interfaceType);
                }
                return result;
            }
            return new Dictionary<Type, Type[]>();
        }
    }
}
