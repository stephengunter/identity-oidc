namespace Infrastructure.Views;

public interface IModifyRecordView
{
   string EntityType { get; set; }
   string EntityId { get; set; }
   string Action { get; set; }
   DateTime DateTime { get; set; }
   string UserId { get; set; }
}