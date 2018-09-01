using System;
using System.Text;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace Api
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
            sb.AppendLine("using IdentityServer4.Models;");
            sb.AppendLine("using Microsoft.AspNetCore.Authentication.JwtBearer;");
            sb.AppendLine("using Microsoft.AspNetCore.Builder;");
            sb.AppendLine("using Microsoft.AspNetCore.Hosting;");
            sb.AppendLine("using Microsoft.AspNetCore.Mvc;");
            sb.AppendLine("using Microsoft.AspNetCore.Mvc.Cors.Internal;");
            sb.AppendLine("using Microsoft.AspNetCore.Mvc.Formatters;");
            sb.AppendLine("using Microsoft.Extensions.Configuration;");
            sb.AppendLine("using Microsoft.Extensions.DependencyInjection;");
            sb.AppendLine("using Newtonsoft.Json.Serialization;");
            sb.AppendLine("using Swashbuckle.AspNetCore.Swagger;");
            sb.AppendLine("using Jbssa.Core.ExceptionTranslation.Extensions;");
            sb.AppendLine("using Jbssa.Core.Security;");
            sb.AppendLine($"using {_module.Namespace}.Api.Middleware;");
            sb.AppendLine($"using {_module.Namespace}.Business;");
            sb.AppendLine("using Jbssa.Core.HealthChecks.Extensions;");
            sb.AppendLine("using Jbssa.Core.HealthChecks.Extensions.SqlServer;");
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine();
            sb.AppendLine($"namespace {_module.Namespace}.Api");
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
            sb.AppendLine("            var settings = Configuration.GetSection(\"appSettings\");");
            sb.AppendLine("            services.Configure<SecuritySettings>(settings);");
            sb.AppendLine("            var security = settings.Get<SecuritySettings>();");
            sb.AppendLine();
            sb.AppendLine("            services.AddExceptionTranslators();");
            sb.AppendLine("            services.AddAuthentication(options =>");
            sb.AppendLine("            {");
            sb.AppendLine("                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;");
            sb.AppendLine("                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;");
            sb.AppendLine("            })");
            sb.AppendLine("            .AddJwtBearer(options =>");
            sb.AppendLine("            {");
            sb.AppendLine("                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters();");
            sb.AppendLine("                options.RequireHttpsMetadata = false;");
            sb.AppendLine($"                options.Audience = \"{_module.Project.Singular.Value}\";");
            sb.AppendLine("                options.Authority = security.Authority;");
            sb.AppendLine("            });");
            sb.AppendLine();
            sb.AppendLine($"            services.Add{_module.Project.Singular.Value}(Configuration);");
            sb.AppendLine();
            sb.AppendLine("            services.AddHealthChecks(checks =>");
            sb.AppendLine("            {");
            sb.AppendLine("                var minutes = 1;");
            sb.AppendLine("                if (int.TryParse(Configuration[\"HealthCheck:Timeout\"], out var minutesParsed))");
            sb.AppendLine("                {");
            sb.AppendLine("                    minutes = minutesParsed;");
            sb.AppendLine("                }");
            sb.AppendLine($"                checks.AddSqlCheck(\"{_module.Project.Singular.Value}\", Configuration[\"ConnectionStrings:{_module.Namespace}\"], TimeSpan.FromMinutes(minutes));");
            sb.AppendLine("                checks.AddValueTaskCheck(\"HTTP Endpoint\", () => new ValueTask<IHealthCheckResult>(HealthCheckResult.Healthy(\"Ok\")));");
            sb.AppendLine("            });");
            sb.AppendLine();
            sb.AppendLine("            services.AddMvc(setupAction =>");
            sb.AppendLine("            {");
            sb.AppendLine("                setupAction.ReturnHttpNotAcceptable = true;");
            sb.AppendLine("                setupAction.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());");
            sb.AppendLine("                setupAction.Filters.Add(new ResponseCacheAttribute() { NoStore = true, Location = ResponseCacheLocation.None });");
            sb.AppendLine("            })");
            sb.AppendLine("            .AddJsonOptions(opt =>");
            sb.AppendLine("            {");
            sb.AppendLine("                opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;");
            sb.AppendLine("                opt.SerializerSettings.ContractResolver = new DefaultContractResolver();");
            sb.AppendLine("            })");
            sb.AppendLine($"            .AddFluentValidation(fv => fv.RegisterValidatorsFromAssembly(System.Reflection.Assembly.GetAssembly(typeof(Domain.{_module.Models[0].Name.Singular.Value}))));");
            sb.AppendLine();
            sb.AppendLine("            services.AddSwaggerGen(c =>");
            sb.AppendLine("            {");
            sb.AppendLine("                c.IgnoreObsoleteActions();");
            sb.AppendLine("                c.IgnoreObsoleteProperties();");
            sb.AppendLine("                c.OrderActionsBy(api => $\"{api.GroupName}_{api.HttpMethod}\");");
            sb.AppendLine("                c.OperationFilter<AuthorizeCheckOperationFilter>();");
            sb.AppendLine("                c.SwaggerDoc(\"v1\", new Info");
            sb.AppendLine("                {");
            sb.AppendLine($"                    Title = \"{_module.Project.Singular.Display} API\",");
            sb.AppendLine($"                    Description = \"The publically available API to manage {_module.Project.Singular.Display}.\",");
            sb.AppendLine("                    TermsOfService = \"None\",");
            sb.AppendLine("                    Version = \"v1\",");
            sb.AppendLine("                    Contact = new Contact { Name = \"JBS ServiceDesk\", Email = \"servicedesk@jbssa.com.au\" },");
            sb.AppendLine("                    License = new License { Name = \"Use under LICX (still to be announced)\", Url = \"http://jbssa.com.au\" }");
            sb.AppendLine("                });");
            sb.AppendLine("                c.AddSecurityDefinition(\"oauth2\", new OAuth2Scheme");
            sb.AppendLine("                {");
            sb.AppendLine("                    Type = \"oauth2\",");
            sb.AppendLine("                    Flow = \"implicit\",");
            sb.AppendLine("                    AuthorizationUrl = security.Authority + \"/connect/authorize\",");
            sb.AppendLine("                    TokenUrl = security.Authority + \"/connect/token\",");
            sb.AppendLine("                    Scopes = new Dictionary<string, string>()");
            sb.AppendLine("                {");
            sb.AppendLine($"                    {{ \"{_module.Project.Singular.LocalVariable}\", \"{_module.Project.Singular.Display} API\" }}");
            sb.AppendLine("                }");
            sb.AppendLine("                });");
            sb.AppendLine("                //c.OperationFilter<AuthorizeCheckOperationFilter>();");
            sb.AppendLine();
            sb.AppendLine("                // var basePath = PlatformServices.Default.Application.ApplicationBasePath;");
            sb.AppendLine($"                // var xmlPath = System.IO.Path.Combine(basePath, \"{_module.Namespace}.Api.xml\");");
            sb.AppendLine("                // c.IncludeXmlComments(xmlPath);");
            sb.AppendLine("            });");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        public void Configure(IApplicationBuilder app, IHostingEnvironment env)");
            sb.AppendLine("        {");
            sb.AppendLine("            app.UseExceptionTranslators()");
            sb.AppendLine("                .UseAuthentication()");
            sb.AppendLine($"                .Use{_module.Project.Singular.Value}()");
            sb.AppendLine("                .UseMvc();");
            sb.AppendLine();
            sb.AppendLine("            app.UseSwagger()");
            sb.AppendLine("                .UseSwaggerUI(c =>");
            sb.AppendLine("                {");
            sb.AppendLine("                    c.DisplayRequestDuration();");
            sb.AppendLine("                    c.EnableFilter();");
            sb.AppendLine("                    c.DefaultModelRendering(ModelRendering.Model);");
            sb.AppendLine("                    c.RoutePrefix = \"api-docs\";");
            sb.AppendLine($"                    c.DocumentTitle = \"{_module.Project.Singular.Display} API\";");
            sb.AppendLine($"                    c.SwaggerEndpoint(\"/swagger/v1/swagger.json\", \"{_module.Project.Singular.Display} API v1\");");
            sb.AppendLine("                    c.OAuthClientSecret(\"swaggerSecret\".Sha256());");
            sb.AppendLine("                    c.OAuthClientId(\"swagger\");");
            sb.AppendLine("                });");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            return new File { Name = "Startup.cs", Content = sb.ToString(), CanOverwrite = Settings.SupportRegen };
        }
    }
}