# Create Project

```shell
DemoSnipp> dotnet new sln
DemoSnipp> dotnet new web -o src
DemoSnipp> dotnet sln add src/.
```

# Fake Api References

* https://dummyjson.com/docs
* https://fakeapi.platzi.com/en/about/introduction/
* https://fakestoreapi.com/docs
* https://jsonplaceholder.typicode.com/guide/

# Nuget Packages

dotnet add package Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation
dotnet add package BenchmarkDotNet
dotnet add package Newtonsoft.Json

# Tools

https://quicktype.io/csharp
https://app.quicktype.io/


```cs

// Content = new FormUrlEncodedContent(new Dictionary<string, string> { {"username", "emilys" }, {"password", "emilyspass" }}),

					// Request Headers
					//req.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


					// req.Headers.Add("", "");

					/* 
										var cts = new CancellationTokenSource();					
										cts.CancelAfter(TimeSpan.FromSeconds(10));
										HttpResponseMessage res = await client.SendAsync(req, cts.Token); */

										                        httpRequestMessageForUserInfo.Headers.Add("authorization", $"Bearer {tokenModel.access_token}");
                        httpRequestMessageForUserInfo.Headers.Add("client_id", client_id);
                        httpRequestMessageForUserInfo.Headers.Add("client_secret", client_secret);
```

