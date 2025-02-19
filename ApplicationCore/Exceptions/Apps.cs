using ApplicationCore.Models;

namespace ApplicationCore.Exceptions;

public class ApplicationNotExistException : Exception
{
	public ApplicationNotExistException(App app) : base($"Application Not Exist. AppId: {app.Id}  ClientId: {app.ClientId}")
	{

	}
}

public class ClientSecretInvalidException : Exception
{
   public ClientSecretInvalidException(App app) : base($"ClientSecret Invalid. AppId: {app.Id}  ClientId: {app.ClientId}")
   {

   }
}