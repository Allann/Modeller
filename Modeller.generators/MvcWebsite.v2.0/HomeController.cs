using System;
using System.Text;
using Modeller.Core;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace MvcWebsite
{
    internal class HomeController : IGenerator
    {
        private readonly Module _module;

        public HomeController(ISettings settings, Module module)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _module = module ?? throw new ArgumentNullException(nameof(module));
        }

        public ISettings Settings { get; }

        public IOutput Create()
        {
            var i1 = h.Indent(1);
            var i2 = h.Indent(2);
            var i3 = h.Indent(3);
            var i4 = h.Indent(4);
            var i5 = h.Indent(5);

            var sb = new StringBuilder();
            sb.AppendLine("using Microsoft.AspNetCore.Mvc;");
            sb.AppendLine("using IdentityModel.Client;");
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine("using Microsoft.AspNetCore.Authentication;");
            sb.AppendLine("using Microsoft.IdentityModel.Protocols.OpenIdConnect;");
            sb.AppendLine("using System.Globalization;");
            sb.AppendLine("using System;");
            sb.AppendLine("using Microsoft.Extensions.Options;");
            sb.AppendLine($"using {_module.Namespace}.Web.Extensions;");
            sb.AppendLine("");
            sb.AppendLine($"namespace {_module.Namespace}.Web.Controllers");
            sb.AppendLine("{");
            sb.AppendLine("    public class HomeController : Controller");
            sb.AppendLine("    {");
            sb.AppendLine("        private readonly AppSettings _settings;");
            sb.AppendLine("        private readonly ViewModels.Shared.ApplicationModel _applicationModel;");
            sb.AppendLine();
            sb.AppendLine("        public HomeController(IOptionsSnapshot<AppSettings> settings, ViewModels.Shared.ApplicationModel applicationModel)");
            sb.AppendLine("        {");
            sb.AppendLine("            _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));");
            sb.AppendLine("            _applicationModel = applicationModel ?? throw new ArgumentNullException(nameof(applicationModel));");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        public IActionResult Index()");
            sb.AppendLine("        {");
            sb.AppendLine("            return View();");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        private async Task RefreshTokens()");
            sb.AppendLine("        {");
            sb.AppendLine("            var authorizationServerInformation = await DiscoveryClient.GetAsync(_settings.Security.Authority);");
            sb.AppendLine();
            sb.AppendLine("            var client = new TokenClient(authorizationServerInformation.TokenEndpoint, _settings.Security.ClientId, \"secret\");");
            sb.AppendLine("            var refreshToken = await HttpContext.GetTokenAsync(\"refresh_token\");");
            sb.AppendLine("            var tokenResponse = await client.RequestRefreshTokenAsync(refreshToken);");
            sb.AppendLine("            var identityToken = await HttpContext.GetTokenAsync(\"id_token\");");
            sb.AppendLine("            var expiresAt = DateTime.UtcNow + TimeSpan.FromSeconds(tokenResponse.ExpiresIn);");
            sb.AppendLine("            var tokens = new[]");
            sb.AppendLine("            {");
            sb.AppendLine("                new AuthenticationToken");
            sb.AppendLine("                {");
            sb.AppendLine("                    Name = OpenIdConnectParameterNames.IdToken,");
            sb.AppendLine("                    Value = identityToken");
            sb.AppendLine("                },");
            sb.AppendLine("                new AuthenticationToken");
            sb.AppendLine("                {");
            sb.AppendLine("                    Name = OpenIdConnectParameterNames.AccessToken,");
            sb.AppendLine("                    Value = tokenResponse.AccessToken");
            sb.AppendLine("                },");
            sb.AppendLine("                new AuthenticationToken");
            sb.AppendLine("                {");
            sb.AppendLine("                    Name = OpenIdConnectParameterNames.RefreshToken,");
            sb.AppendLine("                    Value = tokenResponse.RefreshToken");
            sb.AppendLine("                },");
            sb.AppendLine("                new AuthenticationToken");
            sb.AppendLine("                {");
            sb.AppendLine("                    Name = \"expires_at\",");
            sb.AppendLine("                    Value = expiresAt.ToString(\"o\", CultureInfo.InvariantCulture)");
            sb.AppendLine("                }");
            sb.AppendLine("            };");
            sb.AppendLine("            var authenticationInformation = await HttpContext.AuthenticateAsync(_settings.Security.SignInScheme);");
            sb.AppendLine("            authenticationInformation.Properties.StoreTokens(tokens);");
            sb.AppendLine("            await HttpContext.SignInAsync(_settings.Security.SignInScheme, authenticationInformation.Principal, authenticationInformation.Properties);");
            sb.AppendLine("        }");
            sb.AppendLine("");
            sb.AppendLine("        public async Task Logout()");
            sb.AppendLine("        {");
            sb.AppendLine("            await HttpContext.SignOutAsync(\"Cookies\");");
            sb.AppendLine("            await HttpContext.SignOutAsync(\"oidc\");");
            sb.AppendLine("        }");
            sb.AppendLine("");
            sb.AppendLine("        public IActionResult Error(int? statusCode = null)");
            sb.AppendLine("        {");
            sb.AppendLine("            return statusCode.HasValue ? this.GetViewResult(statusCode.Value, _settings, _applicationModel) : View();");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            return new File()
            {
                Name = "HomeController.cs",
                Content = sb.ToString(),
                Path = "Controllers",
                CanOverwrite = Settings.SupportRegen
            };
        }
    }
}