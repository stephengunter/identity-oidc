using System.Security.Cryptography;
using System.Text;
namespace ApplicationCore.Helpers;

public static class CryptographyHelper
{
   public static byte[] DeriveKeyFromString(this string keyString)
   {
      using (var sha256 = SHA256.Create())
      {
         return sha256.ComputeHash(Encoding.UTF8.GetBytes(keyString));
      }
   }
}