namespace src.App_Data.Entities
{
    public partial class Ilce
    {
        public int IlceId { get; set; }
        public string IlceAdi { get; set; } = null!;
        public virtual ICollection<SemtBucakBelde>? SemtBucakBeldeler { get; set; }
        //public int IlId { get; set; }
        public Il Il { get; set; } = null!;
    }
}
