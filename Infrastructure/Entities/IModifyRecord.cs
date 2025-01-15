namespace Infrastructure.Entities;

public interface IModifyRecord
{
   string EntityType { get; set; }
   string EntityId { get; set; }
   string Action { get; set; }
   DateTime DateTime { get; set; }
   string UserId { get; set; }

   string DataJson { get; set; }
}