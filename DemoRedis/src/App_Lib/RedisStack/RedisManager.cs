using NRedisStack;
using NRedisStack.RedisStackCommands;
using StackExchange.Redis;

namespace src.App_Lib.RedisStack
{
    public class RedisManager
    {
        private readonly IDatabase redisDB;

        public RedisManager()
        {
            redisDB = RedisConnector.Connection.GetDatabase();
        }

        public bool SetString(string key, string value)
        {
            return redisDB.StringSet(key, value);
        }

        public void SetHash(string key, HashEntry[] value)
        {
            redisDB.HashSet(key, value);
        }

        public void CommandRef()
        {
            IBloomCommands bf = redisDB.BF();
            ICuckooCommands cf = redisDB.CF();
            ICmsCommands cms = redisDB.CMS();
            // IGraphCommands graph = redisDB.GRAPH();
            ITopKCommands topk = redisDB.TOPK();
            ITdigestCommands tdigest = redisDB.TDIGEST();
            ISearchCommands ft = redisDB.FT();
            IJsonCommands json = redisDB.JSON();
            ITimeSeriesCommands ts = redisDB.TS();
        }
    }
}
/*

    public IActionResult Index(
        [FromServices] IConfiguration configuration,
        [FromServices] IOptions<RedisOptions> staticRedis,
        [FromServices] IOptionsMonitor<RedisOptions> dynamicRedis)
    {
        var objRedis1 = configuration.GetSection("Redis").Get<RedisOptions>();
        var objRedis2 = staticRedis.Value;
        var objRedis3 = dynamicRedis.CurrentValue;
        var redis_configuration = configuration["Redis:Configuration"];
        

        var db = RedisConnector.Connection.GetDatabase();

        var ft = db.FT();
        var json = db.JSON();

        var user1 = new
        {
            name = "Paul John",
            email = "paul.john@example.com",
            age = 42,
            city = "London"
        };

        var user2 = new
        {
            name = "Eden Zamir",
            email = "eden.zamir@example.com",
            age = 29,
            city = "Tel Aviv"
        };

        var user3 = new
        {
            name = "Paul Zamir",
            email = "paul.zamir@example.com",
            age = 35,
            city = "Tel Aviv"
        };

        var schema = new Schema()
    .AddTextField(new FieldName("$.name", "name"))
    .AddTagField(new FieldName("$.city", "city"))
    .AddNumericField(new FieldName("$.age", "age"));

        ft.Create(
            "idx:users",
            new FTCreateParams().On(IndexDataType.JSON).Prefix("user:"),
            schema);



        json.Set("user:1", "$", user1);
        json.Set("user:2", "$", user2);
        json.Set("user:3", "$", user3);

        // Let's find user Paul and filter the results by age.
        var res = ft.Search("idx:users", new Query("Paul @age:[30 40]")).Documents.Select(x => x["json"]);

        // Return only the city field.
        var res_cities = ft.Search("idx:users", new Query("Paul").ReturnFields(new FieldName("$.city", "city"))).Documents.Select(x => x["city"]);

        // Count all users in the same city.
        var request = new AggregationRequest("*").GroupBy("@city", Reducers.Count().As("count"));
        var result = ft.Aggregate("idx:users", request);

        return View();
    }
*/