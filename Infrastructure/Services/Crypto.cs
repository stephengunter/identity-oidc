using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Services;

public interface ICryptoService
{
   string Encrypt(string plainText);

   string Decrypt(string cipherText);
}

public class AesCryptoService : ICryptoService
{
   private readonly byte[] _key;
   public AesCryptoService(string key)
   {
      _key = CreateKey(key, 32);
   }
   public string Encrypt(string plainText)
   {
      byte[] iv = new byte[16];
      byte[] array;

      using (Aes aes = Aes.Create())
      {
         aes.Key = _key;
         aes.IV = iv;

         ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

         using (MemoryStream memoryStream = new MemoryStream())
         {
            using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
            {
               using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
               {
                  streamWriter.Write(plainText);
               }

               array = memoryStream.ToArray();
            }
         }
      }

      return Convert.ToBase64String(array);
   }

   public string Decrypt(string cipherText)
   {
      byte[] iv = new byte[16];
      byte[] buffer = Convert.FromBase64String(cipherText);

      using (Aes aes = Aes.Create())
      {
         aes.Key = _key;
         aes.IV = iv;
         ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

         using (MemoryStream memoryStream = new MemoryStream(buffer))
         {
            using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
            {
               using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
               {
                  return streamReader.ReadToEnd();
               }
            }
         }
      }
   }

   private byte[] CreateKey(string password, int keyBytes)
   {
      // Use PBKDF2 to derive a secure key from the password
      using (var keyDerivationFunction = new Rfc2898DeriveBytes(password, new byte[16], 10000))
      {
         return keyDerivationFunction.GetBytes(keyBytes);
      }
   }
}
