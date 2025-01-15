namespace Infrastructure.Entities;

public interface IBaseRecord
{
   DateTime CreatedAt { get; set; }
   string CreatedBy { get; set; }
   DateTime? LastUpdated { get; set; }   
   string? UpdatedBy { get; set; }
}