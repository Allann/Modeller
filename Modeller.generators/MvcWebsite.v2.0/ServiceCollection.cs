using System;
using System.Text;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace MvcWebsite
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
            sb.AppendLine("using IdentityServer4.Extensions;");
            sb.AppendLine("using IdentityServer4.Models;");
            sb.AppendLine($"using {_module.Namespace}.Security;");
            sb.AppendLine("using Jbssa.Core.Interfaces;");
            sb.AppendLine("using Jbssa.Core.Security;");
            sb.AppendLine("using Microsoft.AspNetCore.Authorization;");
            sb.AppendLine("using Microsoft.AspNetCore.Builder;");
            sb.AppendLine("using Microsoft.AspNetCore.Http;");
            sb.AppendLine("using Microsoft.Extensions.Configuration;");
            sb.AppendLine("using Microsoft.Extensions.DependencyInjection;");
            sb.AppendLine("using System;");
            sb.AppendLine("using System.IdentityModel.Tokens.Jwt;");
            sb.AppendLine();
            sb.AppendLine($"namespace {_module.Namespace}.Web.Middleware");
            sb.AppendLine("{");
            sb.AppendLine("    public static class ServiceCollectionExtensions");
            sb.AppendLine("    {");
            sb.AppendLine($"        public static IServiceCollection Add{_module.Project.Singular.Value}(this IServiceCollection services, IConfiguration configuration)");
            sb.AppendLine("        {");
            sb.AppendLine("            if (services == null)");
            sb.AppendLine("                throw new ArgumentNullException(nameof(services));");
            sb.AppendLine();
            sb.AppendLine("            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();");
            sb.AppendLine("            services.AddScoped<IJbsAuthorizationService, JbsAuthorizationService>();");
            sb.AppendLine("            services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();");
            sb.AppendLine("            services.AddScoped<ISecurityTokens, SecurityTokens>();");
            sb.AppendLine();
            sb.AppendLine("            var settings = configuration.GetSection(\"Security\");");
            sb.AppendLine("            services.Configure<SecuritySettings>(settings);");
            sb.AppendLine("            var security = settings.Get<SecuritySettings>();");
            sb.AppendLine("            services.AddScoped(s => security);");
            sb.AppendLine();
            sb.AppendLine("            var secret = security.ClientSecret;");
            sb.AppendLine("            if (secret.IsNullOrEmpty())");
            sb.AppendLine("            {");
            sb.AppendLine("                secret = \"defaultSecret\".Sha256();");
            sb.AppendLine("            }");
            sb.AppendLine();
            sb.AppendLine("            services.AddAuthentication(auth =>");
            sb.AppendLine("            {");
            sb.AppendLine("                auth.DefaultScheme = security.DefaultScheme;");
            sb.AppendLine("                auth.DefaultAuthenticateScheme = security.DefaultScheme; // CookieAuthenticationDefaults.AuthenticationScheme;");
            sb.AppendLine("                auth.DefaultChallengeScheme = security.ChallengeScheme; // OpenIdConnectDefaults.AuthenticationScheme;");
            sb.AppendLine("                auth.DefaultSignInScheme = security.SignInScheme; // CookieAuthenticationDefaults.AuthenticationScheme;");
            sb.AppendLine("            })");
            sb.AppendLine("            .AddOpenIdConnect(options =>");
            sb.AppendLine("            {");
            sb.AppendLine("                options.ClientId = security.ClientId;");
            sb.AppendLine("                options.ClientSecret = secret;");
            sb.AppendLine("                options.Authority = security.Authority;");
            sb.AppendLine("                options.SaveTokens = security.SaveTokens;");
            sb.AppendLine("                options.TokenValidationParameters.NameClaimType = \"name\";");
            sb.AppendLine("                options.RequireHttpsMetadata = security.RequireHttpsMetadata;");
            sb.AppendLine("                options.ResponseType = security.ResponseType;");
            sb.AppendLine();
            sb.AppendLine("                var scopes = security.ClientScope?.Split(' ');");
            sb.AppendLine("                if (scopes != null)");
            sb.AppendLine("                {");
            sb.AppendLine("                    foreach (var scope in scopes)");
            sb.AppendLine("                        options.Scope.Add(scope);");
            sb.AppendLine("                }");
            sb.AppendLine();
            sb.AppendLine("                options.GetClaimsFromUserInfoEndpoint = security.GetClaimsFromUserInfoEndpoint;");
            sb.AppendLine("            })");
            sb.AppendLine("            .AddCookie();");
            sb.AppendLine();
            sb.AppendLine($"            services.Add{_module.Project.Singular.Value}Policies();");
            sb.AppendLine();
            sb.AppendLine("            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();");
            sb.AppendLine("            services.AddSingleton<ViewModels.Shared.ApplicationModel>()");
            sb.AppendLine("                .AddOptions();");
            sb.AppendLine();
            sb.AppendLine("            return services;");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine($"        public static IApplicationBuilder Use{_module.Project.Singular.Value}(this IApplicationBuilder app)");
            sb.AppendLine("        {");
            sb.AppendLine("            app.UseCoreSecurity();");
            sb.AppendLine("            return app;");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return new File() { Name = "ServiceCollectionExtensions.cs", Content = sb.ToString(), Path = "Middleware" };
        }
    }
}