--------------------------------------------------------------------------------------------------


--------------------------------------------------------------------------------------------------


--------------------------------------------------------------------------------------------------


--------------------------------------------------------------------------------------------------


--------------------------------------------------------------------------------------------------

<!-- Jumbotron -->
<div class="p-4 shadow-4 rounded-3" style="background-color: hsl(0, 0%, 94%);">
    <h2>Hello world!</h2>
    <p>
        This is a simple hero unit, a simple jumbotron-style component for calling extra
        attention to featured content or information.
    </p>

    <hr class="my-4" />

    <p>
        It uses utility classes for typography and spacing to space content out within the
        larger container.
    </p>
    <button type="button" class="btn btn-primary">
        Learn more
    </button>
</div>
<!-- Jumbotron -->

--------------------------------------------------------------------------------------------------


    string Title(string str)
    {
        TextInfo ti = new CultureInfo("tr-TR", false).TextInfo;
        return ti.ToTitleCase(str);
    }

--------------------------------------------------------------------------------------------------

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            if (!optionsBuilder.IsConfigured)
            {

                // DI ile alınmayan contextlerde (new ile elle oluşturulursa) connection string buradan alınacak!!!
                // JsonSerializer serializer = new JsonSerializer();
                // MyObject obj = serializer.Deserialize<DataOptions>(File.ReadAllText(@".\path\to\json\config\file.json");

                //if (_Configuration == null)
                //{
                //    _Configuration = new ConfigurationBuilder()
                //        .SetBasePath(Directory.GetCurrentDirectory())
                //        .AddJsonFile("data.json", true)
                //        .Build();
                //}

                //var connectionString = DPAPI.Unportect(_Configuration.GetSection("Application").GetSection("ConnectionString").Value, "ConnectionString");

                //optionsBuilder.UseSqlServer(connectionString);
            }
        }
--------------------------------------------------------------------------------------------------

public static class EnumerableExtensions {
    public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> source) =>
        source != null ? source : Enumerable.Empty<T>();
}
--------------------------------------------------------------------------------------------------

    // CREATE DATABASE SampleDB ENCODING = 'UTF8' 
    // dotnet ef migrations add Init -o AppLib/Data/Migrations
    // dotnet ef database update


    ```sql
-- Export to Json
SELECT row_to_json(il_data) FROM (SELECT id "IlId", il "IlAdi" FROM public."iller") il_data
SELECT row_to_json(ilce_data) FROM (SELECT id "IlceId", ilce "IlceAdi", ilid "IlId" FROM public."ilceler") ilce_data
SELECT row_to_json(sbb_data) FROM (SELECT id "SemtBucakBeldeId", semt_bucak_belde "SemtBucakBeldeAdi", ilceid "IlceId" FROM public."semtbucakbeldeler") sbb_data
SELECT row_to_json(mahalle_data) FROM (SELECT id "MahalleId", mahalle "MahalleAdi", pk "PK", sbbid "SemtBucakBeldeId" FROM public."mahalleler") mahalle_data
SELECT row_to_json(mahalle_data) FROM (SELECT * FROM public."mahalleler") mahalle_data
```