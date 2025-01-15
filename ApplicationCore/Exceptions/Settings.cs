using Infrastructure.Entities;

namespace ApplicationCore.Exceptions;
public class SettingsException : Exception
{
   public SettingsException(string key) : base($"SettingsError. Key: {key}")
   {

   }
}
