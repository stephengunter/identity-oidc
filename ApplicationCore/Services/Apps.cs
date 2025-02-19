using ApplicationCore.Consts;
using ApplicationCore.DataAccess;
using ApplicationCore.Exceptions;
using ApplicationCore.Helpers;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using Infrastructure.Helpers;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace ApplicationCore.Services;

public interface IAppService
{
   Task<IEnumerable<App>> FetchAsync();
   Task<IEnumerable<App>> FetchByTypeAsync(string type);
   Task<IEnumerable<App>> FetchByIdsAsync(ICollection<int> ids);
   Task<IEnumerable<App>> GetPermissionApisAsync(App app, IEnumerable<App>? apiApps = null);
   Task<App?> FindByClientIdAsync(string clientId);
   Task<App?> GetByIdAsync(int id);
   Task<App> CreateSpaAsync(App entity, ICollection<App> apis);
   Task<App> CreateApiAsync(App entity);
   Task UpdateSpaAsync(App entity, ICollection<App> apis);
   Task UpdateApiAsync(App entity);
   Task ResetClientSecretAsync(App entity);
   Task<bool> ValidateClientSecretAsync(App entity);
   Task RemoveAsync(App entity, string userId);
   string GetDecryptClientSecret(App entity);
}

public class AppService : IAppService
{
   private readonly IDefaultRepository<App> _appRepository;
   private readonly IOpenIddictApplicationManager _applicationManager;
   private readonly ICryptoService _cryptoService;
   public AppService(IDefaultRepository<App> appRepository, IOpenIddictApplicationManager applicationManager,
      ICryptoService cryptoService)
   {
      _appRepository = appRepository;
      _applicationManager = applicationManager; 
      _cryptoService = cryptoService;
   }
   public async Task<IEnumerable<App>> FetchAsync()
       => await _appRepository.ListAsync(new AppsSpecification());

   public async Task<IEnumerable<App>> FetchByTypeAsync(string type)
   {
      var apps = await FetchAsync();
      return apps.Where(x => x.Type.EqualTo(type));
   }
   public async Task<IEnumerable<App>> GetPermissionApisAsync(App app, IEnumerable<App>? apiApps = null)
   {
      var application = await GetApplicationAsync(app);
      if (application == null) throw new ApplicationNotExistException(app);

      if(apiApps.IsNullOrEmpty()) apiApps = await FetchByTypeAsync(AppTypes.Api);

      var hasPermissionApis = new List<App>();
      foreach (var api in apiApps)
      {
         if (await _applicationManager.HasPermissionAsync(application!, OpenIddictConstants.Permissions.Prefixes.Scope + api.ClientId))
         {
            hasPermissionApis.Add(api);
         }
      }
      return hasPermissionApis;
   }
   public async Task<IEnumerable<App>> FetchByIdsAsync(ICollection<int> ids)
       => await _appRepository.ListAsync(new AppsSpecification(ids));

   public async Task<App?> FindByClientIdAsync(string clientId)
       => await _appRepository.FirstOrDefaultAsync(new AppsSpecification(clientId));

   public async Task<App?> GetByIdAsync(int id)
      => await _appRepository.FirstOrDefaultAsync(new AppsSpecification(id));

   public async Task<App> CreateSpaAsync(App entity, ICollection<App> apis)
   {
      entity.Encrypt = "";
      entity = await _appRepository.AddAsync(entity);

      var descriptor = await CreateDescriptorAsync(entity);
      if (apis.HasItems())
      {
         var apiClientIds = apis.Select(x => x.ClientId).ToList();
         descriptor.Permissions.UnionWith(OidcHelper.GetPublicPermissions(apiClientIds!));
      }
      await _applicationManager.CreateAsync(descriptor);

      return entity;
   }
   public async Task<App> CreateApiAsync(App entity)
   {
      string clientSecret = Guid.NewGuid().ToString();
      entity.Encrypt = _cryptoService.Encrypt(clientSecret);
      entity.Type = AppTypes.Api;
      entity.Roles = "";
      entity = await _appRepository.AddAsync(entity);

      var descriptor = await CreateDescriptorAsync(entity);
      descriptor.ClientSecret = clientSecret;

      await _applicationManager.CreateAsync(descriptor);
      return entity;
   }
   
