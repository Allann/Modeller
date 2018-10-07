using System;
using System.Text;
using Modeller;
using Modeller.Core;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace ApiController
{
    public class ControllerGenerated : IGenerator
    {
        private readonly Module _module;
        private readonly Model _model;

        public ControllerGenerated(ISettings settings, Module module, Model model)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _module = module ?? throw new ArgumentNullException(nameof(module));
            _model = model ?? throw new ArgumentNullException(nameof(model));
        }

        public ISettings Settings { get; }

        public IOutput Create()
        {
            if (!_model.IsEntity())
                return null;

            var i1 = h.Indent(1);
            var i2 = h.Indent(2);
            var i3 = h.Indent(3);
            var i4 = h.Indent(4);
            var i5 = h.Indent(5);

            var sb = new StringBuilder();
            sb.AppendLine(((ISnippet)new Header.Generator(Settings, new GeneratorDetails()).Create()).Content);
            sb.AppendLine("using System.Linq;");
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine($"using {_module.Namespace}.Dto;");
            sb.AppendLine($"using {_module.Namespace}.Interfaces;");
            sb.AppendLine("using Jbssa.Core.Interfaces;");
            sb.AppendLine("using Jbssa.Core.Mvc;");
            sb.AppendLine("using Kendo.Mvc.Extensions;");
            sb.AppendLine("using Kendo.Mvc.UI;");
            sb.AppendLine("using Microsoft.AspNetCore.Authorization;");
            sb.AppendLine("using Microsoft.AspNetCore.Http;");
            sb.AppendLine("using Microsoft.AspNetCore.JsonPatch;");
            sb.AppendLine("using Microsoft.AspNetCore.Mvc;");
            sb.AppendLine();
            sb.AppendLine($"namespace {_module.Namespace}.Api.Controllers");
            sb.AppendLine("{");
            if(Settings.SupportRegen)
            {
                sb.AppendLine($"{i1}partial class {_model.Name.Plural.Value}ApiController");
                sb.AppendLine($"{i1}{{");
            }
            else
            {
                sb.AppendLine($"{i1}// todo: fix api security");
                sb.AppendLine($"{i1}//[Authorize(Security.SupportedPolicies.{_model.Name.Plural.Value})]");
                sb.AppendLine($"{i1}[Route(\"api/{_model.Name.Plural.Value}\")]");
                sb.AppendLine($"{i1}public class {_model.Name.Plural.Value}ApiController : ApiBaseAsync<Domain.{_model.Name.Singular.Value}>");
                sb.AppendLine($"{i1}{{");
            }
            sb.AppendLine($"{i2}const string _displayName = \"{_model.Name.Singular.Value}\";");
            sb.AppendLine($"{i2}const string GetItemRouteName = \"Get{_model.Name.Singular.Value}\";");
            sb.AppendLine();
            sb.AppendLine($"{i2}private readonly IControllerMapper _mapper;");
            sb.AppendLine($"{i2}private readonly string _controllerOptions = \"OPTIONS\";");
            sb.AppendLine();
            if (!Settings.SupportRegen)
            {
                sb.AppendLine($"{i2}public {_model.Name.Plural.Value}ApiController(IEntityOptionsAsync<Domain.{_model.Name.Singular.Value}> options)");
                sb.AppendLine($"{i3}: base(options)");
                sb.AppendLine($"{i2}{{");
                sb.AppendLine($"{i3}if (options.ReadService != null)");
                sb.AppendLine($"{i4}_controllerOptions += \", GET\";");
                sb.AppendLine($"{i3}if (options.EditService != null)");
                sb.AppendLine($"{i4}_controllerOptions += \", POST, PUT, PATCH, DELETE\";");
                sb.AppendLine($"{i3}if (_options is IHasMapper o)");
                sb.AppendLine($"{i4}_mapper = o.GetMapper();");
                sb.AppendLine($"{i3}if (_mapper == null)");
                sb.AppendLine($"{i4}throw new ApplicationException($\"Missing required Mapping component in {{DisplayName}}\");");
                sb.AppendLine($"{i2}}}");
            }
            else
            {
                sb.AppendLine($"{i2}partial void FilterMapping(ref DataSourceRequest request);");
                sb.AppendLine($"{i2}partial void GetDisplayName(ref string name);");
                sb.AppendLine($"{i2}partial void Create(Domain.{_model.Name.Singular.Value} entity, ref bool handled);");
            }
            sb.AppendLine();
            sb.AppendLine($"{i2}internal string DisplayName");
            sb.AppendLine($"{i2}{{");
            sb.AppendLine($"{i3}get");
            sb.AppendLine($"{i3}{{");
            sb.AppendLine($"{i4}var name = _displayName;");
            sb.AppendLine($"{i4}GetDisplayName(ref name);");
            sb.AppendLine($"{i4}return name;");
            sb.AppendLine($"{i3}}}");
            sb.AppendLine($"{i2}}}");
            sb.AppendLine();
            sb.AppendLine($"{i2}[Route(\"Lookup\")]");
            sb.AppendLine($"{i2}[HttpGet]");
            sb.AppendLine($"{i2}public async Task<IActionResult> GetLookup()");
            sb.AppendLine($"{i2}{{");
            sb.AppendLine($"{i3}if(!(_options is IHasListProvider<Domain.{_model.Name.Singular.Value}> options))");
            sb.AppendLine($"{i4}return BadRequest();");
            sb.AppendLine();
            sb.AppendLine($"{i3}var data = await options.GetListProvider().GetItemsAsync(t => t.ToString());");
            sb.AppendLine($"{i3}return Ok(data);");
            sb.AppendLine($"{i2}}}");
            sb.AppendLine();
            sb.AppendLine($"{i2}[Route(\"Create\")]");
            sb.AppendLine($"{i2}[HttpGet]");
            sb.AppendLine($"{i2}public async Task<IActionResult> GetEmptyModel()");
            sb.AppendLine($"{i2}{{");
            if (Settings.SupportRegen)
            {
                sb.AppendLine($"{i3}Domain.{_model.Name.Singular.Value} entity = null;");
                sb.AppendLine($"{i3}var handled = false;");
                sb.AppendLine($"{i3}Create(entity, ref handled);");
                sb.AppendLine($"{i3}if (!handled)");
                sb.AppendLine($"{i4}entity = await CreateInternal();");
            }
            else
            {
                sb.AppendLine($"{i3}var entity = await CreateInternal();");
            }
            sb.AppendLine();
            sb.AppendLine($"{i3}return Ok(_mapper.Map<{_model.Name.Singular.Value}Response>(entity));");
            sb.AppendLine($"{i2}}}");
            sb.AppendLine();
            sb.AppendLine($"{i2}[HttpGet]");
            sb.AppendLine($"{i2}public async Task<IActionResult> Get([DataSourceRequest]DataSourceRequest request)");
            sb.AppendLine($"{i2}{{");
            sb.AppendLine($"{i3}FilterMapping(ref request);");
            sb.AppendLine($"{i3}return Ok(await base.GetInternal(request));");
            sb.AppendLine($"{i2}}}");
            sb.AppendLine();
            sb.AppendLine($"{i2}[HttpGet(\"{{id}}\", Name = GetItemRouteName)]");
            sb.AppendLine($"{i2}public async Task<IActionResult> Get(string id)");
            sb.AppendLine($"{i2}{{");
            sb.AppendLine($"{i3}var entity = await GetItemInternal(id);");
            sb.AppendLine($"{i3}if (entity == null)");
            sb.AppendLine($"{i4}return NotFound(MessageHelper.GetMessage(MessageType.NotFound, DisplayName, id));");
            sb.AppendLine();
            sb.AppendLine($"{i3}return Ok(_mapper.Map<{_model.Name.Singular.Value}Response>(entity));");
            sb.AppendLine($"{i2}}}");
            sb.AppendLine();
            sb.AppendLine($"{i2}[HttpPost]");
            sb.AppendLine($"{i2}public async Task<IActionResult> Create{_model.Name.Singular.Value}([FromBody]{_model.Name.Singular.Value}CreateRequest request)");
            sb.AppendLine($"{i2}{{");
            sb.AppendLine($"{i3}if (request == null)");
            sb.AppendLine($"{i4}return BadRequest($\"Missing the create {{DisplayName}} request body.\");");
            sb.AppendLine();
            sb.AppendLine($"{i3}var entity = _mapper.Map<Domain.{_model.Name.Singular.Value}>(request);");
            sb.AppendLine();
            sb.AppendLine($"{i3}var result = await AddInternal(entity);");
            sb.AppendLine($"{i3}var affected = await SaveInternalAsync();");
            sb.AppendLine($"{i3}if (affected == 0 || result == null)");
            sb.AppendLine($"{i4}throw new Jbssa.Core.NoRecordsUpdatedException(DisplayName);");
            sb.AppendLine();
            sb.AppendLine($"{i3}return CreatedAtRoute(GetItemRouteName, new {{ id = entity.BusinessKey }}, _mapper.Map<{_model.Name.Singular.Value}Response>(result));");
            sb.AppendLine($"{i2}}}");
            sb.AppendLine();
            sb.AppendLine($"{i2}[HttpPost(\"id\")]");
            sb.AppendLine($"{i2}public async Task<IActionResult> BlockCreate{_model.Name.Singular.Value}(string id)");
            sb.AppendLine($"{i2}{{");
            sb.AppendLine($"{i3}return await ExistsInternal(id) ? (IActionResult)Conflict() : (IActionResult)NotFound();");
            sb.AppendLine($"{i2}}}");
            sb.AppendLine();
            sb.AppendLine($"{i2}[HttpDelete(\"{{id}}\")]");
            sb.AppendLine($"{i2}public async Task<IActionResult> Remove(string id)");
            sb.AppendLine($"{i2}{{");
            sb.AppendLine($"{i3}var oldModel = await GetItemInternal(id);");
            sb.AppendLine($"{i3}if (oldModel == null)");
            sb.AppendLine($"{i4}return NotFound(MessageHelper.GetMessage(MessageType.NotFound, DisplayName, id));");
            sb.AppendLine();
            sb.AppendLine($"{i3}await DeleteInternal(oldModel);");
            sb.AppendLine();
            sb.AppendLine($"{i3}var affected = await SaveInternalAsync();");
            sb.AppendLine($"{i3}if (affected == 0)");
            sb.AppendLine($"{i4}throw new Jbssa.Core.NoRecordsUpdatedException(DisplayName);");
            sb.AppendLine();
            sb.AppendLine($"{i3}return NoContent();");
            sb.AppendLine($"{i2}}}");
            sb.AppendLine();
            sb.AppendLine($"{i2}[HttpPut(\"{{id}}\")]");
            sb.AppendLine($"{i2}public async Task<IActionResult> Put(string id, [FromBody]{_model.Name.Singular.Value}UpdateRequest model)");
            sb.AppendLine($"{i2}{{");
            sb.AppendLine($"{i3}if (model == null)");
            sb.AppendLine($"{i4}return BadRequest();");
            sb.AppendLine();
            sb.AppendLine($"{i3}if (!ModelState.IsValid)");
            sb.AppendLine($"{i4}return new UnprocessableEntityObjectResult(ModelState);");
            sb.AppendLine();
            sb.AppendLine($"{i3}var entity = _mapper.Map<Domain.{_model.Name.Singular.Value}>(model);");
            sb.AppendLine();
            sb.AppendLine($"{i3}var updateAction = await UpdateInternal(id, entity);");
            sb.AppendLine($"{i3}if (updateAction.Result == null || updateAction.UpdateResultType == UpdateResultType.None)");
            sb.AppendLine($"{i4}return NotFound(MessageHelper.GetMessage(MessageType.NotFound, DisplayName, id));");
            sb.AppendLine();
            sb.AppendLine($"{i3}var affected = await SaveInternalAsync();");
            sb.AppendLine($"{i3}if (affected == 0)");
            sb.AppendLine($"{i4}throw new Jbssa.Core.NoRecordsUpdatedException(DisplayName);");
            sb.AppendLine();
            sb.AppendLine($"{i3}var data = _mapper.Map<{_model.Name.Singular.Value}Response>(updateAction.Result);");
            sb.AppendLine($"{i3}return updateAction.UpdateResultType == UpdateResultType.Inserted ? (IActionResult)CreatedAtRoute(GetItemRouteName, new {{ id }}, data) : (IActionResult)Ok(data);");
            sb.AppendLine($"{i2}}}");
            sb.AppendLine();
            sb.AppendLine($"{i2}[HttpPatch(\"{{id}}\")]");
            sb.AppendLine($"{i2}public async Task<IActionResult> Patch(string id, [FromBody]JsonPatchDocument<{_model.Name.Singular.Value}UpdateRequest> document)");
            sb.AppendLine($"{i2}{{");
            sb.AppendLine($"{i3}if (document == null)");
            sb.AppendLine($"{i4}return BadRequest();");
            sb.AppendLine();
            sb.AppendLine($"{i3}var model = await GetItemInternal(id);");
            sb.AppendLine($"{i3}if (model == null)");
            sb.AppendLine($"{i4}return NotFound(MessageHelper.GetMessage(MessageType.NotFound, DisplayName, id));");
            sb.AppendLine();
            sb.AppendLine($"{i3}var modelToPatch = _mapper.Map<{_model.Name.Singular.Value}UpdateRequest>(model);");
            sb.AppendLine($"{i3}document.ApplyTo(modelToPatch, ModelState);");
            sb.AppendLine($"{i3}TryValidateModel(modelToPatch);");
            sb.AppendLine($"{i3}if (!ModelState.IsValid)");
            sb.AppendLine($"{i4}return new UnprocessableEntityObjectResult(ModelState);");
            sb.AppendLine();
            sb.AppendLine($"{i3}var entity = _mapper.Map<Domain.{_model.Name.Singular.Value}>(modelToPatch);");
            sb.AppendLine($"{i3}var updateAction = await UpdateInternal(id, entity);");
            sb.AppendLine($"{i3}if (updateAction.Result == null || updateAction.UpdateResultType == UpdateResultType.None)");
            sb.AppendLine($"{i4}return NotFound(MessageHelper.GetMessage(MessageType.NotFound, DisplayName, id));");
            sb.AppendLine();
            sb.AppendLine($"{i3}var affected = await SaveInternalAsync();");
            sb.AppendLine($"{i3}if (affected == 0)");
            sb.AppendLine($"{i4}throw new Jbssa.Core.NoRecordsUpdatedException(DisplayName);");
            sb.AppendLine();
            sb.AppendLine($"{i3}return Ok(_mapper.Map<{_model.Name.Singular.Value}Response>(entity));");
            sb.AppendLine($"{i2}}}");
            sb.AppendLine($"{i1}}}");
            sb.AppendLine("}");

            var name = $"{_model.Name.Plural.Value}Controller";
            if (Settings.SupportRegen)
                name += ".generated";
            return new File { Name = $"{name}.cs", Content = sb.ToString(), CanOverwrite = Settings.SupportRegen };
        }
    }
}