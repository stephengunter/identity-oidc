using ApplicationCore.Consts;
using ApplicationCore.Models;
using ApplicationCore.Views;
using Infrastructure.Helpers;
using Infrastructure.Views;
using System;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Web.Models;

public class AppLabels
{
   public string Type => "類型";
   public string ClientId => "ClientId";
   public string Name => "名稱";
   public string Url => "Url";
   public string Api => "Api";
   public string Roles => "角色";
   public string Ps => "備註";
}
public class AppsIndexModel
{
   public AppsIndexModel(AppsFetchRequest request, List<RoleViewModel> roles)
   {
      Request = request;
      Roles = roles;
   }
   public AppLabels Labels => new AppLabels();
   public List<string> Types => ObjectsHelpers.GetStaticKeys<AppTypes>();
   public List<RoleViewModel> Roles { get; set; }
   public AppsFetchRequest Request { get; set; }
}

public class AppsFetchRequest
{
   public AppsFetchRequest(string type)
   {
      Type = type;
   }
   public string Type { get; set; }
}

public class AppsEditRequest
{
   public AppsEditRequest(BaseAppForm form)
   {
      Form = form;
   }
   public List<BaseOption<int>> ApiOptions { get; set; } = new List<BaseOption<int>>();
   public BaseAppForm Form { get; set; }
}

public abstract class BaseAppForm
{
   public string ClientId { get; set; } = String.Empty;
   public string Type { get; set; } = String.Empty;
   public string Url { get; set; } = String.Empty;
   public string Icon { get; set; } = String.Empty;
   public string Name { get; set; } = String.Empty;

   public List<int> Apis { get; set; } = new List<int>();
   public List<string> Roles { get; set; } = new List<string>();


}
public class AppAddForm : BaseAppForm
{

}
public class AppEditForm : BaseAppForm
{
   
}