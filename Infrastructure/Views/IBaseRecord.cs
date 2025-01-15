using Infrastructure.Entities;

namespace Infrastructure.Views;

public interface IBaseRecordView
{
   DateTime CreatedAt { get; set; }
   string CreatedBy { get; set; }
   DateTime? LastUpdated { get; set; }
   string? UpdatedBy { get; set; }

	string CreatedAtText { get;}
   string LastUpdatedText { get;}

}

