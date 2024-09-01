using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using src.App_Data;
using src.App_Data.Context;
using src.App_Data.Entities;

namespace src.App_Data
{
    public static class DataSeeder
    {
        /// <summary>
        /// This SeedData method will run along with migrations (add migration and update database).
        /// Primary key field (ID) data can be SET in this type of seeding.
        /// But this will cause the migration files and times bigger and longer if the seed data size is  large.
        /// </summary>                
        public static void SeedData(this ModelBuilder modelBuilder)
        {
            //
            // if (System.Diagnostics.Debugger.IsAttached == false) System.Diagnostics.Debugger.Launch();
            //             

            // States
            modelBuilder.Entity<State>().HasData(
                new State { Id = 1, StateName = "Baden-Wurttemberg" },
                new State { Id = 2, StateName = "Bavaria" },
                new State { Id = 3, StateName = "Berlin" },
                new State { Id = 4, StateName = "Brandenburg" },
                new State { Id = 5, StateName = "Bremen" },
                new State { Id = 6, StateName = "Hamburg" },
                new State { Id = 7, StateName = "Hesse" },
                new State { Id = 8, StateName = "Lower Saxony" },
                new State { Id = 9, StateName = "Mecklenburg-Vorpommern" },
                new State { Id = 10, StateName = "North Rhine-Westphalia" },
                new State { Id = 11, StateName = "Rhineland-Palatinate" },
                new State { Id = 12, StateName = "Saarland" },
                new State { Id = 13, StateName = "Saxony" },
                new State { Id = 14, StateName = "Saxony-Anhalt" },
                new State { Id = 15, StateName = "Schleswig-Holstein" },
                new State { Id = 16, StateName = "Thuringia" }
            );

            // Cities
            modelBuilder.Entity<City>().HasData(
                new City { Id = 1, StateId = 3, CityName = "Berlin", Population = 3460725, Area = 887.7M },
                new City { Id = 2, StateId = 6, CityName = "Hamburg", Population = 1786448, Area = 755.16M },
                new City { Id = 3, StateId = 2, CityName = "Munich / Munchen", Population = 1353186, Area = 310.69M },
                new City { Id = 4, StateId = 10, CityName = "Cologne / Koln", Population = 1007119, Area = 405.17M },
                new City { Id = 5, StateId = 7, CityName = "Frankfurt am Main", Population = 679664, Area = 248.31M },
                new City { Id = 6, StateId = 1, CityName = "Stuttgart", Population = 606588, Area = 207.35M },
                new City { Id = 7, StateId = 10, CityName = "Dusseldorf", Population = 588735, Area = 217.22M },
                new City { Id = 8, StateId = 10, CityName = "Dortmund", Population = 580444, Area = 280.71M },
                new City { Id = 9, StateId = 10, CityName = "Essen", Population = 574635, Area = 210.32M },
                new City { Id = 10, StateId = 5, CityName = "Bremen", Population = 547340, Area = 325.42M },
                new City { Id = 11, StateId = 13, CityName = "Dresden", Population = 523058, Area = 328.31M },
                new City { Id = 12, StateId = 13, CityName = "Leipzig", Population = 522883, Area = 297.36M },
                new City { Id = 13, StateId = 8, CityName = "Hanover / Hannover", Population = 522686, Area = 204.14M },
                new City { Id = 14, StateId = 2, CityName = "Nuremberg / Nurnberg", Population = 505664, Area = 186.38M },
                new City { Id = 15, StateId = 10, CityName = "Duisburg", Population = 489599, Area = 232.83M },
                new City { Id = 16, StateId = 10, CityName = "Bochum", Population = 374737, Area = 145.66M },
                new City { Id = 17, StateId = 10, CityName = "Wuppertal", Population = 349721, Area = 168.39M },
                new City { Id = 18, StateId = 10, CityName = "Bonn", Population = 324899, Area = 141.22M },
                new City { Id = 19, StateId = 10, CityName = "Bielefeld", Population = 323270, Area = 257.92M },
                new City { Id = 20, StateId = 1, CityName = "Mannheim", Population = 313174, Area = 144.96M },
                new City { Id = 21, StateId = 1, CityName = "Karlsruhe", Population = 294761, Area = 173.46M },
                new City { Id = 22, StateId = 10, CityName = "Munster", Population = 279803, Area = 302.96M },
                new City { Id = 23, StateId = 7, CityName = "Wiesbaden", Population = 275976, Area = 203.93M },
                new City { Id = 24, StateId = 2, CityName = "Augsburg", Population = 264708, Area = 146.84M },
                new City { Id = 25, StateId = 10, CityName = "Aachen", Population = 258664, Area = 160.84M },
                new City { Id = 26, StateId = 10, CityName = "Monchengladbach", Population = 257993, Area = 170.45M },
                new City { Id = 27, StateId = 10, CityName = "Gelsenkirchen", Population = 257981, Area = 104.94M },
                new City { Id = 28, StateId = 8, CityName = "Brunswick / Braunschweig", Population = 248867, Area = 192.15M },
                new City { Id = 29, StateId = 13, CityName = "Chemnitz", Population = 243248, Area = 220.84M },
                new City { Id = 30, StateId = 15, CityName = "Kiel", Population = 239526, Area = 118.65M },
                new City { Id = 31, StateId = 10, CityName = "Krefeld", Population = 235076, Area = 137.75M },
                new City { Id = 32, StateId = 14, CityName = "Halle", Population = 232963, Area = 135.02M },
                new City { Id = 33, StateId = 14, CityName = "Magdeburg", Population = 231549, Area = 200.99M },
                new City { Id = 34, StateId = 1, CityName = "Freiburg", Population = 224191, Area = 153.06M },
                new City { Id = 35, StateId = 10, CityName = "Oberhausen", Population = 212945, Area = 77.11M },
                new City { Id = 36, StateId = 15, CityName = "Lübeck", Population = 210232, Area = 214.21M },
                new City { Id = 37, StateId = 16, CityName = "Erfurt", Population = 204994, Area = 269.14M },
                new City { Id = 38, StateId = 9, CityName = "Rostock", Population = 202735, Area = 181.26M },
                new City { Id = 39, StateId = 11, CityName = "Mainz", Population = 199237, Area = 97.74M },
                new City { Id = 40, StateId = 7, CityName = "Kassel", Population = 195530, Area = 106.78M },
                new City { Id = 41, StateId = 10, CityName = "Hagen", Population = 188529, Area = 160.35M },
                new City { Id = 42, StateId = 10, CityName = "Hamm", Population = 181783, Area = 226.25M },
                new City { Id = 43, StateId = 12, CityName = "Saarbrucken", Population = 175741, Area = 167.09M },
                new City { Id = 44, StateId = 10, CityName = "Mulheim", Population = 167344, Area = 91.29M },
                new City { Id = 45, StateId = 10, CityName = "Herne", Population = 164762, Area = 51.41M },
                new City { Id = 46, StateId = 11, CityName = "Ludwigshafen", Population = 164177, Area = 77.55M },
                new City { Id = 47, StateId = 8, CityName = "Osnabruck", Population = 164119, Area = 119.8M },
                new City { Id = 48, StateId = 8, CityName = "Oldenburg", Population = 162173, Area = 102.98M },
                new City { Id = 49, StateId = 10, CityName = "Leverkusen", Population = 160772, Area = 78.87M },
                new City { Id = 50, StateId = 10, CityName = "Solingen", Population = 159927, Area = 89.54M },
                new City { Id = 51, StateId = 4, CityName = "Potsdam", Population = 156906, Area = 187.53M },
                new City { Id = 52, StateId = 10, CityName = "Neuss", Population = 151388, Area = 99.53M },
                new City { Id = 53, StateId = 1, CityName = "Heidelberg", Population = 147312, Area = 108.83M },
                new City { Id = 54, StateId = 10, CityName = "Paderborn", Population = 146283, Area = 179.51M },
                new City { Id = 55, StateId = 7, CityName = "Darmstadt", Population = 144402, Area = 122.09M },
                new City { Id = 56, StateId = 2, CityName = "Regensburg", Population = 135520, Area = 80.7M },
                new City { Id = 57, StateId = 2, CityName = "Wurzburg", Population = 133799, Area = 87.63M },
                new City { Id = 58, StateId = 2, CityName = "Ingolstadt", Population = 125088, Area = 133.37M },
                new City { Id = 59, StateId = 1, CityName = "Heilbronn", Population = 122879, Area = 99.88M },
                new City { Id = 60, StateId = 1, CityName = "Ulm", Population = 122801, Area = 118.69M },
                new City { Id = 61, StateId = 8, CityName = "Wolfsburg", Population = 121451, Area = 204.05M },
                new City { Id = 62, StateId = 8, CityName = "Gottingen", Population = 121060, Area = 116.89M },
                new City { Id = 63, StateId = 7, CityName = "Offenbach", Population = 120435, Area = 44.89M },
                new City { Id = 64, StateId = 1, CityName = "Pforzheim", Population = 119781, Area = 98M },
                new City { Id = 65, StateId = 10, CityName = "Recklinghausen", Population = 118365, Area = 66.43M },
                new City { Id = 66, StateId = 10, CityName = "Bottrop", Population = 116771, Area = 100.61M },
                new City { Id = 67, StateId = 2, CityName = "Furth", Population = 114628, Area = 63.35M },
                new City { Id = 68, StateId = 5, CityName = "Bremerhaven", Population = 113366, Area = 93.82M },
                new City { Id = 69, StateId = 1, CityName = "Reutlingen", Population = 112484, Area = 87.06M },
                new City { Id = 70, StateId = 10, CityName = "Remscheid", Population = 110563, Area = 74.6M },
                new City { Id = 71, StateId = 11, CityName = "Koblenz", Population = 106417, Area = 105.05M },
                new City { Id = 72, StateId = 10, CityName = "Bergisch Gladbach", Population = 105723, Area = 83.11M },
                new City { Id = 73, StateId = 2, CityName = "Erlangen", Population = 105629, Area = 76.95M },
                new City { Id = 74, StateId = 10, CityName = "Moers", Population = 105506, Area = 67.69M },
                new City { Id = 75, StateId = 11, CityName = "Trier", Population = 105260, Area = 117.13M },
                new City { Id = 76, StateId = 16, CityName = "Jena", Population = 105129, Area = 114.47M },
                new City { Id = 77, StateId = 10, CityName = "Siegen", Population = 103424, Area = 114.67M },
                new City { Id = 78, StateId = 8, CityName = "Hildesheim", Population = 102794, Area = 92.18M },
                new City { Id = 79, StateId = 8, CityName = "Salzgitter", Population = 102394, Area = 223.91M },
                new City { Id = 80, StateId = 4, CityName = "Cottbus", Population = 102091, Area = 164.29M }
            );
        }

