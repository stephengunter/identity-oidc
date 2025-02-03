namespace Infrastructure.Helpers;
public static class ValidationMessages
{
   public static string Required(string title) =>
       $"必須填寫{title}";
   public static string IsEmpty(string title) =>
       $"{title}是空白的";
   public static string Duplicate(string title) =>
       $"{title}重複了";
   public static string NotExist(string title) =>
       $"{title}不存在";
   public static string MinLength(int len, string title = "") =>
       $"{title}長度至少需要{len}個字元";

   public static string IsNumeric(string title) =>
       $"{title}必須是數字";

   public static string AlphaNum(string title) =>
       $"{title}只能是英文或數字";

   public static string SameAs(string source, string target) =>
       $"{source}與{target}必須相同";

   public static string NotSameAs(string source, string target) =>
       $"{source}與{target}必須不同";

   public static string WrongFormatOf(string title) =>
       $"不正確的{title}格式";

   public static string MustSelect(string title) =>
       $"必須選擇{title}";

   // Static strings that don't need parameters
   public const string Sub = "sub";
   public const string Roles = "roles";
   public const string Name = "name";
}

