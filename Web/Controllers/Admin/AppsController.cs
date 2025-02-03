using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using ApplicationCore.Views.Oidc;
using Web.Models;
using ApplicationCore.Consts;
using Infrastructure.Helpers;
using static OpenIddict.Abstractions.OpenIddictConstants;
using Permissions = OpenIddict.Abstractions.OpenIddictConstants.Permissions;
using static System.Net.Mime.MediaTypeNames;
using ApplicationCore.Authorization;
using Microsoft.AspNetCore.Hosting.Server;
using System;

namespace Web.Controllers.Admin;

public class AppsController : BaseAdminController
{
   private readonly IOpenIddictApplicationManager _applicationManager;
   public AppsController(IOpenIddictApplicationManager applicationManager)
   {
      _applicationManager = applicationManager;
   }

   [HttpGet("init")]
   public async Task<ActionResult<AppsIndexModel>> Init()
   {
      string type = AppTypes.Spa;
      var request = new AppsFetchRequest(type);

      return new AppsIndexModel(request);
   }

   [HttpGet]   
   public async Task<ActionResult<ICollection<AppViewModel>>> Index(string type)
   {
      type = ValidateType(type);
      if (!ModelState.IsValid) return BadRequest(ModelState);

      var list = new List<AppViewModel>();
      await foreach (var application in _applicationManager.ListAsync())
      {
         if (await _applicationManager.GetClientTypeAsync(application) == type)
         {
            list.Add(new AppViewModel
            {
               Id = await _applicationManager.GetIdAsync(application) ?? "",
               ClientId = await _applicationManager.GetClientIdAsync(application) ?? "",
               ClientType = type,
               DisplayName = await _applicationManager.GetDisplayNameAsync(application) ?? "",
               RedirectUris = (await _applicationManager.GetRedirectUrisAsync(application)).ToList(),
               PostLogoutRedirectUris = (await _applicationManager.GetPostLogoutRedirectUrisAsync(application)).ToList()
            });
         }
      }
      return list;
   }
   [HttpGet("create")]
   public ActionResult<AppAddForm> Create()
   {
      var form = new AppAddForm();
      return form;
   }

