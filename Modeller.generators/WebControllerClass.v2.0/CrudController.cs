using System;
using System.Text;
using Modeller.Core;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace WebController
{
    internal class CrudController : IGenerator
    {
        private readonly Module _module;
        private readonly Model _model;

        public CrudController(ISettings settings, Module module, Model model)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _module = module ?? throw new ArgumentNullException(nameof(module));
            _model = model ?? throw new ArgumentNullException(nameof(model));
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
            sb.AppendLine("using Flurl;");
            sb.AppendLine("using Flurl.Http;");
            sb.AppendLine($"using {_module.Namespace}.Dto;");
            sb.AppendLine($"using {_module.Namespace}.Web.Extensions;");
            sb.AppendLine($"using {_module.Namespace}.Web.ViewModels.{_model.Name.Plural.Value};");
            sb.AppendLine("using Jbssa.Core;");
            sb.AppendLine("using Jbssa.Core.Mvc;");
            sb.AppendLine("using Jbssa.Core.Mvc.Extensions;");
            sb.AppendLine("using Jbssa.Core.Mvc.Services.Toastr;");
            sb.AppendLine("using Jbssa.Core.Mvc.ViewModels;");
            sb.AppendLine("using Kendo.Mvc.Extensions;");
            sb.AppendLine("using Kendo.Mvc.UI;");
            sb.AppendLine("using Microsoft.AspNetCore.Authorization;");
            sb.AppendLine("using Microsoft.AspNetCore.Mvc;");
            sb.AppendLine("using Microsoft.AspNetCore.Mvc.Rendering;");
            sb.AppendLine("using Microsoft.Extensions.Logging;");
            sb.AppendLine("using Microsoft.Extensions.Options;");
            sb.AppendLine("using Newtonsoft.Json;");
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Linq;");
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine();
            sb.AppendLine($"namespace {_module.Namespace}.Web.Controllers");
            sb.AppendLine("{");
            sb.AppendLine($"{i1}//[Authorize(Security.SupportedPolicies.{_model.Name.Plural.Value})]");
            sb.AppendLine($"{i1}public partial class {_model.Name.Plural.Value}Controller : Core.Mvc.ControllerBase");
            sb.AppendLine($"{i1}{{");
            sb.AppendLine($"{i2}private ILogger<{_model.Name.Plural.Value}Controller> _logger;");
            sb.AppendLine($"{i2}private readonly string _baseUri;");
            sb.AppendLine();
            var parentModel = _model.Name;
            sb.Append($"{i2}partial void AddLinks(SidebarViewModel sidebar, {parentModel.Singular.Value}Response data = null");
            foreach (var item in _model.Relationships)
            {
                item.GetOther(parentModel, out var childType, out var childModel, out var childField);
                if (childType == RelationShipTypes.One)
                {
                    sb.Append($", Guid? {childModel.Singular.LocalVariable}{childField.Singular.Value} = null");
                }
            }
            sb.AppendLine(");");
            sb.AppendLine();
            sb.AppendLine($"{i2}public {_model.Name.Plural.Value}Controller(ILogger<{_model.Name.Plural.Value}Controller> logger, IOptionsSnapshot<AppSettings> settings)");
            sb.AppendLine($"{i2}{{");
            sb.AppendLine($"{i3}_logger = logger;");
            sb.AppendLine($"{i3}var baseUri = settings.Value.{_module.Project.Singular.Value}BaseUri;");
            sb.AppendLine($"{i3}if (string.IsNullOrWhiteSpace(baseUri))");
            sb.AppendLine($"{i4}throw new InvalidOperationException(KnownMessageFor.Exceptions.MissingBaseUri);");
            sb.AppendLine($"{i3}_baseUri = baseUri + \"/api\";");
            sb.AppendLine($"{i2}}}");
            sb.AppendLine();
            sb.AppendLine($"{i2}protected override string DisplayName => KnownEntities.{_model.Name.Singular.Value}.DisplayName;");
            sb.AppendLine();
            sb.AppendLine($"{i2}[Route(\"[controller]/Lookup\", Name = RouteNames.{_model.Name.Singular.Value}Lookup)]");
            sb.AppendLine($"{i2}public async Task<IActionResult> Lookup([DataSourceRequest]DataSourceRequest request)");
            sb.AppendLine($"{i2}{{");
            sb.AppendLine($"{i3}try");
            sb.AppendLine($"{i3}{{");
            sb.AppendLine($"{i4}var url = _baseUri.AppendPathSegments(KnownApiRoutes.{_model.Name.Plural.Value}.Name, KnownApiRoutes.{_model.Name.Plural.Value}.Lookup);");
            sb.AppendLine($"{i4}var result = await url.GetJsonAsync<Dictionary<Guid, string>>();");
            sb.AppendLine($"{i4}if (result == null)");
            sb.AppendLine($"{i5}return NotFound();");
            sb.AppendLine($"{i4}var models = result.Select(d => new SelectListItem {{ Value = d.Key.ToString(), Text = d.Value }});");
            sb.AppendLine($"{i4}var dsr = new DataSourceResult");
            sb.AppendLine($"{i4}{{");
            sb.AppendLine($"{i5}Data = models,");
            sb.AppendLine($"{i5}Total = models.Count()");
            sb.AppendLine($"{i4}}};");
            sb.AppendLine($"{i4}return Ok(dsr);");
            sb.AppendLine($"{i3}}}");
            sb.AppendLine($"{i3}catch (FlurlHttpException ex)");
            sb.AppendLine($"{i3}{{");
            sb.AppendLine($"{i4}ex.Call.LogFailedCall(_logger, KnownMessageFor.WebController.FindFailed(DisplayName, string.Empty));");
            sb.AppendLine($"{i4}throw ex.Call.Exception;");
            sb.AppendLine($"{i3}}}");
            sb.AppendLine($"{i3}catch (TranslatedException ex)");
            sb.AppendLine($"{i3}{{");
            sb.AppendLine($"{i4}this.AddToastMessage($\"{{DisplayName}} failed\", ex.ToString(), ToastType.Error);");
            sb.AppendLine($"{i4}return NoContent();");
            sb.AppendLine($"{i3}}}");
            sb.AppendLine($"{i2}}}");
            sb.AppendLine();
            sb.AppendLine($"{i2}public override BaseQuickFilterViewModel CreateFilterViewModel()");
            sb.AppendLine($"{i2}{{");
            sb.AppendLine($"{i3}var vm = new {_model.Name.Singular.Value}FilterViewModel();");
            sb.AppendLine($"{i3}AddLinks(vm.Page.Sidebar);");
            sb.AppendLine($"{i3}return vm;");
            sb.AppendLine($"{i2}}}");
            sb.AppendLine();
            sb.AppendLine($"{i2}public IActionResult Index(string id)");
            sb.AppendLine($"{i2}{{");
            sb.AppendLine($"{i3}var viewModel = CreateFilterViewModel() as {_model.Name.Singular.Value}FilterViewModel;");
            sb.AppendLine($"{i3}viewModel.{_model.Name.Singular.Value}Uri = _baseUri.AppendPathSegment(KnownApiRoutes.{_model.Name.Plural.Value}.Name);");
            sb.AppendLine($"{i3}viewModel.Page.Referer = Request.Headers[\"Referer\"].ToString();");
            sb.AppendLine($"{i3}return View(viewModel);");
            sb.AppendLine($"{i2}}}");
            sb.AppendLine();
            sb.AppendLine($"{i2}public async Task<IActionResult> Detail(string id)");
            sb.AppendLine($"{i2}{{");
            sb.AppendLine($"{i3}try");
            sb.AppendLine($"{i3}{{");
            sb.AppendLine($"{i4}_logger.LogInformation(KnownMessageFor.WebController.Fetching(DisplayName, id));");
            sb.AppendLine();
            sb.AppendLine($"{i4}var url = _baseUri");
            sb.AppendLine($"{i5}.AppendPathSegments(KnownApiRoutes.{_model.Name.Plural.Value}.Name, id);");
            sb.AppendLine($"{i4}var model = await url.GetJsonAsync<{_model.Name.Singular.Value}Response>();");
            sb.AppendLine();
            sb.AppendLine($"{i4}var filterModel = GetTemp<{_model.Name.Singular.Value}FilterViewModel>(nameof({_model.Name.Singular.Value}Response));");
            sb.AppendLine($"{i4}if (filterModel != null && model != null)");
            sb.AppendLine($"{i4}{{");
            sb.AppendLine($"{i5}filterModel.SelectedId = model.BusinessKey;");
            sb.AppendLine($"{i5}SetTemp(nameof({_model.Name.Singular.Value}Response) + id, filterModel);");
            sb.AppendLine($"{i4}}}");
            sb.AppendLine();
            sb.AppendLine($"{i4}var viewModel = model.GetDetailViewModel(filterModel);");
            foreach (var item in _model.Relationships)
            {
                item.GetOther(_model.Name, out var type, out var model, out var field);
                if (type == RelationShipTypes.Many)
                    sb.AppendLine($"{i4}viewModel.{model.Singular.Value}Uri = _baseUri.AppendPathSegment(KnownApiRoutes.{model.Plural.Value}.Name);");
            }
            sb.AppendLine($"{i4}AddLinks(viewModel.Page.Sidebar, model);");
            sb.AppendLine($"{i4}viewModel.Page.Referer = Request.Headers[\"Referer\"].ToString();");
            sb.AppendLine($"{i4}return View(viewModel);");
            sb.AppendLine($"{i3}}}");
            sb.AppendLine($"{i3}catch (FlurlHttpException ex)");
            sb.AppendLine($"{i3}{{");
            sb.AppendLine($"{i4}ex.Call.LogFailedCall(_logger, KnownMessageFor.WebController.FindFailed(DisplayName, string.Empty));");
            sb.AppendLine($"{i4}return this.RedirectOrThrow(ex.Call, DisplayName);");
            sb.AppendLine($"{i3}}}");
            sb.AppendLine($"{i2}}}");
            sb.AppendLine();
            sb.Append($"{i2}public async Task<IActionResult> Create(");
            var first = true;
            foreach (var item in _model.Relationships)
            {
                item.GetOther(_model.Name, out var type, out var model, out var field);
                if (type == RelationShipTypes.One)
                {
                    if (!first)
                        sb.Append(", ");
                    sb.Append($"[FromQuery] Guid? {model.Singular.LocalVariable}Id");
                    first = false;
                }
            }
            sb.AppendLine(")");
            sb.AppendLine($"{i2}{{");
            sb.AppendLine($"{i3}try");
            sb.AppendLine($"{i3}{{");
            sb.AppendLine($"{i4}_logger.LogInformation(KnownMessageFor.WebController.Creating(DisplayName));");
            sb.AppendLine();
            sb.AppendLine($"{i4}var url = _baseUri");
            sb.AppendLine($"{i5}.AppendPathSegments(KnownApiRoutes.{_model.Name.Plural.Value}.Name, KnownApiRoutes.{_model.Name.Plural.Value}.Create);");
            sb.AppendLine($"{i4}var model = await url.GetJsonAsync<{_model.Name.Singular.Value}Response>();");
            sb.AppendLine();
            sb.AppendLine($"{i4}var viewModel = model.GetCreateViewModel();");
            sb.AppendLine($"{i4}viewModel.Page.Referer = Request.Headers[\"Referer\"].ToString();");
            foreach (var item in _model.Relationships)
            {
                item.GetOther(_model.Name, out var type, out var model, out var field);
                if (type == RelationShipTypes.One)
                {
                    sb.AppendLine($"{i4}if ({model.Singular.LocalVariable}Id.HasValue && {model.Singular.LocalVariable}Id.Value != Guid.Empty)");
                    sb.AppendLine($"{i5}viewModel.{model.Singular.Value}Id = {model.Singular.LocalVariable}Id.Value;");
                }
            }
            sb.AppendLine($"{i4}return View(viewModel);");
            sb.AppendLine($"{i3}}}");
            sb.AppendLine($"{i3}catch (FlurlHttpException ex)");
            sb.AppendLine($"{i3}{{");
            sb.AppendLine($"{i4}await this.LogError(ex, ModelState, _logger, KnownMessageFor.WebController.CreateFailed(DisplayName), KnownEntities.{_model.Name.Singular.Value}.DisplayName);");
            sb.AppendLine($"{i4}return BadRequest(ex.Message);");
            sb.AppendLine($"{i3}}}");
            sb.AppendLine($"{i2}}}");
            sb.AppendLine();
            sb.AppendLine($"{i2}[HttpPost]");
            sb.AppendLine($"{i2}[ValidationToast]");
            sb.AppendLine($"{i2}[ValidateAntiForgeryToken]");
            sb.AppendLine($"{i2}public async Task<IActionResult> Create({_model.Name.Singular.Value}CreateViewModel viewModel)");
            sb.AppendLine($"{i2}{{");
            sb.AppendLine($"{i3}if (!ModelState.IsValid)");
            sb.AppendLine($"{i4}return View(viewModel);");
            sb.AppendLine();
            sb.AppendLine($"{i3}_logger.LogInformation(KnownMessageFor.WebController.Creating(DisplayName));");
            sb.AppendLine();
            sb.AppendLine($"{i3}var model = viewModel.CreateModel();");
            sb.AppendLine($"{i3}try");
            sb.AppendLine($"{i3}{{");
            sb.AppendLine($"{i4}var entity = await _baseUri");
            sb.AppendLine($"{i5}.AppendPathSegment(KnownApiRoutes.{_model.Name.Plural.Value}.Name)");
            sb.AppendLine($"{i5}.PostJsonAsync(model)");
            sb.AppendLine($"{i5}.ReceiveJson<{_model.Name.Singular.Value}Response>();");
            sb.AppendLine();
            sb.AppendLine($"{i4}if (entity != null)");
            sb.AppendLine($"{i4}{{");
            sb.AppendLine($"{i5}this.AddToastMessage(KnownMessageFor.WebController.Created(DisplayName), entity.ToString(), ToastType.Success);");
            sb.AppendLine($"{i5}return GotoDefaultView(entity.BusinessKey);");
            sb.AppendLine($"{i4}}}");
            sb.AppendLine($"{i3}}}");
            sb.AppendLine($"{i3}catch (FlurlHttpException ex)");
            sb.AppendLine($"{i3}{{");
            sb.AppendLine($"{i4}await this.LogError(ex, ModelState, _logger, KnownMessageFor.WebController.CreateFailed(DisplayName), KnownEntities.{_model.Name.Singular.Value}.DisplayName);");
            sb.AppendLine($"{i3}}}");
            sb.AppendLine($"{i3}return View(viewModel);");
            sb.AppendLine($"{i2}}}");
            sb.AppendLine();
            sb.AppendLine($"{i2}public async Task<IActionResult> Edit(string id)");
            sb.AppendLine($"{i2}{{");
            sb.AppendLine($"{i3}try");
            sb.AppendLine($"{i3}{{");
            sb.AppendLine($"{i4}_logger.LogInformation(KnownMessageFor.WebController.Editing(DisplayName, id));");
            sb.AppendLine();
            sb.AppendLine($"{i4}var url = _baseUri");
            sb.AppendLine($"{i5}.AppendPathSegments(KnownApiRoutes.{_model.Name.Plural.Value}.Name, id);");
            sb.AppendLine($"{i4}var model = await url.GetJsonAsync<{_model.Name.Singular.Value}Response>();");
            sb.AppendLine($"{i4}if (model == null)");
            sb.AppendLine($"{i5}return NotFound(KnownMessageFor.Exceptions.NotFound(DisplayName, id));");
            sb.AppendLine();
            sb.AppendLine($"{i4}var filterModel = GetTemp<{_model.Name.Singular.Value}FilterViewModel>(nameof({_model.Name.Singular.Value}Response));");
            sb.AppendLine($"{i4}if (filterModel != null && model != null)");
            sb.AppendLine($"{i4}{{");
            sb.AppendLine($"{i5}//set the selected id");
            sb.AppendLine($"{i5}filterModel.SelectedId = model.BusinessKey;");
            sb.AppendLine($"{i5}filterModel.StoredQueryId = id;");
            sb.AppendLine($"{i5}SetTemp(nameof({_model.Name.Singular.Value}Response) + filterModel.StoredQueryId, filterModel);");
            sb.AppendLine($"{i4}}}");
            sb.AppendLine();
            sb.AppendLine($"{i4}var viewModel = model.GetEditViewModel(filterModel);");
            sb.AppendLine($"{i4}viewModel.Page.Referer = Request.Headers[\"Referer\"].ToString();");
            sb.AppendLine($"{i4}return View(viewModel);");
            sb.AppendLine($"{i3}}}");
            sb.AppendLine($"{i3}catch (FlurlHttpException ex)");
            sb.AppendLine($"{i3}{{");
            sb.AppendLine($"{i4}await this.LogError(ex, ModelState, _logger, KnownMessageFor.WebController.EditFailed(DisplayName, id), KnownEntities.{_model.Name.Singular.Value}.DisplayName);");
            sb.AppendLine($"{i4}return BadRequest(ex.Message);");
            sb.AppendLine($"{i3}}}");
            sb.AppendLine($"{i2}}}");
            sb.AppendLine();
            sb.AppendLine($"{i2}[HttpPost]");
            sb.AppendLine($"{i2}[ValidationToast]");
            sb.AppendLine($"{i2}[ValidateAntiForgeryToken]");
            sb.AppendLine($"{i2}public async Task<IActionResult> Edit({_model.Name.Singular.Value}EditViewModel viewModel)");
            sb.AppendLine($"{i2}{{");
            sb.AppendLine($"{i3}if (!ModelState.IsValid)");
            sb.AppendLine($"{i4}return View(viewModel);");
            sb.AppendLine();
            sb.AppendLine($"{i3}var id = viewModel.Id.ToString();");
            sb.AppendLine($"{i3}try");
            sb.AppendLine($"{i3}{{");
            sb.AppendLine($"{i4}var model = viewModel.CreateUpdateModel();");
            sb.AppendLine($"{i4}id = model.Id.ToString();");
            sb.AppendLine();
            sb.AppendLine($"{i4}_logger.LogInformation(KnownMessageFor.WebController.Editing(DisplayName, id));");
            sb.AppendLine();
            sb.AppendLine($"{i4}var entity = await _baseUri");
            sb.AppendLine($"{i5}.AppendPathSegments(KnownApiRoutes.{_model.Name.Plural.Value}.Name, id)");
            sb.AppendLine($"{i5}.PutJsonAsync(model)");
            sb.AppendLine($"{i5}.ReceiveJson<{_model.Name.Singular.Value}Response>();");
            sb.AppendLine();
            sb.AppendLine($"{i4}if (entity != null)");
            sb.AppendLine($"{i4}{{");
            sb.AppendLine($"{i5}this.AddToastMessage(KnownMessageFor.WebController.Edited(DisplayName), entity.ToString(), ToastType.Success);");
            sb.AppendLine($"{i5}return GotoDefaultView(entity.BusinessKey);");
            sb.AppendLine($"{i4}}}");
            sb.AppendLine($"{i3}}}");
            sb.AppendLine($"{i3}catch (FlurlHttpException ex)");
            sb.AppendLine($"{i3}{{");
            sb.AppendLine($"{i4}await this.LogError(ex, ModelState, _logger, KnownMessageFor.WebController.EditFailed(DisplayName, id), KnownEntities.{_model.Name.Singular.Value}.DisplayName);");
            sb.AppendLine($"{i3}}}");
            sb.AppendLine($"{i3}return View(viewModel);");
            sb.AppendLine($"{i2}}}");
            sb.AppendLine();
            sb.AppendLine($"{i2}[HttpPost]");
            sb.AppendLine($"{i2}[ValidateAntiForgeryToken]");
            sb.AppendLine($"{i2}public async Task<IActionResult> Delete(string id)");
            sb.AppendLine($"{i2}{{");
            sb.AppendLine($"{i3}_logger.LogInformation(KnownMessageFor.WebController.Deleting(DisplayName, id));");
            sb.AppendLine();
            sb.AppendLine($"{i3}var code = id;");
            sb.AppendLine($"{i3}try");
            sb.AppendLine($"{i3}{{");
            sb.AppendLine($"{i4}var model = await _baseUri");
            sb.AppendLine($"{i4}    .AppendPathSegments(KnownApiRoutes.{_model.Name.Plural.Value}.Name, id)");
            sb.AppendLine($"{i4}    .GetJsonAsync<{_model.Name.Singular.Value}Response>();");
            sb.AppendLine($"{i4}code = model?.ToString() ?? id.ToString();");
            sb.AppendLine($"{i4}var result = await _baseUri");
            sb.AppendLine($"{i5}.AppendPathSegments(KnownApiRoutes.{_model.Name.Plural.Value}.Name, id)");
            sb.AppendLine($"{i5}.DeleteAsync();");
            sb.AppendLine($"{i4}if (result.IsSuccessStatusCode)");
            sb.AppendLine($"{i4}{{");
            sb.AppendLine($"{i5}this.AddToastMessage(KnownMessageFor.WebController.Deleted(DisplayName), code, ToastType.Success);");
            sb.AppendLine($"{i5}return RedirectToAction(nameof(Index));");
            sb.AppendLine($"{i4}}}");
            sb.AppendLine($"{i3}}}");
            sb.AppendLine($"{i3}catch (FlurlHttpException ex)");
            sb.AppendLine($"{i3}{{");
            sb.AppendLine($"{i4}await this.LogError(ex, ModelState, _logger, KnownMessageFor.WebController.DeleteFailed(DisplayName, code), KnownEntities.{_model.Name.Singular.Value}.DisplayName);");
            sb.AppendLine($"{i3}}}");
            sb.AppendLine($"{i3}return GotoDefaultView(id);");
            sb.AppendLine($"{i2}}}");
            sb.AppendLine($"{i1}}}");
            sb.AppendLine("}");

            return new File
            {
                Name = _model.Name.Plural.Value + "Controller.cs",
                Content = sb.ToString(),
                CanOverwrite = Settings.SupportRegen
            };
        }
    }
}