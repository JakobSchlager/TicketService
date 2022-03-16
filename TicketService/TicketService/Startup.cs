using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicketDbLib;
using MassTransit; 

namespace TicketService
{
    public class Startup
    {
        private readonly string myAllowSpecificOrigins = "_myAllowSpecificOrigins";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Masstransit RabbitMQ
            var queueSettings = Configuration.GetSection("RabbitMQ:QueueSettings").Get<QueueSettings>(); 
            var rabbitmqHostname = Environment.GetEnvironmentVariable("RABBITMQ_HOSTNAME"); 
            if(rabbitmqHostname != null)
            {
                queueSettings.HostName = rabbitmqHostname; 
            }
            Console.WriteLine($"queueSettings.HostName = {queueSettings.HostName}");
            
            services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(queueSettings.HostName, queueSettings.VirtualHost, h =>
                    {
                        h.Username(queueSettings.UserName);
                        h.Password(queueSettings.Password); 
                    }); 
                    cfg.ConfigureEndpoints(context);
                });
            });
            services.AddMassTransitHostedService();

            //Basic ConfigureServices

            services.AddDbContext<TicketDbContext>(options => options.UseMySql(Configuration["ConnectionStrings:DefaultConnection"], new MySqlServerVersion(new Version(8, 0, 26))));

            services.AddScoped<Services.RoomService>();
            services.AddScoped<Services.MovieService>();
            services.AddScoped<Services.TicketService>();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "TicketService", Version = "v1" });
            });
            services.AddCors(options =>
            {
                options.AddPolicy(myAllowSpecificOrigins,
                x => x.AllowAnyMethod()
                .AllowAnyHeader()
                .AllowAnyOrigin());
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TicketService v1"));
            }
            app.UseCors(myAllowSpecificOrigins);

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
