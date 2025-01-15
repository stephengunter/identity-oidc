using ApplicationCore.Consts;
using Infrastructure.Helpers;
using Infrastructure.Views;

namespace ApplicationCore.Views;
public class UserViewModel : IBaseRecordView
{
	public string Id { get; set; } = String.Empty;

	public string Name { get; set; } = String.Empty;

   public string UserName { get; set; } = String.Empty;

	public string Email { get; set; } = String.Empty;

	public string? PhoneNumber { get; set; }

   public bool Active { get; set; }

   public DateTime CreatedAt { get; set; }
   public string CreatedBy { get; set; } = String.Empty;
   public DateTime? LastUpdated { get; set; }
   public string? UpdatedBy { get; set; }

   public string CreatedAtText => CreatedAt.ToDateString();
   public string LastUpdatedText => LastUpdated.ToDateString();


   public ICollection<RoleViewModel> Roles { get; set; } = new List<RoleViewModel>();

	public bool HasPassword { get; set; }

	public ProfilesViewModel? Profiles { get; set; }
}

public class RoleViewModel
{ 
	public string Id { get; set; } = String.Empty;
   public string Name { get; set; } = String.Empty;
   public string Title { get; set; } = String.Empty;
}