   [HttpPost]
   public async Task<ActionResult<AppViewModel>> Store([FromBody] AppAddForm form)
   {
      await ValidateRequestAsync(form);
      if (!ModelState.IsValid) return BadRequest(ModelState);

      if (form.Type.EqualTo(ClientTypes.Public))
      {
         string url = form.Url!;
         if (!url.EndsWith("/")) url += "/";

         var descriptor = new OpenIddictApplicationDescriptor()
         {
            ClientId = form.ClientId,
            ClientType = form.Type,
            DisplayName = form.Title,
            RedirectUris =
            {
               new Uri(url),
               new Uri($"{url}{AppUris.SigninCallback}"),
               new Uri($"{url}{AppUris.SigninSilentCallback}")
            },
            Permissions =
            {
               Permissions.Endpoints.Authorization,
               //Permissions.Endpoints,
               Permissions.Endpoints.Token,
               Permissions.GrantTypes.AuthorizationCode,
               Permissions.GrantTypes.RefreshToken,
               Permissions.ResponseTypes.Code,
               Permissions.Scopes.Email,
               Permissions.Scopes.Profile,
               Permissions.Scopes.Roles,
               Permissions.Prefixes.Scope + "identity-api"
            },
            Requirements =
            {
               Requirements.Features.ProofKeyForCodeExchange,
            }
         };
         var application = await _applicationManager.CreateAsync(descriptor);
         return new AppViewModel
         {
            Id = await _applicationManager.GetIdAsync(application) ?? "",
            ClientId = await _applicationManager.GetClientIdAsync(application) ?? "",
            ClientType = await _applicationManager.GetClientTypeAsync(application) ?? "",
            DisplayName = await _applicationManager.GetDisplayNameAsync(application) ?? "",
            RedirectUris = (await _applicationManager.GetRedirectUrisAsync(application)).ToList(),
            PostLogoutRedirectUris = (await _applicationManager.GetPostLogoutRedirectUrisAsync(application)).ToList()
         };
      }

      return Ok();
   }
   [HttpGet("edit/{id}")]
   public async Task<ActionResult<AppEditForm>> Edit(string id)
   {
      var application = await _applicationManager.FindByIdAsync(id);
      if (application == null) return NotFound();

      var redirectUris = (await _applicationManager.GetRedirectUrisAsync(application)).ToList();
      var uri = redirectUris.FirstOrDefault();
      var type = (await _applicationManager.GetClientTypeAsync(application)) ?? "";
      return new AppEditForm
      {
         ClientId = await _applicationManager.GetClientIdAsync(application) ?? "",
         Title = await _applicationManager.GetDisplayNameAsync(application) ?? "",
         Url = uri!,
         Type = ConvertType(type)
      };
   }
   [HttpPut("{id}")]
   public async Task<IActionResult> Update(string id, [FromBody] AppEditForm form)
   {
      var application = await _applicationManager.FindByIdAsync(id);
      if (application == null) return NotFound();

      await ValidateRequestAsync(form, id);
      if (!ModelState.IsValid) return BadRequest(ModelState);

      var descriptor = CreateDescriptor(form);

      if (form.Type.EqualTo(ClientTypes.Public))
      {
         await _applicationManager.UpdateAsync(application, descriptor);
      }

      await _applicationManager.UpdateAsync(application, descriptor);
      //descriptor.ClientSecret = "846B62D0-DEF9-4215-A99D-86E6B8DAB342";

      //var descriptor = new OpenIddictApplicationDescriptor
      //{
      //   ClientId = form.ClientId,
      //   ClientType = form.Type,
      //   DisplayName = form.Title,
      //   RedirectUris = {},
      //   Permissions =
      //   {
      //      Permissions.Endpoints.Introspection
      //   }
      //};
      //await _applicationManager.PopulateAsync(descriptor, application);
      //await _applicationManager.UpdateAsync(application, descriptor);
      return NoContent();
   }
   OpenIddictApplicationDescriptor CreateDescriptor(BaseAppForm model)
   {
      if (model.Type.EqualTo(ClientTypes.Public))
      {
         string url = model.Url!;
         if (!url.EndsWith("/")) url += "/";
         return new OpenIddictApplicationDescriptor()
         {
            ClientId = model.ClientId,
            ClientType = model.Type,
            DisplayName = model.Title,
            RedirectUris =
            {
               new Uri(url),
               new Uri($"{url}{AppUris.SigninCallback}"),
               new Uri($"{url}{AppUris.SigninSilentCallback}")
            },
            Permissions =
            {
               Permissions.Endpoints.Authorization,
               //Permissions.Endpoints,
               Permissions.Endpoints.Token,
               Permissions.GrantTypes.AuthorizationCode,
               Permissions.GrantTypes.RefreshToken,
               Permissions.ResponseTypes.Code,
               Permissions.Scopes.Email,
               Permissions.Scopes.Profile,
               Permissions.Scopes.Roles,
               Permissions.Prefixes.Scope + "identity-api"
            },
            Requirements =
            {
               Requirements.Features.ProofKeyForCodeExchange,
            }
         };
      }
      // Api
      return new OpenIddictApplicationDescriptor
      {
         ClientId = model.ClientId,
         ClientType = model.Type,
         DisplayName = model.Title,
         RedirectUris = { },
         Permissions =
         {
            Permissions.Endpoints.Introspection
         }
      };
   }
   async Task ValidateRequestAsync(BaseAppForm model, string id = "")
   {
      var labels = new AppLabels();
      if (string.IsNullOrEmpty(model.ClientId))
      {
         ModelState.AddModelError(nameof(model.ClientId), ValidationMessages.Required(labels.ClientId));
      }

      model.Type = ValidateType(model.Type);
      ValidateUrl(model);

      if (!ModelState.IsValid) return;

      var existApp = await _applicationManager.FindByClientIdAsync(model.ClientId);
      if (existApp is not null)
      {
         if (string.IsNullOrEmpty(id))
         {
            // Duplicate ClientId
            ModelState.AddModelError(nameof(model.ClientId), ValidationMessages.Duplicate(labels.ClientId));
            return;
         }

         string existAppId = await _applicationManager.GetIdAsync(existApp) ?? "";
         if (existAppId != id)
         {
            // Duplicate ClientId
            ModelState.AddModelError(nameof(model.ClientId), ValidationMessages.Duplicate(labels.ClientId));
         }
      }      
   }

   string ValidateType(string type)
   {
      if (string.IsNullOrEmpty(type))
      {
         ModelState.AddModelError("type", ValidationMessages.Required(type));
         return "";
      }
      if (type.EqualTo(AppTypes.Spa)) return ClientTypes.Public;
      if (type.EqualTo(AppTypes.Api)) return ClientTypes.Confidential;


      ModelState.AddModelError("type", ValidationMessages.NotExist(type));
      return "";
   }
   void ValidateUrl(BaseAppForm model)
   {
      if (model.Type.EqualTo(ClientTypes.Public))
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
   string ConvertType(string type)
   {
      if (type.EqualTo(ClientTypes.Public)) return AppTypes.Spa;
      if (type.EqualTo(ClientTypes.Confidential)) return AppTypes.Api;
      return "";
   }
}
