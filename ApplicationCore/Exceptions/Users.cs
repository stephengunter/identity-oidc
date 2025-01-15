using ApplicationCore.Models;

namespace ApplicationCore.Exceptions;

public class CreateUserException : Exception
{
	public CreateUserException(User user, string msg = "") : base($"Create User Failed. UserName: {user.UserName} , Msg:{msg}")
	{

	}
}

public class UpdateUserException : Exception
{
	public UpdateUserException(User user, string msg = "") : base($"Update User Failed. UserName: {user.UserName} , Msg:{msg}")
	{

	}
}

public class UserNotFoundException : Exception
{
	public UserNotFoundException(string val, string key) : base($"UserNotFound. {key}: {val}")
	{

	}
}
public class UpdateUserRoleException : Exception
{
   public UpdateUserRoleException(User user, string role, string msg = "") : base($"UpdateUserRoles Failed. UserName: {user.UserName} ,  role: {role}, Msg: {msg}")
   {

   }
}