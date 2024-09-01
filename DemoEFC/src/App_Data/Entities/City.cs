namespace src.App_Data.Entities
{
    public class City : EntityBase
    {
        public int StateId { get; set; }
        public string CityName { get; set; } = null!;
        public decimal Population { get; set; }
        public decimal Area { get; set; }
    }
}
