using System;
using System.Reflection;

namespace Infrastructure.Helpers;
public static class DictionaryHelpers
{
   public static Dictionary<string, TValue> GetDictionaries<TValue>(this Type type)
   {
      Dictionary<string, TValue> result = new Dictionary<string, TValue>();

      FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Static);

      foreach (FieldInfo field in fields)
      {
         string fieldName = field.Name;
         TValue fieldValue = (TValue)field.GetValue(null)!;
         result[fieldName] = fieldValue;
      }
      return result;
   }
   public static Dictionary<string, TValue> CombineDictionaries<TValue>(this Dictionary<string, TValue> dict1, Dictionary<string, TValue> dict2)
   {
      var combinedDict = new Dictionary<string, TValue>(dict1);

      foreach (var kvp in dict2)
      {
         if (!combinedDict.ContainsKey(kvp.Key))
         {
            combinedDict.Add(kvp.Key, kvp.Value);
         }
         // Optionally, you can handle existing keys differently, such as updating the value
         // else
         // {
         //     combinedDict[kvp.Key] = kvp.Value;
         // }
      }

      return combinedDict;
   }

}
