using Microsoft.AspNetCore.Mvc;
using Web.Models;
using ApplicationCore.Consts;
using Infrastructure.Helpers;
using ApplicationCore.Authorization;
using ApplicationCore.Helpers;
using ApplicationCore.Services;
using ApplicationCore.Models;
using Ardalis.Specification;
using AutoMapper;
using ApplicationCore.Views;

namespace Web.Controllers.Admin;

public class AppsController : BaseAdminController
{
   private readonly IAppService _appService;
   private readonly IRolesService _rolesService;
   private readonly IMapper _mapper;

   public AppsController(IAppService appService, IRolesService rolesService, IMapper mapper)
   {
      _appService = appService;
      _rolesService = rolesService;
      _mapper = mapper;
   }

   [HttpGet("init")]
   public async Task<ActionResult<AppsIndexModel>> Init()
   {
      string type = AppTypes.Spa;
      var request = new AppsFetchRequest(type);

      var roles = await _rolesService.FetchAsync();

      return new AppsIndexModel(request, roles.MapViewModelList(_mapper));
   }

   [HttpGet]   
   public async Task<ActionResult<ICollection<AppViewModel>>> Index(string type)
   {
      ValidateType(type); 
      if (!ModelState.IsValid) return BadRequest(ModelState);

      var apps = await _appService.FetchByTypeAsync(type);
      return apps.MapViewModelList(_mapper);
   }
   [HttpGet("create")]
   public async Task<ActionResult<AppsEditRequest>> Create()
   {
      var model = new AppsEditRequest(new AppAddForm());
      var apiApps = await _appService.FetchByTypeAsync(AppTypes.Api);

      model.ApiOptions = apiApps.MapOptionList();

      return model;
   }

   [HttpPost]
   public async Task<ActionResult> Store([FromBody] AppAddForm form)
   {
      await ValidateRequestAsync(form);
      if (!ModelState.IsValid) return BadRequest(ModelState);

      var app = new App();
      form.SetValuesTo(app);
      app.Roles = form.Roles.JoinToString();
      app.SetCreated(User.Id());

      if (form.Type.EqualTo(AppTypes.Api))
      {
         app = await _appService.CreateApiAsync(app);
      }
      else
      {
         var apiApps = new List<App>();
         if(form.Apis.HasItems()) apiApps = (await _appService.FetchByIdsAsync(form.Apis)).ToList();
         app = await _appService.CreateSpaAsync(app, apiApps);
      }

      return Ok();
   }
   [HttpGet("edit/{id}")]
   public async Task<ActionResult<AppsEditRequest>> Edit(int id)
   {
      var app = await _appService.GetByIdAsync(id);
      if (app == null) return NotFound();

      var form = new AppEditForm();
      app.SetValuesTo(form);
      form.Roles = app.Roles.SplitToList();

      var apiApps = await _appService.FetchByTypeAsync(AppTypes.Api);
      var hasPermissionApis = await _appService.GetPermissionApisAsync(app, apiApps);
      form.Apis = hasPermissionApis.Select(x => x.Id).ToList();

      var model = new AppsEditRequest(form);
      model.ApiOptions = apiApps.MapOptionList();
      return model;
   }
   [HttpPut("{id}")]
   public async Task<IActionResult> Update(int id, [FromBody] AppEditForm form)
   {
      var app = await _appService.GetByIdAsync(id);
      if (app == null) return NotFound();

      await ValidateRequestAsync(form, id);
      if (!ModelState.IsValid) return BadRequest(ModelState);

      form.SetValuesTo(app); 
      app.Roles = form.Roles.JoinToString();
      app.SetUpdated(User.Id());

      if (app.Type.EqualTo(AppTypes.Api))
      {
         await _appService.UpdateApiAsync(app);
      }
      else
      {
         var apiApps = new List<App>();
         if (form.Apis.HasItems()) apiApps = (await _appService.FetchByIdsAsync(form.Apis)).ToList();
         await _appService.UpdateSpaAsync(app, apiApps);
      }
      return NoContent();
   }
   [HttpPut("reset-client-secret/{id}")]
   public async Task<IActionResult> ResetClientSecret(int id)
   {
      var app = await _appService.GetByIdAsync(id);
      if (app == null) return NotFound();

      if (!app.Type.EqualTo(AppTypes.Api))
      {
         ModelState.AddModelError("type", "AppType Not Valid.");
         return BadRequest(ModelState);
      }

      await _appService.ResetClientSecretAsync(app);
      return NoContent();
   }
   [HttpDelete("{id}")]
   public async Task<IActionResult> Remove(int id)
   {
      var app = await _appService.GetByIdAsync(id);
      if (app == null) return NotFound();

      await _appService.RemoveAsync(app, User.Id());
     
      return NoContent();
   }
   
   async Task ValidateRequestAsync(BaseAppForm model, int id = 0)
   {
      var labels = new AppLabels();
      if (string.IsNullOrEmpty(model.ClientId))
      {
         ModelState.AddModelError(nameof(model.ClientId), ValidationMessages.Required(labels.ClientId));
      }

      ValidateType(model.Type);
      
      ValidateUrl(model);

      if (!ModelState.IsValid) return;

      var existApp = await _appService.FindByClientIdAsync(model.ClientId);
      if (existApp is not null && existApp.Id != id)
      {
         ModelState.AddModelError(nameof(model.ClientId), ValidationMessages.Duplicate(labels.ClientId));
      }      
   }

   void ValidateType(string type)
   {
      if (string.IsNullOrEmpty(type))
      {
         ModelState.AddModelError("type", ValidationMessages.Required(type));
      }

      if (type.EqualTo(AppTypes.Spa) || type.EqualTo(AppTypes.Api)) return;
      ModelState.AddModelError("type", ValidationMessages.NotExist("type"));
      
   }
   void ValidateUrl(BaseAppForm model)
   {
      if (string.IsNullOrEmpty(model.Url))
      {
         ModelState.AddModelError(nameof(model.Url), ValidationMessages.Required("Url"));
      }
      if (!model.Url!.IsValidUrl())
      {
         ModelState.AddModelError(nameof(model.Url), ValidationMessages.WrongFormatOf("Url"));
      }

   }
}
