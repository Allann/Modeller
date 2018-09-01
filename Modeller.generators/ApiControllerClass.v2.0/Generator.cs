using System;
using System.Text;
using Modeller;
using Modeller.Core;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace ApiController
{
    public class Generator : IGenerator
    {
        private readonly Module _module;
        private readonly Model _model;

        public Generator(ISettings settings, Module module, Model model)
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

            var sb = new StringBuilder();
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine("using AutoMapper;");
            sb.AppendLine("using Jbssa.Core.Interfaces;");
            sb.AppendLine("using Jbssa.Core.Mvc;");
            sb.AppendLine("using Kendo.Mvc.Extensions;");
            sb.AppendLine("using Kendo.Mvc.UI;");
            sb.AppendLine("using Microsoft.AspNetCore.Authorization;");
            sb.AppendLine("using Microsoft.AspNetCore.Http;");
            sb.AppendLine("using Microsoft.AspNetCore.JsonPatch;");
            sb.AppendLine("using Microsoft.AspNetCore.Mvc;");
            sb.AppendLine("using System.Linq;");
            sb.AppendLine($"using {_module.Namespace}.Dto;");
            sb.AppendLine();
            sb.AppendLine($"namespace {_module.Namespace}.Api.Controllers");
            sb.AppendLine("{");
            sb.AppendLine($"{i1}// todo: fix api security");
            sb.AppendLine($"{i1}//[Authorize(Security.SupportedPolicies.{_model.Name.Plural.Value})]");
            sb.AppendLine($"{i1}[Route(\"api/{_model.Name.Plural.Value}\")]");
            sb.AppendLine($"{i1}public class {_model.Name.Plural.Value}ApiController : ApiBaseAsync<Domain.{_model.Name.Singular.Value}>");
            sb.AppendLine($"{i1}{{");
            sb.AppendLine($"{i1}    const string GetItemRouteName = \"Get{_model.Name.Singular.Value}\";");
            sb.AppendLine($"{i1}    private readonly IMapper _mapper;");
            sb.AppendLine();
            sb.AppendLine($"{i1}    public {_model.Name.Plural.Value}ApiController(IEntityOptionsAsync<Domain.{_model.Name.Singular.Value}> options, IMapper mapper)");
            sb.AppendLine($"{i1}        : base(options)");
            sb.AppendLine($"{i1}    {{");
            sb.AppendLine($"{i1}        _mapper = mapper;");
            sb.AppendLine($"{i1}    }}");
            sb.AppendLine();
            sb.AppendLine($"{i1}    [Route(\"Create\")]");
            sb.AppendLine($"{i1}    [HttpGet]");
            sb.AppendLine($"{i1}    public async Task<IActionResult> GetEmptyModel()");
            sb.AppendLine($"{i1}    {{");
            sb.AppendLine($"{i1}        var entity = await CreateInternal();");
            sb.AppendLine($"{i1}        var data = _mapper.Map<{_model.Name.Singular.Value}Response>(entity);");
            sb.AppendLine($"{i1}        return Ok(data);");
            sb.AppendLine($"{i1}    }}");
            sb.AppendLine();
            sb.AppendLine($"{i1}    [Route(\"Lookup\")]");
            sb.AppendLine($"{i1}    [HttpGet]");
            sb.AppendLine($"{i1}    public async Task<IActionResult> GetLookup()");
            sb.AppendLine($"{i1}    {{");
            sb.AppendLine($"{i1}        if(!(_options is IHasListProvider<Domain.{_model.Name.Singular.Value}> options))");
            sb.AppendLine($"{i1}            return BadRequest();");
            sb.AppendLine();
            sb.AppendLine($"{i1}        var data = await options.GetListProvider().GetItemsAsync(t => t.ToString());");
            sb.AppendLine($"{i1}        return Ok(data);");
            sb.AppendLine($"{i1}    }}");
            sb.AppendLine();
            sb.AppendLine($"{i1}    [HttpGet]");
            sb.AppendLine($"{i1}    public async Task<IActionResult> Get([DataSourceRequest]DataSourceRequest request)");
            sb.AppendLine($"{i1}    {{");
            sb.AppendLine($"{i1}        return Ok(await base.GetInternal(request));");
            sb.AppendLine($"{i1}    }}");
            sb.AppendLine();
            sb.AppendLine($"{i1}    [HttpGet(\"{{id}}\", Name = GetItemRouteName)]");
            sb.AppendLine($"{i1}    public async Task<IActionResult> Get(string id)");
            sb.AppendLine($"{i1}    {{");
            sb.AppendLine($"{i1}        var entity = await GetItemInternal(id);");
            sb.AppendLine($"{i1}        if (entity == null)");
            sb.AppendLine($"{i1}            return NotFound(MessageHelper.GetMessage(MessageType.NotFound, \"{_model.Name.Singular.Value}\", id));");
            sb.AppendLine();
            sb.AppendLine($"{i1}        var data = _mapper.Map<{_model.Name.Singular.Value}Response>(entity);");
            sb.AppendLine($"{i1}        return Ok(data);");
            sb.AppendLine($"{i1}    }}");
            sb.AppendLine();
            sb.AppendLine($"{i1}    [HttpPost]");
            sb.AppendLine($"{i1}    public async Task<IActionResult> Create{_model.Name.Singular.Value}([FromBody]{_model.Name.Singular.Value}CreateRequest model)");
            sb.AppendLine($"{i1}    {{");
            sb.AppendLine($"{i1}        if (model == null)");
            sb.AppendLine($"{i1}            return BadRequest(\"Missing the create {_model.Name.Singular.Display} request body.\");");
            sb.AppendLine();
            sb.AppendLine($"{i1}        var entity = _mapper.Map<Domain.{_model.Name.Singular.Value}>(model);");
            sb.AppendLine($"{i1}        var result = await AddInternal(entity);");
            sb.AppendLine();
            sb.AppendLine($"{i1}        var affected = await SaveInternalAsync();");
            sb.AppendLine($"{i1}        if (affected == 0 || result == null)");
            sb.AppendLine($"{i1}            throw new Jbssa.Core.NoRecordsUpdatedException(\"{_model.Name.Singular.Display}\");");
            sb.AppendLine();
            sb.AppendLine($"{i1}        var data = _mapper.Map<{_model.Name.Singular.Value}Response>(result);");
            sb.AppendLine($"{i1}        return CreatedAtRoute(GetItemRouteName, new {{ id = entity.BusinessKey }}, data);");
            sb.AppendLine($"{i1}    }}");
            sb.AppendLine();
            sb.AppendLine($"{i1}    [HttpPost(\"id\")]");
            sb.AppendLine($"{i1}    public async Task<IActionResult> BlockCreate{_model.Name.Singular.Value}(string id)");
            sb.AppendLine($"{i1}    {{");
            sb.AppendLine($"{i1}        return await ExistsInternal(id) ? new StatusCodeResult(StatusCodes.Status409Conflict) : NotFound();");
            sb.AppendLine($"{i1}    }}");
            sb.AppendLine();
            sb.AppendLine($"{i1}    [HttpDelete(\"{{id}}\")]");
            sb.AppendLine($"{i1}    public async Task<IActionResult> Remove(string id)");
            sb.AppendLine($"{i1}    {{");
            sb.AppendLine($"{i1}        var oldModel = await GetItemInternal(id);");
            sb.AppendLine($"{i1}        if (oldModel == null)");
            sb.AppendLine($"{i1}            return NotFound(MessageHelper.GetMessage(MessageType.NotFound, \"{_model.Name.Singular.Display}\", id));");
            sb.AppendLine();
            sb.AppendLine($"{i1}        await DeleteInternal(oldModel);");
            sb.AppendLine();
            sb.AppendLine($"{i1}        var affected = await SaveInternalAsync();");
            sb.AppendLine($"{i1}        if (affected == 0)");
            sb.AppendLine($"{i1}            throw new Jbssa.Core.NoRecordsUpdatedException(\"{_model.Name.Singular.Display}\");");
            sb.AppendLine();
            sb.AppendLine($"{i1}        return NoContent();");
            sb.AppendLine($"{i1}    }}");
            sb.AppendLine();
            sb.AppendLine($"{i1}    [HttpPut(\"{{id}}\")]");
            sb.AppendLine($"{i1}    public async Task<IActionResult> Put(string id, [FromBody]{_model.Name.Singular.Value}UpdateRequest model)");
            sb.AppendLine($"{i1}    {{");
            sb.AppendLine($"{i1}        if (model == null)");
            sb.AppendLine($"{i1}            return BadRequest();");
            sb.AppendLine();
            sb.AppendLine($"{i1}        if (!ModelState.IsValid)");
            sb.AppendLine($"{i1}            return new UnprocessableEntityObjectResult(ModelState);");
            sb.AppendLine();
            sb.AppendLine($"{i1}        var entity = _mapper.Map<Domain.{_model.Name.Singular.Value}>(model);");
            sb.AppendLine();
            sb.AppendLine($"{i1}        var updateAction = await UpdateInternal(id, entity);");
            sb.AppendLine($"{i1}        if (updateAction.Result == null || updateAction.UpdateResultType == UpdateResultType.None)");
            sb.AppendLine($"{i1}            return NotFound(MessageHelper.GetMessage(MessageType.NotFound, \"{_model.Name.Singular.Display}\", id));");
            sb.AppendLine();
            sb.AppendLine($"{i1}        var affected = await SaveInternalAsync();");
            sb.AppendLine($"{i1}        if (affected == 0)");
            sb.AppendLine($"{i1}            throw new Jbssa.Core.NoRecordsUpdatedException(\"{_model.Name.Singular.Display}\");");
            sb.AppendLine();
            sb.AppendLine($"{i1}        var data = _mapper.Map<{_model.Name.Singular.Value}Response>(updateAction.Result);");
            sb.AppendLine($"{i1}        return updateAction.UpdateResultType == UpdateResultType.Inserted ? CreatedAtRoute(GetItemRouteName, new {{ id }}, data) : (IActionResult)Ok(data);");
            sb.AppendLine($"{i1}    }}");
            sb.AppendLine();
            sb.AppendLine($"{i1}    [HttpPatch(\"{{id}}\")]");
            sb.AppendLine($"{i1}    public async Task<IActionResult> Patch(string id, [FromBody]JsonPatchDocument<{_model.Name.Singular.Value}UpdateRequest> document)");
            sb.AppendLine($"{i1}    {{");
            sb.AppendLine($"{i1}        if (document == null)");
            sb.AppendLine($"{i1}            return BadRequest();");
            sb.AppendLine();
            sb.AppendLine($"{i1}        var model = await GetItemInternal(id);");
            sb.AppendLine($"{i1}        if (model == null)");
            sb.AppendLine($"{i1}            return NotFound(MessageHelper.GetMessage(MessageType.NotFound, \"{_model.Name.Singular.Display}\", id));");
            sb.AppendLine();
            sb.AppendLine($"{i1}        var modelToPatch = _mapper.Map<{_model.Name.Singular.Value}UpdateRequest>(model);");
            sb.AppendLine($"{i1}        document.ApplyTo(modelToPatch, ModelState);");
            sb.AppendLine();
            sb.AppendLine($"{i1}        TryValidateModel(modelToPatch);");
            sb.AppendLine($"{i1}        if (!ModelState.IsValid)");
            sb.AppendLine($"{i1}            return new UnprocessableEntityObjectResult(ModelState);");
            sb.AppendLine();
            sb.AppendLine($"{i1}        _mapper.Map(modelToPatch, model);");
            sb.AppendLine();
            sb.AppendLine($"{i1}        var affected = await SaveInternalAsync();");
            sb.AppendLine($"{i1}        if (affected == 0)");
            sb.AppendLine($"{i1}            throw new Jbssa.Core.NoRecordsUpdatedException(\"{_model.Name.Singular.Display}\");");
            sb.AppendLine();
            sb.AppendLine($"{i1}        var data = _mapper.Map<{_model.Name.Singular.Value}Response>(model);");
            sb.AppendLine($"{i1}        return Ok(model);");
            sb.AppendLine($"{i1}    }}");
            sb.AppendLine();
            sb.AppendLine($"{i1}    [HttpOptions]");
            sb.AppendLine($"{i1}    public IActionResult Get{_model.Name.Singular.Value}Options()");
            sb.AppendLine($"{i1}    {{");
            sb.AppendLine($"{i1}        Response.Headers.Add(\"Allow\", \"GET,OPTIONS,POST,PUT,PATCH,DELETE\");");
            sb.AppendLine($"{i1}        return Ok();");
            sb.AppendLine($"{i1}    }}");
            sb.AppendLine($"{i1}}}");
            sb.AppendLine("}");

            return new File { Name = $"{_model.Name.Plural.Value}Controller.cs", Content = sb.ToString(), CanOverwrite = Settings.SupportRegen };
        }
    }
}