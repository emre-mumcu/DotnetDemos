using System.Net;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using src.App_Lib.Tools;

namespace src.Controllers
{
	public class HttpRequestsController : Controller
	{
		[HttpGet]
		public async Task<ActionResult> Login()
		{
			var requestData = new { username = "emilys", password = "emilyspass", expiresInMins = 30 };

			using (var client = new HttpClient(new HttpClientHandler { UseProxy = true, Proxy = WebRequest.DefaultWebProxy, UseDefaultCredentials = true }))
			{
				try
				{
					HttpRequestMessage request = new HttpRequestMessage()
					{
						Method = HttpMethod.Post,
						RequestUri = new Uri("https://dummyjson.com/auth/login"),
						Content = new StringContent(requestData.Serialize(), Encoding.UTF8, MediaTypeNames.Application.Json)
					};

					HttpResponseMessage response = await client.SendAsync(request);

					response.EnsureSuccessStatusCode();

					if (response.StatusCode == HttpStatusCode.OK)
					{
						var responseStr = await response.Content.ReadAsStringAsync();

						var model = Serialization.Deserialize<DummyJson.Login>(responseStr);

						return Content(Serialization.Serialize(model));
					}
					else
					{
						return Content(response.StatusCode.ToString());
					}
				}
				catch (Exception ex)
				{
					return Content(ex.Message);
				}
			}
		}

		[HttpPost]
		public async Task<ActionResult> UserDetail(string token)
		{			
			using (var client = new HttpClient(new HttpClientHandler { UseProxy = true, Proxy = WebRequest.DefaultWebProxy, UseDefaultCredentials = true }))
			{
				try
				{
					HttpRequestMessage request = new HttpRequestMessage()
					{
						Method = HttpMethod.Get,
						RequestUri = new Uri("https://dummyjson.com/auth/me")
					};

					request.Headers.Add("Authorization", $"Bearer {token}");

					HttpResponseMessage response = await client.SendAsync(request);

					response.EnsureSuccessStatusCode();

					if (response.StatusCode == HttpStatusCode.OK)
					{
						var responseStr = await response.Content.ReadAsStringAsync();						

						var model = Serialization.DeserializeNS<DummyJson.UserDetail>(responseStr);

						return Content(Serialization.Serialize(model));
					}
					else
					{
						return Content(response.StatusCode.ToString());
					}
				}
				catch (Exception ex)
				{
					return Content(ex.Message);
				}
			}
		}
	}
}

namespace DummyJson
{
	public class Login
	{
		public int id { get; set; }
		public string? username { get; set; }
		public string? email { get; set; }
		public string? firstName { get; set; }
		public string? lastName { get; set; }
		public string? gender { get; set; }
		public string? image { get; set; }
		public string? accessToken { get; set; }
		public string? refreshToken { get; set; }
	}

	public partial class UserDetail
	{
		public long Id { get; set; }
		public string? FirstName { get; set; }
		public string? LastName { get; set; }
		public string? MaidenName { get; set; }
		public long Age { get; set; }
		public string? Gender { get; set; }
		public string? Email { get; set; }
		public string? Phone { get; set; }
		public string? Username { get; set; }
		public string? Password { get; set; }
		public string? BirthDate { get; set; }
		public Uri? Image { get; set; }
		public string? BloodGroup { get; set; }
		public double Height { get; set; }
		public double Weight { get; set; }
		public string? EyeColor { get; set; }
		public Hair? Hair { get; set; }
		public string? Ip { get; set; }
		public Address? Address { get; set; }
		public string? MacAddress { get; set; }
		public string? University { get; set; }
		public Bank? Bank { get; set; }
		public Company? Company { get; set; }
		public string? Ein { get; set; }
		public string? Ssn { get; set; }
		public string? UserAgent { get; set; }
		public Crypto? Crypto { get; set; }
		public string? Role { get; set; }
	}

	public partial class Address
	{
		[Newtonsoft.Json.JsonProperty("Address")]
		public string? Street { get; set; }
		public string? City { get; set; }
		public string? State { get; set; }
		public string? StateCode { get; set; }
		public long PostalCode { get; set; }
		public Coordinates? Coordinates { get; set; }
		public string? Country { get; set; }
	}

	public partial class Coordinates
	{
		public double Lat { get; set; }
		public double Lng { get; set; }
	}

	public partial class Bank
	{
		public string? CardExpire { get; set; }
		public string? CardNumber { get; set; }
		public string? CardType { get; set; }
		public string? Currency { get; set; }
		public string? Iban { get; set; }
	}

	public partial class Company
	{
		public string? Department { get; set; }
		public string? Name { get; set; }
		public string? Title { get; set; }
		public Address? Address { get; set; }
	}

	public partial class Crypto
	{
		public string? Coin { get; set; }
		public string? Wallet { get; set; }
		public string? Network { get; set; }
	}

	public partial class Hair
	{
		public string? Color { get; set; }
		public string? Type { get; set; }
	}
}

