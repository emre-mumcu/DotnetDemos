namespace src.Lib
{
    public sealed class App
    {
        # region Singleton

        private static readonly Lazy<App> appInstance = new Lazy<App>(() => new App());        

        private App()
        {
            DataConfiguration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("app.json", true)
                .Build();            
        }

        public static App Instance { get { return appInstance.Value; } }

        #endregion Singleton

        
        public IConfigurationRoot DataConfiguration { get; set; }
        
        public IWebHostEnvironment? WebHostEnvironment { get; set; }
    }
}