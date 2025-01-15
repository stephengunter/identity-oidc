namespace ApplicationCore.Exceptions;

public class CurrentUserIdNotEqualToRequestUserIdException : Exception
{
	public CurrentUserIdNotEqualToRequestUserIdException(string msg = "") : base(msg)
	{

	}
}
public class CurrentUserIdNotFoundException : Exception
{
   //can not get user id from claims
   public CurrentUserIdNotFoundException(string msg = "") : base(msg)
   {

   }
}