   public async Task UpdateSpaAsync(App entity, ICollection<App> apis)
   {
      var application = await GetApplicationAsync(entity);
      if (application == null) throw new ApplicationNotExistException(entity);

      await _appRepository.UpdateAsync(entity);

      var descriptor = await CreateDescriptorAsync(entity, application);
      if (apis.HasItems())
      {
         var apiClientIds = apis.Select(x => x.ClientId).ToList();
         descriptor.Permissions.UnionWith(OidcHelper.GetPublicPermissions(apiClientIds!));
      }
      await _applicationManager.UpdateAsync(application, descriptor);
   }

   public async Task UpdateApiAsync(App entity)
   {
      var application = await GetApplicationAsync(entity);
      if (application == null) throw new ApplicationNotExistException(entity);

      string clientSecret = GetDecryptClientSecret(entity);
      if (string.IsNullOrEmpty(clientSecret))
      {
         clientSecret = Guid.NewGuid().ToString();
         entity.Encrypt = _cryptoService.Encrypt(clientSecret);
      }
      await _appRepository.UpdateAsync(entity);
     
      var descriptor = await CreateDescriptorAsync(entity, application);
      descriptor.ClientSecret = clientSecret;

      await _applicationManager.UpdateAsync(application, descriptor);
   }

   public async Task ResetClientSecretAsync(App entity)
   {
      entity.Encrypt = "";
      await UpdateApiAsync(entity);
   }

   public async Task RemoveAsync(App entity, string userId)
   {
      var application = await GetApplicationAsync(entity);
      if (application == null) throw new ApplicationNotExistException(entity);

      entity.Removed = true;
      entity.SetUpdated(userId);
      await _appRepository.UpdateAsync(entity);

      await _applicationManager.DeleteAsync(application);
   }

   public string GetDecryptClientSecret(App entity)
   {
      if (string.IsNullOrEmpty(entity.Encrypt)) return "";
      return _cryptoService.Decrypt(entity.Encrypt!);
   }

   public async Task<bool> ValidateClientSecretAsync(App entity)
   {
      var application = await GetApplicationAsync(entity);
      if (application == null) throw new ApplicationNotExistException(entity);

      string clientSecret = GetDecryptClientSecret(entity);
      return await _applicationManager.ValidateClientSecretAsync(application, clientSecret);
   }

   async Task<object?> GetApplicationAsync(App app) 
      => await _applicationManager.FindByClientIdAsync(app.ClientId);


   async Task<OpenIddictApplicationDescriptor> CreateDescriptorAsync(App entity, object? application = null)
   {
      var descriptor = new OpenIddictApplicationDescriptor();
      if (application is not null)
      {
         await _applicationManager.PopulateAsync(descriptor, application);
      }
      descriptor.ClientId = entity.ClientId;
      descriptor.ClientType = entity.Type.EqualTo(AppTypes.Spa) ? ClientTypes.Public : ClientTypes.Confidential;
      descriptor.DisplayName = entity.Name;

      if (entity.Type.EqualTo(AppTypes.Spa))
      {
         descriptor.RedirectUris.Clear();
         descriptor.RedirectUris.UnionWith(OidcHelper.GetRedirectUris(entity.Url));

         descriptor.Permissions.Clear();
         //descriptor.Permissions.UnionWith(OidcHelper.GetPublicPermissions(model.Apis));

         descriptor.Requirements.Clear();
         descriptor.Requirements.UnionWith(OidcHelper.GetPublicRequirements());
      }
      else
      {
         // Api
         descriptor.RedirectUris.Clear();

         descriptor.Permissions.Clear();
         descriptor.Permissions.UnionWith(OidcHelper.GetApiPermissions());

         descriptor.Requirements.Clear();
         descriptor.Requirements.UnionWith(OidcHelper.GetApiRequirements());
      }

      return descriptor;
   }

}
