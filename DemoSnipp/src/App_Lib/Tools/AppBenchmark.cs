using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;

namespace src.App_Lib.Tools
{
	// var summary = BenchmarkRunner.Run<AppBenchmark>();
	// dotnet run -c Release

	[MemoryDiagnoser]
	[Orderer(BenchmarkDotNet.Order.SummaryOrderPolicy.FastestToSlowest)]
	[RankColumn]
	public class AppBenchmark
    {
		[GlobalSetup]
		public void GlobalSetup()
		{
			
		}

		[GlobalCleanup]
		public void GlobalCleanup()
		{
			
		}

		[Benchmark(Baseline = true)]
		public void Bench1()
		{
			var test = new  { prop1 = "1", prop2 = "lorem ipsum dolor sitamet", prop3 = "YYYY.MM.DD hh:mm:ss fff" };

			for(int i = 0; i< 50; i++)
			{
				var result = Serialization.Serialize(test);
			}
		}

		[Benchmark]
		public async Task Bench2()
		{
			var test = new { prop1 = "1", prop2 = "lorem ipsum dolor sitamet", prop3 = "YYYY.MM.DD hh:mm:ss fff" };

			for (int i = 0; i < 50; i++)
			{
				var result = await Serialization.SerializeAsync(test);
			}
		}		
	}
}