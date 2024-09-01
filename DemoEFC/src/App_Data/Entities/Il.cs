namespace src.App_Data.Entities
{
    public partial class Il
    {
        public int IlId { get; set; }
        public string IlAdi { get; set; } = null!;
        public virtual ICollection<Ilce>? Ilceler { get; set; }
    }
}