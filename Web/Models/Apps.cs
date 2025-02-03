using ApplicationCore.Consts;
using Infrastructure.Helpers;

namespace Web.Models;

public class AppLabels
{
   public string Type => "類型";
   public string ClientId => "ClientId";
   public string DisplayName => "名稱";
   public string RedirectUris => "RedirectUris";
   public string PostLogoutRedirectUris => "PostLogoutRedirectUris";
   public string Ps => "備註";
}
public class AppsIndexModel
{
   public AppsIndexModel(AppsFetchRequest request)
   {
      Request = request;
   }
   public AppLabels Labels => new AppLabels();
   public List<string> Types => ObjectsHelpers.GetStaticKeys<AppTypes>();
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


public abstract class BaseAppForm
{
   public string ClientId { get; set; } = String.Empty;
   public string Type { get; set; } = String.Empty;
   public string? Url { get; set; } = String.Empty;
   public string? Title { get; set; } = String.Empty;

}
public class AppAddForm : BaseAppForm
{

}
public class AppEditForm : BaseAppForm
{
   
}