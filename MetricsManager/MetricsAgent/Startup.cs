using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Dapper;
using MetricsAgent.Repositories;
using MetricsAgent.Settings;
using Microsoft.OpenApi.Models;

namespace MetricsAgent
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
            ConfigureSqlLiteConnection(services);
            services.AddScoped<ICpuMetricsRepository, CpuMetricsRepository>();
            services.AddScoped<IDotNetMetricsRepository, DotNetMetricsRepository>();
            services.AddScoped<IHddMetricsRepository, HddMetricsRepository>();
            services.AddScoped<INetworkMetricsRepository, NetworkMetricsRepository>();
            services.AddScoped<IRamMetricsRepository, RamMetricsRepository>();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "MetricsAgent", Version = "v1" });
            });
            var mapperConfiguration = new MapperConfiguration(mp => mp.AddProfile(new MapperProfile()));
            var mapper = mapperConfiguration.CreateMapper();
            services.AddSingleton(mapper);
        }

        private void ConfigureSqlLiteConnection(IServiceCollection services)
        {
            string connectionString = "Data Source=:memory:";
            var connection = new SQLiteConnection(connectionString);
            connection.Open();
            PrepareSchema(connection);
            services.AddSingleton(connection);
        }

        private void PrepareSchema(SQLiteConnection connection)
        {
            // Создаем таблицу с метриками CPU
            using (var createTableConnection = new SQLiteConnection(connection))
            {
                // Создаем таблицус метриками CPU
                createTableConnection.Execute("DROP TABLE IF EXISTS cpumetrics");
                createTableConnection.Execute("CREATE TABLE cpumetrics(id INTEGER PRIMARY KEY, value INT, time INT)");

                // Создаем таблицу с метриками .Net
                createTableConnection.Execute("DROP TABLE IF EXISTS dotnetmetrics");
                createTableConnection.Execute("CREATE TABLE dotnetmetrics(id INTEGER PRIMARY KEY, value INT, time INT)");

                // Создаем таблицу с метриками HDD
                createTableConnection.Execute("DROP TABLE IF EXISTS hddmetrics");
                createTableConnection.Execute("CREATE TABLE hddmetrics(id INTEGER PRIMARY KEY, value INT, time INT)");

                // Создаем таблицу с метриками Network
                createTableConnection.Execute("DROP TABLE IF EXISTS networkmetrics");
                createTableConnection.Execute("CREATE TABLE networkmetrics(id INTEGER PRIMARY KEY, value INT, time INT)");

                // Создаем таблицу с метриками RAM
                createTableConnection.Execute("DROP TABLE IF EXISTS rammetrics");
                createTableConnection.Execute("CREATE TABLE rammetrics(id INTEGER PRIMARY KEY, value INT, time INT)");
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MetricsAgent v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
