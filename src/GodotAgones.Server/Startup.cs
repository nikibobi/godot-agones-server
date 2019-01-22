using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Net;
using System.Net.Http;

namespace GodotAgones.Server
{
    using Services;

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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            
            bool inCluster = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("IN_CLUSTER"));

            services.AddHttpClient<AgonesService>(c =>
            {
                string baseUrl = "http://127.0.0.1:8001";

                if (inCluster)
                {
                    string host = Environment.GetEnvironmentVariable("KUBERNETES_SERVICE_HOST");
                    string port = Environment.GetEnvironmentVariable("KUBERNETES_SERVICE_PORT");
                    
                    baseUrl = $"https://{host}:{port}";

                    const string serviceAccountPath = "/var/run/secrets/kubernetes.io/serviceaccount";
                    string tokenPath = Path.Combine(serviceAccountPath, "token");
                    string token = File.ReadAllText(tokenPath);
                    c.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                }
                
                // TODO: move to options
                const string api = "stable.agones.dev";
                const string version = "v1alpha1";
                const string @namespace = "default";

                c.BaseAddress = new Uri($"{baseUrl}/apis/{api}/{version}/namespaces/{@namespace}/");
            }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
