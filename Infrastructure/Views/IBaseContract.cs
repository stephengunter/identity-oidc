namespace Infrastructure.Views;

public interface IBaseContractView
{
   DateTime? StartDate { get; set; }
   DateTime? EndDate { get; set; }

   public int Status { get; set; }
   public string StatusText { get; set; }
}

