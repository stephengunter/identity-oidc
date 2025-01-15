namespace Infrastructure.Entities;

public interface IReviewable
{
	bool Reviewed { get; set; }
   DateTime? ReviewedAt { get; set; }
   string? ReviewedBy { get; set; }
}
