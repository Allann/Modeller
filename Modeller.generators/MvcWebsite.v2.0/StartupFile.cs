using System;
using System.Linq;
using System.Text;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace MvcWebsite
{
    internal class StartupFile : IGenerator
    {
        private readonly Module _module;

        public StartupFile(ISettings settings, Module module)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _module = module ?? throw new ArgumentNullException(nameof(module));
        }

        public ISettings Settings { get; }

        public IOutput Create()
        {
            var sb = new StringBuilder();
            sb.AppendLine("using FluentValidation.AspNetCore;");
            sb.AppendLine("using Microsoft.AspNetCore.Builder;");
            sb.AppendLine("using Microsoft.AspNetCore.Hosting;");
            sb.AppendLine("using Microsoft.Extensions.Configuration;");
            sb.AppendLine("using Microsoft.Extensions.DependencyInjection;");
            sb.AppendLine("using Microsoft.Net.Http.Headers;");
            sb.AppendLine("using Newtonsoft.Json.Serialization;");
            sb.AppendLine($"using {_module.Namespace}.Web.Middleware;");
            sb.AppendLine("using Jbssa.Core.HealthChecks.Extensions;");
            sb.AppendLine("using System;");
            sb.AppendLine("using Microsoft.AspNetCore.DataProtection;");
            sb.AppendLine();
            sb.AppendLine($"namespace {_module.Namespace}.Web");
            sb.AppendLine("{");
            sb.AppendLine("    public class Startup");
            sb.AppendLine("    {");
            sb.AppendLine("        public Startup(IConfiguration configuration)");
            sb.AppendLine("        {");
            sb.AppendLine("            Configuration = configuration;");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        private IConfiguration Configuration { get; }");
            sb.AppendLine();
            sb.AppendLine("        public void ConfigureServices(IServiceCollection services)");
            sb.AppendLine("        {");
            sb.AppendLine("            services");
            sb.AppendLine($"                .Add{_module.Project.Singular.Value}(Configuration)");
            sb.AppendLine("                .AddMvc()");
            sb.AppendLine("                .AddJsonOptions(opt =>");
            sb.AppendLine("                {");
            sb.AppendLine("                    opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;");
            sb.AppendLine("                    opt.SerializerSettings.ContractResolver = new DefaultContractResolver();");
            var model = _module.Models.FirstOrDefault();
            if (model != null)
            {
                sb.AppendLine("                })");
                sb.AppendLine($"                .AddFluentValidation(fv => fv.RegisterValidatorsFromAssembly(System.Reflection.Assembly.GetAssembly(typeof(Domain.{model?.Name.Singular.Value}))));");
            }
            else
                sb.AppendLine("                });");
            sb.AppendLine();
            sb.AppendLine("            services.Configure<AppSettings>(Configuration);");
            sb.AppendLine("            services.AddKendo();");
            sb.AppendLine();
            sb.AppendLine("            services.AddDataProtection()");
            sb.AppendLine($"                .SetApplicationName(\"{_module.Project.Singular.Value}\")");
            sb.AppendLine("                .PersistKeysToFileSystem(new System.IO.DirectoryInfo(@\"/var/dpkeys/\"));");
            sb.AppendLine();
            sb.AppendLine("            services.AddHealthChecks(checks =>");
            sb.AppendLine("            {");
            sb.AppendLine("                var minutes = 1;");
            sb.AppendLine("                if (int.TryParse(Configuration[\"HealthCheck:Timeout\"], out var minutesParsed))");
            sb.AppendLine("                {");
            sb.AppendLine("                    minutes = minutesParsed;");
            sb.AppendLine("                }");
            sb.AppendLine($"                checks.AddUrlCheck(Configuration[\"{_module.Project.Singular.Value}BaseUri\"] + \"/hc\", TimeSpan.FromMinutes(minutes));");
            sb.AppendLine("            });");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        public void Configure(IApplicationBuilder app, IHostingEnvironment env)");
            sb.AppendLine("        {");
            sb.AppendLine("            if (env.IsDevelopment())");
            sb.AppendLine("                app.UseDeveloperExceptionPage();");
            sb.AppendLine("            else");
            sb.AppendLine("                app.UseExceptionHandler(\"/Home/Error\");");
            sb.AppendLine();
            sb.AppendLine("            app.UseStatusCodePagesWithReExecute(\"/Home/Error\",\"?statusCode={0}\");");
            sb.AppendLine("            app.UseStaticFiles(new StaticFileOptions");
            sb.AppendLine("            {");
            sb.AppendLine("                OnPrepareResponse = ctx =>");
            sb.AppendLine("                {");
            sb.AppendLine("                    const int durationInSeconds = 60 * 60 * 24;");
            sb.AppendLine("                    ctx.Context.Response.Headers[HeaderNames.CacheControl] = \"public,max-age=\" + durationInSeconds;");
            sb.AppendLine("                }");
            sb.AppendLine("            });");
            sb.AppendLine();
            sb.AppendLine($"            app.Use{_module.Project.Singular.Value}();");
            sb.AppendLine("            app.UseMvcWithDefaultRoute();");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            return new File { Name = "Startup.cs", Content = sb.ToString(), CanOverwrite = Settings.SupportRegen };
        }
    }
}