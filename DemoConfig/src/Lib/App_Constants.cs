namespace src.Lib
{
    public class App_Constants
    {
        public static IEnumerable<KeyValuePair<string, string?>>? App_Keys = new Dictionary<string, string?> { 
            { "App_Keys_TimeStamp", DateTime.UtcNow.ToString() } 
        };
    }
}