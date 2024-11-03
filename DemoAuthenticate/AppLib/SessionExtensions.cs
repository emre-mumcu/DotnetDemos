using System;

namespace DemoAuthenticate.AppLib;

public static partial class SessionExtensions
{
	public static void Set<T>(this ISession session, string key, T value)
	{
		session.Set(key, System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(value));
	}

	public static T? Get<T>(this ISession session, string key)
	{
		session.TryGetValue(key, out byte[]? dataByte);

		string? data = dataByte != null ? System.Text.Encoding.UTF8.GetString(dataByte) : null;

		return data == null ? default(T) : System.Text.Json.JsonSerializer.Deserialize<T>(data);
	}
}


