using System;
using System.Security.Cryptography;
using System.Text;

public interface ICryptoService
{
   string Encrypt(string plainText);
   string Decrypt(string cipherText);
}

public class AesGcmCryptoService : ICryptoService
{
   private readonly byte[] _key;
   private const int NonceSize = 12; // 96 bits for the nonce (recommended for AES-GCM)
   private const int TagSize = 16;   // 128 bits for the authentication tag

   public AesGcmCryptoService(byte[] key)
   {
      if (key == null || key.Length != 32) // 256-bit key
         throw new ArgumentException("Key must be 256 bits (32 bytes).", nameof(key));

      _key = key;
   }

   public string Encrypt(string plainText)
   {
      if (string.IsNullOrEmpty(plainText))
         throw new ArgumentNullException(nameof(plainText));

      byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
      byte[] nonce = new byte[NonceSize];
      byte[] cipherText = new byte[plainBytes.Length];
      byte[] tag = new byte[TagSize];

      using (var aesGcm = new AesGcm(_key))
      {
         // Generate a unique nonce for each encryption operation
         RandomNumberGenerator.Fill(nonce);

         // Encrypt the plaintext
         aesGcm.Encrypt(nonce, plainBytes, cipherText, tag);
      }

      // Combine nonce, cipherText, and tag into a single byte array
      byte[] encryptedBytes = new byte[nonce.Length + cipherText.Length + tag.Length];
      Buffer.BlockCopy(nonce, 0, encryptedBytes, 0, nonce.Length);
      Buffer.BlockCopy(cipherText, 0, encryptedBytes, nonce.Length, cipherText.Length);
      Buffer.BlockCopy(tag, 0, encryptedBytes, nonce.Length + cipherText.Length, tag.Length);

      // Convert to Base64 for easy storage/transmission
      return Convert.ToBase64String(encryptedBytes);
   }

   public string Decrypt(string cipherText)
   {
      if (string.IsNullOrEmpty(cipherText))
         throw new ArgumentNullException(nameof(cipherText));

      byte[] encryptedBytes = Convert.FromBase64String(cipherText);

      // Extract nonce, cipherText, and tag from the combined byte array
      byte[] nonce = new byte[NonceSize];
      byte[] cipherBytes = new byte[encryptedBytes.Length - NonceSize - TagSize];
      byte[] tag = new byte[TagSize];

      Buffer.BlockCopy(encryptedBytes, 0, nonce, 0, nonce.Length);
      Buffer.BlockCopy(encryptedBytes, nonce.Length, cipherBytes, 0, cipherBytes.Length);
      Buffer.BlockCopy(encryptedBytes, nonce.Length + cipherBytes.Length, tag, 0, tag.Length);

      byte[] plainBytes = new byte[cipherBytes.Length];

      using (var aesGcm = new AesGcm(_key))
      {
         // Decrypt the ciphertext
         aesGcm.Decrypt(nonce, cipherBytes, tag, plainBytes);
      }

      // Convert the decrypted bytes back to a string
      return Encoding.UTF8.GetString(plainBytes);
   }
}