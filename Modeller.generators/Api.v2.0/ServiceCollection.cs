using System;
using System.Linq;
using System.Text;
using Modeller;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace Api
{
    internal class ServiceCollection : IGenerator
    {
        private readonly Module _module;

        public ServiceCollection(ISettings settings, Module module)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _module = module ?? throw new ArgumentNullException(nameof(module));
        }

        public ISettings Settings { get; }

        public IOutput Create()
        {
            var sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine("using AutoMapper;");
            sb.AppendLine("using Jbssa.Core.AutoMapper.TypeConverters;");
            sb.AppendLine("using Jbssa.Core.Interfaces;");
            sb.AppendLine("using Microsoft.AspNetCore.Builder;");
            sb.AppendLine("using Microsoft.AspNetCore.Cors.Infrastructure;");
            sb.AppendLine("using Microsoft.AspNetCore.Http;");
            sb.AppendLine("using Microsoft.AspNetCore.Mvc;");
            sb.AppendLine("using Microsoft.AspNetCore.Mvc.Cors.Internal;");
            sb.AppendLine("using Microsoft.EntityFrameworkCore;");
            sb.AppendLine("using Microsoft.Extensions.Configuration;");
            sb.AppendLine("using Microsoft.Extensions.DependencyInjection;");
            sb.AppendLine($"using {_module.Namespace}.Api.Options;");
            sb.AppendLine($"using {_module.Namespace}.Business.Services;");
            sb.AppendLine($"using {_module.Namespace}.Data;");
            sb.AppendLine($"using {_module.Namespace}.Domain;");
            sb.AppendLine($"using {_module.Namespace}.Interface;");
            sb.AppendLine();
            sb.AppendLine($"namespace {_module.Namespace}.Api.Middleware");
            sb.AppendLine("{");
            sb.AppendLine("    public static class ServiceCollectionExtensions");
            sb.AppendLine("    {");
            sb.AppendLine($"        public static IServiceCollection Add{_module.Project.Singular.Value}(this IServiceCollection services, IConfiguration configuration)");
            sb.AppendLine("        {");
            sb.AppendLine("            if (services == null)");
            sb.AppendLine("                throw new ArgumentNullException(nameof(services));");
            sb.AppendLine();
            sb.AppendLine("            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();");
            sb.AppendLine("            services.AddAutoMapper(cfg => cfg.AddProfiles(typeof(NullableDateTimeConverter), typeof(Startup)));");
            sb.AppendLine();
            sb.AppendLine($"            services.AddDbContext<{_module.Project.Singular.Value}DbContext>(options => options.UseSqlServer(configuration.GetConnectionString(\"{_module.Namespace}\")));");
            sb.AppendLine("            // CodeGen: DbContext (do not remove)");
            sb.AppendLine();
            sb.AppendLine("            // todo: adjust CORS policy to match your project requirements");
            sb.AppendLine("            var corsBuilder = new CorsPolicyBuilder();");
            sb.AppendLine("            corsBuilder.AllowAnyHeader();");
            sb.AppendLine("            corsBuilder.AllowAnyMethod();");
            sb.AppendLine("            corsBuilder.AllowAnyOrigin();");
            sb.AppendLine("            corsBuilder.AllowCredentials();");
            sb.AppendLine("            services.AddCors(options =>");
            sb.AppendLine("            {");
            sb.AppendLine("                options.AddPolicy(\"SiteCorsPolicy\", corsBuilder.Build());");
            sb.AppendLine("            });");
            sb.AppendLine("            services.Configure<MvcOptions>(options =>");
            sb.AppendLine("            {");
            sb.AppendLine("                options.Filters.Add(new CorsAuthorizationFilterFactory(\"SiteCorsPolicy\"));");
            sb.AppendLine("            });");
            sb.AppendLine();
            sb.AppendLine("            //// Register Widgets");
            sb.AppendLine("            //services.AddScoped<IWidgetService, WidgetService>(c =>");
            sb.AppendLine("            //{");
            sb.AppendLine("            //    IWidgetService s = new WidgetService();");
            sb.AppendLine("            //    s.RegisterWidget(new WidgetName());");
            sb.AppendLine("            //    return s as WidgetService;");
            sb.AppendLine("            //});");
            sb.AppendLine();
            foreach (var model in _module.Models.Where(m => m.IsEntity()))
            {
                sb.AppendLine("            services");
                sb.AppendLine($"                .AddScoped<IListProvider<Domain.{model.Name.Singular.Value}>, {model.Name.Singular.Value}ListProvider>()");
                sb.AppendLine($"                .AddScoped<I{model.Name.Singular.Value}Features, {model.Name.Singular.Value}Features>()");
                sb.AppendLine($"                .AddScoped<IEntityOptionsAsync<Domain.{model.Name.Singular.Value}>, {model.Name.Singular.Value}Options>()");
                sb.AppendLine($"                .AddScoped<IReadableAsync<Domain.{model.Name.Singular.Value}>, {model.Name.Singular.Value}ReadService>()");
                sb.AppendLine($"                .AddScoped<IEditableAsync<Domain.{model.Name.Singular.Value}>, {model.Name.Singular.Value}EditService>();");
            }
            sb.AppendLine("            // CodeGen: Services (do not remove)");
            sb.AppendLine("            return services;");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine($"        public static IApplicationBuilder Use{_module.Project.Singular.Value}(this IApplicationBuilder app)");
            sb.AppendLine("        {");
            sb.AppendLine("            app.UseCors(\"SiteCorsPolicy\");");
            sb.AppendLine("            return app;");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            return new File { Name = "ServiceCollectionExtensions.cs", Content = sb.ToString(), Path="Middleware", CanOverwrite = Settings.SupportRegen };
        }
    }
}