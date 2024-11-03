using System;
using Microsoft.CodeAnalysis;

namespace DemoAuthenticate.AppLib;

public static partial class SessionExtensions
{
	public static void Set<T>(this ISession session, string key, T value)
	{
		session.Set(key, System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(value));
	}

	public static T? Get<T>(this ISession session, string key)
	{
		return System.Text.Json.JsonSerializer.Deserialize<T>(session.Get(key));
	}

	public static T? TryGet<T>(this ISession session, string key)
	{
		if (session.TryGetValue(key, out byte[]? value))
		{
			return System.Text.Json.JsonSerializer.Deserialize<T>(value);
		}
		else
		{
			return default;
		}
	}
}