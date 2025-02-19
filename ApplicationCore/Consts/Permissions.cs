namespace ApplicationCore.Consts;
public enum Permissions
{
	Admin,
   JudgebookFiles

}
public enum AppRoles
{
   UnKnown = -1,
	Boss,
	Dev,
   IT,
   Clerk,//書記官
   Recorder,
   Files, //檔案管理
   Driver,
   CarManager, //車輛管理
}
public class AdminRoles
{
   public static string Dev = "Dev";
   public static string Boss = "Boss";
}