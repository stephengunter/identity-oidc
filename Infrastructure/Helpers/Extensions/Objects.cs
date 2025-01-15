using Newtonsoft.Json;
using System.Reflection;

namespace Infrastructure.Helpers;

public static class ObjectsHelpers
{
   public static List<string> GetStaticKeys<T>()
   {
      List<string> keys = new List<string>();
      Type type = typeof(T);

      // Get all public static fields of type T
      foreach (FieldInfo field in type.GetFields(BindingFlags.Public | BindingFlags.Static))
      {
         keys.Add(field.Name);  // Add the field name to the list
      }

      return keys;
   }
	public static void SetValuesTo(this object source, object dest, ICollection<string> exceptsNames)
	{
      var sourceProperties = source.GetType().GetProperties();
      var destProperties = dest.GetType().GetProperties();

      foreach (var sourceProperty in sourceProperties)
      {
         if (exceptsNames.HasItems())
         {
            var isExcept = (exceptsNames.FirstOrDefault(x => sourceProperty.Name.EqualTo(x)) != null);
            if (isExcept) continue;
         }
         var destProperty = destProperties.FirstOrDefault(p => p.Name == sourceProperty.Name);
         if (destProperty != null && destProperty.CanWrite)
         {
            var value = sourceProperty.GetValue(source);
            destProperty.SetValue(dest, value);
         }
      }
   }
   public static void SetValuesTo(this object source, object dest, string excepts = "")
   {
      if (string.IsNullOrEmpty(excepts)) SetValuesTo(source, dest, new List<string>());
      else
      {
         var exceptsNames = excepts.SplitToList();
         SetValuesTo(source, dest, exceptsNames);
      }
   }

   public static T CloneEntity<T>(this T entity)
   {
      string json = JsonConvert.SerializeObject(entity, new JsonSerializerSettings
      {
         ReferenceLoopHandling = ReferenceLoopHandling.Ignore
      });

      T clonedEntity = JsonConvert.DeserializeObject<T>(json); 
		return clonedEntity;
   }

   public static void Dump(this object obj, TextWriter writer)
	{
		if (obj == null)
		{
			writer.WriteLine("Object is null");
			return;
		}

		writer.Write("Hash: ");
		writer.WriteLine(obj.GetHashCode());
		writer.Write("Type: ");
		writer.WriteLine(obj.GetType());

		var props = GetProperties(obj);

		if (props.Count > 0)
		{
			writer.WriteLine("-------------------------");
		}

		foreach (var prop in props)
		{
			writer.Write(prop.Key);
			writer.Write(": ");
			writer.WriteLine(prop.Value);
		}
	}

	private static Dictionary<string, string> GetProperties(object obj)
	{
		var props = new Dictionary<string, string>();
		if (obj == null)
			return props;

		var type = obj.GetType();
		foreach (var prop in type.GetProperties())
		{
			var val = prop.GetValue(obj, new object[] { });
			var valStr = val == null ? "" : val.ToString();
			props.Add(prop.Name, valStr!);
		}

		return props;
	}
}
