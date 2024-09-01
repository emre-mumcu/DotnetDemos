namespace src.App_Data.Entities
{
    public class Mahalle
    {
        public int MahalleId { get; set; }
        public string MahalleAdi { get; set; } = null!;
        public string PostaKodu { get; set; } = null!;
        public int SemtBucakBeldeId { get; set; }
        public SemtBucakBelde SemtBucakBelde { get; set; } = null!;
    }
}
