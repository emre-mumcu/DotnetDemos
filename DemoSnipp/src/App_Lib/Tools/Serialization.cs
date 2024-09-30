using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace src.App_Lib.Tools;

public static class Serialization
{
	public static JsonSerializerOptions options = GetOptions();

	private static JsonSerializerOptions GetOptions()
	{
		if (options == null)

			return new JsonSerializerOptions()
			{
				WriteIndented = true,
				ReferenceHandler = ReferenceHandler.IgnoreCycles,
				PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
				PropertyNameCaseInsensitive = false
			};

		else return options;
	}


	public static JsonSerializerSettings settings = GetSettings();

	private static JsonSerializerSettings GetSettings()
	{
		if (settings == null)

			return new JsonSerializerSettings
			{
				NullValueHandling = NullValueHandling.Ignore, 
				Formatting = Formatting.Indented,             
				MissingMemberHandling = MissingMemberHandling.Ignore,
				ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver
				{
					IgnoreSerializableAttribute = true,
					NamingStrategy = new Newtonsoft.Json.Serialization.DefaultNamingStrategy()
				}
			};

		else return settings;
	}

	public static string Serialize<T>(this T obj)
	{
		return System.Text.Json.JsonSerializer.Serialize(obj, options);
	}

	public static async Task<string> SerializeAsync<T>(this T obj)
	{
		using (var stream = new MemoryStream())
		{
			// await JsonSerializer.SerializeAsync(stream, obj, obj.GetType());

			await System.Text.Json.JsonSerializer.SerializeAsync(stream, obj, options);
			stream.Position = 0;
			using var reader = new StreamReader(stream);
			return await reader.ReadToEndAsync();
		}
	}

	public static T? Deserialize<T>(this string str)
	{
		T? obj = System.Text.Json.JsonSerializer.Deserialize<T>(str);
		return obj;
	}

	public static async Task<T?> DeserializeAsync<T>(this string str)
	{
		using var stream = new MemoryStream(Encoding.ASCII.GetBytes(str));
		T? obj = await System.Text.Json.JsonSerializer.DeserializeAsync<T>(stream);
		return obj;
	}

	public static T? DeserializeNS<T>(this string str)
	{
		T? obj = JsonConvert.DeserializeObject<T>(str, settings);
		return obj;
	}
}
