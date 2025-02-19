namespace Infrastructure.Views;
public class BaseOption<Tkey>
{
   public BaseOption(Tkey value, string text)
   {
      this.Value = value;
      this.Title = text;
   }
   public Tkey Value { get; set; }
   public string Title { get; set; }
}

