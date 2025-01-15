namespace Infrastructure.Entities;

public interface IBaseContract
{
   DateTime? StartDate { get; set; }
   DateTime? EndDate { get; set; }

   public ContractStatus Status { get; }
}

public enum ContractStatus
{
   NA = -1,
   Before = 0,
	Active = 1,
   Ended = 2
}

