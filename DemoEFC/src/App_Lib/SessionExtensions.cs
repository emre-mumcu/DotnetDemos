using Newtonsoft.Json;

namespace src.App_Lib;

public static class SessionExtensions
{
    private static JsonSerializerSettings jsonSerializerSettings
    {
        get
        {
            return new JsonSerializerSettings()
            {
                Formatting = Formatting.None,
                PreserveReferencesHandling = PreserveReferencesHandling.None,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Include,
                Culture = new System.Globalization.CultureInfo(AppConstants.App_Culture)
            };
        }
    }

    public static void SetKey<T>(this ISession session, string key, T value)
    {
        session.SetString(key, JsonConvert.SerializeObject(value, Formatting.None, jsonSerializerSettings));
    }

    public static T? GetKey<T>(this ISession session, string key)
    {
        var value = session.GetString(key);
        return value == null ? default : JsonConvert.DeserializeObject<T>(value);
    }

    public static void RemoveKey(this ISession session, string key)
    {
        session.Remove(key);
    }
}