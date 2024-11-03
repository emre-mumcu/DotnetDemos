using System;

namespace DemoAuthenticate.AppLib;

public static partial class UserExtensions
{
	public static (DateTime? Created, DateTime? LastRequest, Boolean? Login, AppUser? User) GetUserSession(this ISession session, AppUser user)
	{
		return (
			session.Get<DateTime>(Literals.SessionKey_Created),			
			session.Get<DateTime>(Literals.SessionKey_LastRequest),
			session.Get<Boolean>(Literals.SessionKey_Login),
			session.Get<AppUser>(Literals.SessionKey_User)
		);		
	}

	public static void SetUserSession(this ISession session, AppUser user)
	{
		session.Set<DateTime>(Literals.SessionKey_Created, DateTime.UtcNow);
		session.Set<DateTime>(Literals.SessionKey_LastRequest, DateTime.UtcNow);
		session.Set<Boolean>(Literals.SessionKey_Login, true);
		session.Set<AppUser>(Literals.SessionKey_User, user);
	}

	public static void ResetUserSession(this ISession session)
	{
		session.Set<DateTime>(Literals.SessionKey_Created, DateTime.MinValue);
		session.Set<DateTime>(Literals.SessionKey_LastRequest, DateTime.MinValue);
		session.Set<Boolean>(Literals.SessionKey_Login, false);
		session.Set<AppUser>(Literals.SessionKey_User, new AppUser());
	}

	public static void SetLastRequestTimeStamp(this ISession session)
	{
		session.Set<DateTime>(Literals.SessionKey_LastRequest, DateTime.UtcNow);
	}
}

public class AppUser
{

}