        /// <summary>
        /// This SeedData method runs along with the application run.
        /// </summary>
        public static async Task SeedData(IServiceProvider services)
        {
            IWebHostEnvironment environment = services.GetRequiredService<IWebHostEnvironment>();

            if (environment.IsDevelopment())
            {
                IServiceScope scope = services.CreateScope();

                AppDbContext context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                context.Database.EnsureCreated();

                if (context.Database.GetPendingMigrations().Any()) await context.Database.MigrateAsync();


                /// Normally the DbContext takes care of the transaction, but in this case (SET IDENTITY_INSERT) 
                /// manually taking care of the transactions are required. 
                /// Database context will generate a BEGIN TRAN after the SET IDENTITY_INSERT is issued.
                /// This will make transaction's inserts to fail since IDENTITY_INSERT seems to affect tables at session/transaction level.
                /// So, everything must be wrapped in a single transaction to work properly.
                using var transaction = context.Database.BeginTransaction();

                if (!context.Iller.Any())
                {
                    var iller = JsonConvert.DeserializeObject<List<Il>>(File.ReadAllText(
                        Path.Combine(environment.WebRootPath, "static", "iller.json")));
                    context.Iller.AddRange(iller!);

                    if (context.Database.IsSqlServer()) await context.EnableIdentityInsert<Il>();
                    await context.SaveChangesAsync();
                    if (context.Database.IsSqlServer()) await context.DisableIdentityInsert<Il>();
                }

                if (!context.Ilceler.Any())
                {
                    var ilceler = JsonConvert.DeserializeObject<List<Ilce>>(File.ReadAllText(
                        Path.Combine(Environment.CurrentDirectory, "wwwroot", "static", "ilceler.json")));
                    context.Ilceler.AddRange(ilceler!);

                    if (context.Database.IsSqlServer()) await context.EnableIdentityInsert<Ilce>();
                    await context.SaveChangesAsync();
                    if (context.Database.IsSqlServer()) await context.DisableIdentityInsert<Ilce>();
                }

                if (!context.SemtBucakBeldeler.Any())
                {
                    var sbbler = JsonConvert.DeserializeObject<List<SemtBucakBelde>>(File.ReadAllText(
                        Path.Combine(Environment.CurrentDirectory, "wwwroot", "static", "semtbucakbeldeler.json")));
                    context.SemtBucakBeldeler.AddRange(sbbler!);

                    if (context.Database.IsSqlServer()) await context.EnableIdentityInsert<SemtBucakBelde>();
                    await context.SaveChangesAsync();
                    if (context.Database.IsSqlServer()) await context.DisableIdentityInsert<SemtBucakBelde>();
                }

                if (!context.Mahalleler.Any())
                {
                    var mahalleler = JsonConvert.DeserializeObject<List<Mahalle>>(File.ReadAllText(
                        Path.Combine(Environment.CurrentDirectory, "wwwroot", "static", "mahalleler.json")));
                    context.Mahalleler.AddRange(mahalleler!);

                    if (context.Database.IsSqlServer()) await context.EnableIdentityInsert<Mahalle>();
                    await context.SaveChangesAsync();
                    if (context.Database.IsSqlServer()) await context.DisableIdentityInsert<Mahalle>();
                }

                transaction.Commit();
            }
        }

        #region SQL Server Specific

        public static Task EnableIdentityInsert<T>(this DbContext context) => SetIdentityInsert<T>(context, enable: true);
        public static Task DisableIdentityInsert<T>(this DbContext context) => SetIdentityInsert<T>(context, enable: false);
        private static Task SetIdentityInsert<T>(DbContext context, bool enable)
        {
            var entityType = context.Model.FindEntityType(typeof(T))!;
            var value = enable ? "ON" : "OFF";
#pragma warning disable EF1002 // Risk of vulnerability to SQL injection.
            return context.Database.ExecuteSqlRawAsync($"SET IDENTITY_INSERT {entityType.GetSchema()}.{entityType.GetTableName()} {value}");
#pragma warning restore EF1002 // Risk of vulnerability to SQL injection.
        }
        
        #endregion SQL Server Specific
    }
}
