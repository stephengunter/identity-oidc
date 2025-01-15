namespace ApplicationCore.Exceptions;

public class TokenResolveFailedException : Exception
{
	public TokenResolveFailedException(string msg = "") : base(msg)
	{

	}
}
public class RefreshTokenFailedException : Exception
{
	public RefreshTokenFailedException(string msg) : base(msg)
	{

	}
}

public class AuthTokenCreateFailedException : Exception
{
   public AuthTokenCreateFailedException(string msg) : base(msg)
   {

   }
}

public class AuthTokenLoginFailedException : Exception
{
   public AuthTokenLoginFailedException(string msg) : base(msg)
   {

   }
}
