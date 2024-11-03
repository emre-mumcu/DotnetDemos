using System;

namespace DemoAuthenticate.AppLib;

public static partial class UserExtensions
{
	public static (DateTime? Created, DateTime? LastRequest, Boolean Login, AppUser? User) GetUserSession(this ISession session)
	{
		return (
			session.TryGet<DateTime>(Literals.SessionKey_Created),			
			session.TryGet<DateTime>(Literals.SessionKey_LastRequest),
			session.TryGet<Boolean>(Literals.SessionKey_Login),
			session.TryGet<AppUser>(Literals.SessionKey_User)
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

	public static DateTime? GetLastRequestTimeStamp(this ISession session)
	{
		var v = session.TryGet<DateTime>(Literals.SessionKey_LastRequest);
		return v == default(DateTime) ? null : v;
	}
}

public class AppUser
{

}