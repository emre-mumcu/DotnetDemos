namespace src.App_Data.Entities
{
    public partial class SemtBucakBelde
    {
        public int SemtBucakBeldeId { get; set; }
        public string SemtBucakBeldeAdi { get; set; } = null!;
        public virtual ICollection<Mahalle>? Mahalleler { get; set; }
        //public int IlceId { get; set; }
        public Ilce Ilce { get; set; } = null!;
    }
}
