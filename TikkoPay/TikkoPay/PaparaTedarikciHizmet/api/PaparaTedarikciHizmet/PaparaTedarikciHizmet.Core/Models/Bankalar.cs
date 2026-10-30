namespace PaparaTedarikciHizmet.Core.Models
{
    public class Bankalar : BaseModel
    {
        public Bankalar()
        {
            HavaleBankalari = new List<HavaleBankalari>();
        }

        public string Baslik { get; set; } = string.Empty;

        public ICollection<HavaleBankalari> HavaleBankalari { get; set; }
        public ICollection<YatirimCekimTalepleri> YatirimTalepleri { get; set; }

    }
}
